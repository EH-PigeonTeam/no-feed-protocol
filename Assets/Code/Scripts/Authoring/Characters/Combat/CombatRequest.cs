using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Battle.Players;
using NoFeedProtocol.Runtime.Services.Characters;
using NoFeedProtocol.Runtime.Services.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
    /// <summary>
    /// Describes a combat action to be resolved.
    /// </summary>
    public readonly struct CombatRequest
    {
        public CharacterRuntimeData Attacker { get; }
        public PlayerController AttackerTeam { get; }

        public CharacterRuntimeData Defender { get; }
        public PlayerController DefenderTeam { get; }

        public CombatRequest(
            CharacterRuntimeData attacker,
            PlayerController attackerTeam,
            CharacterRuntimeData defender,
            PlayerController defenderTeam)
        {
            Attacker = attacker;
            AttackerTeam = attackerTeam;
            Defender = defender;
            DefenderTeam = defenderTeam;
        }
    }

    /// <summary>
    /// Represents the outcome of a resolved combat action.
    /// </summary>
    public class CombatResult
    {
        public int HealthTop { get; }
        public int HealthBottom { get; }
        public int ShieldChange { get; }

        public CombatResult(int characterTop, int characterBottom, int shieldChange)
        {
            HealthTop = characterTop;
            HealthBottom = characterBottom;
            ShieldChange = shieldChange;
        }
    }

    /// <summary>
    /// Handles combat logic evaluation and result generation.
    /// </summary>
    public static class CombatResolver
    {
        public static void Resolve(CombatRequest request, CombatTriggerType situation = CombatTriggerType.OnAttackReady)
        {
            // Get item-based modifiers for damage
            ItemResolver resolver = ServiceLocator.Get<ItemResolver>();
            List<string> attackerItemIds = request.AttackerTeam.RuntimeData.Items;
            List<string> defenderItemIds = request.DefenderTeam.RuntimeData.Items;

            int hpDamageBonus = resolver.GetTotalValueForStat(attackerItemIds, StatType.HpDamage);
            int shieldDamageBonus = resolver.GetTotalValueForStat(attackerItemIds, StatType.ShieldDamage);
            int hpBonus = resolver.GetTotalValueForStat(attackerItemIds, StatType.Hp);
            int shieldBonus = resolver.GetTotalValueForStat(attackerItemIds, StatType.Shield);

            ICharacterStaticData attackerStatic = request.AttackerTeam.Resolver.GetById(request.Attacker.Id);
            ICharacterStaticData defenderStatic = request.DefenderTeam.Resolver.GetById(request.Defender.Id);
            CombatBehavior behavior = attackerStatic.CombatBehavior;

            int dmgTopDef = 0, dmgBottomDef = 0, shieldDef = 0;
            int dmgTopAtk = 0, dmgBottomAtk = 0, shieldAtk = 0;

            bool defenderHasShield = request.DefenderTeam.RuntimeData.CurrentShield > 0;

            foreach (CombatTriggerBlock trigger in behavior.Triggers)
            {
                if (trigger.Trigger != situation)
                {
                    continue;
                }

                foreach (CombatActionBlock block in trigger.Actions)
                {
                    AttackMode mode = GetAttackMode(block, request.AttackerTeam, request.DefenderTeam, request.Attacker);
                    if (mode == AttackMode.Invalid)
                    {
                        continue;
                    }

                    foreach (CombatAction action in block.Sequence)
                    {
                        int finalValue = action.OverrideValue
                            ? action.Value
                            : GetContextualValue(attackerStatic, mode);

                        int val = Mathf.RoundToInt(finalValue * action.Modifier);

                        switch (action.Target)
                        {
                            case CombatTargetType.EnemyTargeted:
                                {
                                    CharacterRuntimeData top = request.DefenderTeam.RuntimeData.CharacterTop;
                                    CharacterRuntimeData bottom = request.DefenderTeam.RuntimeData.CharacterBottom;
                                    CharacterRuntimeData target = (request.Defender == top) ? top : bottom;

                                    int maxHp = request.DefenderTeam.Resolver.GetById(target.Id).MaxHealth;

                                    if (mode == AttackMode.ShieldOnly || (defenderHasShield && mode == AttackMode.Normal))
                                    {
                                        int currentShield = request.DefenderTeam.RuntimeData.CurrentShield;
                                        int rawShieldDamage = val + shieldDamageBonus;
                                        int clampedShield = Mathf.Clamp(currentShield - rawShieldDamage, 0, maxHp);
                                        int deltaShield = clampedShield - currentShield;
                                        shieldDef += deltaShield;
                                    }
                                    else
                                    {
                                        int currentHealth = target.Health;
                                        int rawHpDamage = val + hpDamageBonus;
                                        int clampedHealth = Mathf.Clamp(currentHealth - rawHpDamage, 0, maxHp);
                                        int deltaHp = clampedHealth - currentHealth;

                                        if (target == top)
                                        {
                                            dmgTopDef += deltaHp;
                                        }
                                        else
                                        {
                                            dmgBottomDef += deltaHp;
                                        }
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyAll:
                                {
                                    if (mode == AttackMode.ShieldOnly ||
                                        (defenderHasShield && mode == AttackMode.Normal))
                                    {
                                        int currentShield = request.DefenderTeam.RuntimeData.CurrentShield;
                                        int rawShieldDamage = val + shieldDamageBonus;
                                        int newShield = Mathf.Clamp(currentShield - rawShieldDamage, 0, currentShield);
                                        int shieldDelta = newShield - currentShield;
                                        shieldDef += shieldDelta;
                                    }
                                    else
                                    {
                                        int rawHpDamage = val + hpDamageBonus;

                                        {
                                            var top = request.DefenderTeam.RuntimeData.CharacterTop;
                                            int maxHp = request.DefenderTeam.Resolver.GetById(top.Id).MaxHealth;
                                            int currentHealth = top.Health;
                                            int newHealth = Mathf.Clamp(currentHealth - rawHpDamage, 0, maxHp);
                                            int delta = newHealth - currentHealth;
                                            dmgTopDef += delta;
                                        }

                                        {
                                            var bottom = request.DefenderTeam.RuntimeData.CharacterBottom;
                                            int maxHp = request.DefenderTeam.Resolver.GetById(bottom.Id).MaxHealth;
                                            int currentHealth = bottom.Health;
                                            int newHealth = Mathf.Clamp(currentHealth - rawHpDamage, 0, maxHp);
                                            int delta = newHealth - currentHealth;
                                            dmgBottomDef += delta;
                                        }
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyOther:
                                {
                                    CharacterRuntimeData other = request.Defender == request.DefenderTeam.RuntimeData.CharacterTop
                                        ? request.DefenderTeam.RuntimeData.CharacterBottom
                                        : request.DefenderTeam.RuntimeData.CharacterTop;

                                    int maxHp = request.DefenderTeam.Resolver.GetById(other.Id).MaxHealth;

                                    int currentHealth = other.Health;
                                    int rawDamage = val + hpDamageBonus;
                                    int rawNewHealth = currentHealth - rawDamage;
                                    int clampedHealth = Mathf.Clamp(rawNewHealth, 0, maxHp);
                                    int delta = clampedHealth - currentHealth;

                                    if (other == request.DefenderTeam.RuntimeData.CharacterTop)
                                    {
                                        dmgTopDef += delta;
                                    }
                                    else
                                    {
                                        dmgBottomDef += delta;
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyAttacker:
                                {
                                    CharacterRuntimeData top = request.DefenderTeam.RuntimeData.CharacterTop;
                                    CharacterRuntimeData bottom = request.DefenderTeam.RuntimeData.CharacterBottom;
                                    CharacterRuntimeData target = (request.Defender == top) ? top : bottom;

                                    int maxHp = request.DefenderTeam.Resolver.GetById(target.Id).MaxHealth;

                                    int damage = val + hpDamageBonus;
                                    int currentHealth = target.Health;

                                    int clampedHealth = Mathf.Clamp(currentHealth - damage, 0, maxHp);

                                    int delta = clampedHealth - currentHealth;

                                    if (target == top)
                                    {
                                        dmgTopDef += delta;
                                    }
                                    else
                                    {
                                        dmgBottomDef += delta;
                                    }

                                    break;
                                }

                            case CombatTargetType.Self:
                                {
                                    CharacterRuntimeData top = request.AttackerTeam.RuntimeData.CharacterTop;
                                    CharacterRuntimeData bottom = request.AttackerTeam.RuntimeData.CharacterBottom;
                                    CharacterRuntimeData target = request.Attacker;

                                    int maxHealth = request.AttackerTeam.Resolver.GetById(target.Id).MaxHealth + hpBonus;

                                    int clampedNewHealth = Mathf.Clamp(target.Health + val, 0, maxHealth);
                                    val = clampedNewHealth - target.Health;

                                    if (target == top)
                                    {
                                        dmgTopAtk += val;
                                    }
                                    else
                                    {
                                        dmgBottomAtk += val;
                                    }

                                    break;
                                }

                            case CombatTargetType.SelfShield:
                                {
                                    int currentShield = request.AttackerTeam.RuntimeData.CurrentShield;
                                    int maxShield = request.AttackerTeam.RuntimeData.MaxShield + shieldBonus;

                                    int rawNewShield = currentShield + val;
                                    int clampedShield = Mathf.Clamp(rawNewShield, 0, maxShield);

                                    int delta = clampedShield - currentShield;
                                    shieldAtk += delta;
                                    break;
                                }

                            case CombatTargetType.AllyLowestHP:
                                {
                                    var top = request.AttackerTeam.RuntimeData.CharacterTop;
                                    var bottom = request.AttackerTeam.RuntimeData.CharacterBottom;
                                    var target = (top.Health <= bottom.Health && top.Health > 0) ? top : bottom;

                                    int maxHealth = request.AttackerTeam.Resolver
                                                                  .GetById(target.Id)
                                                                  .MaxHealth
                                                    + hpBonus;

                                    int clampedNewHealth = Mathf.Clamp(target.Health + val, 0, maxHealth);
                                    val = clampedNewHealth - target.Health;

                                    if (target == top)
                                    {
                                        dmgTopAtk += val;
                                    }
                                    else
                                    {
                                        dmgBottomAtk += val;
                                    }

                                    break;
                                }
                        }
                    }
                }
            }

            request.DefenderTeam.ApplyCombatDeltas(new CombatResult(dmgTopDef, dmgBottomDef, shieldDef));
            request.AttackerTeam.ApplyCombatDeltas(new CombatResult(dmgTopAtk, dmgBottomAtk, shieldAtk));
        }

        private static int GetContextualValue(
            ICharacterStaticData attackerStatic,
            AttackMode mode)
        {
            return mode switch
            {
                AttackMode.Override => 0,
                AttackMode.ShieldOnly => attackerStatic.AttackPointsShield,
                AttackMode.Normal => attackerStatic.AttackPoints,
                _ => attackerStatic.AttackPoints
            };
        }

        private static AttackMode GetAttackMode(
            CombatActionBlock block,
            PlayerController attackerTeam,
            PlayerController defenderTeam,
            CharacterRuntimeData self)
        {
            CombatConditionType type = block.Conditions;

            if (type == CombatConditionType.Always)
            {
                return block.Conditions == CombatConditionType.SelfHpBelow ? AttackMode.Override : AttackMode.Normal;
            }

            bool defenderHasShield = defenderTeam.RuntimeData.CurrentShield > 0;

            if (type.HasFlag(CombatConditionType.WithShield))
            {
                return defenderHasShield ? AttackMode.ShieldOnly : AttackMode.Invalid;
            }

            if (type.HasFlag(CombatConditionType.WithOutShield))
            {
                return defenderHasShield ? AttackMode.Invalid : AttackMode.Normal;
            }

            if (type.HasFlag(CombatConditionType.SelfHpBelow) && self.Health >= block.Value)
            {
                return AttackMode.Invalid;
            }

            if (type.HasFlag(CombatConditionType.HasEnemyAtLeast))
            {
                int alive = 0;
                if (defenderTeam.RuntimeData.CharacterTop?.IsAlive == true)
                {
                    alive++;
                }

                if (defenderTeam.RuntimeData.CharacterBottom?.IsAlive == true)
                {
                    alive++;
                }

                if (alive < block.Value)
                {
                    return AttackMode.Invalid;
                }
            }

            if (type.HasFlag(CombatConditionType.HasAllyAtLeast))
            {
                int alive = 0;
                if (attackerTeam.RuntimeData.CharacterTop?.IsAlive == true)
                {
                    alive++;
                }

                if (attackerTeam.RuntimeData.CharacterBottom?.IsAlive == true)
                {
                    alive++;
                }

                if (alive < block.Value)
                {
                    return AttackMode.Invalid;
                }
            }

            return block.Conditions == CombatConditionType.SelfHpBelow ? AttackMode.Override : AttackMode.Normal;
        }
    }

    public enum AttackMode
    {
        Invalid,
        Normal,
        ShieldOnly,
        Override
    }
}
