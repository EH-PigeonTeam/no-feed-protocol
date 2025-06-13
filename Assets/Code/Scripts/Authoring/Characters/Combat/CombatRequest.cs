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
                                    if (mode == AttackMode.ShieldOnly)
                                    {
                                        shieldDef -= val + shieldDamageBonus;
                                    }
                                    else
                                    {
                                        if (defenderHasShield && mode == AttackMode.Normal)
                                        {
                                            shieldDef -= val + shieldDamageBonus;
                                        }
                                        else if (request.Defender == request.DefenderTeam.RuntimeData.CharacterTop)
                                        {
                                            dmgTopDef -= val + hpDamageBonus;
                                        }
                                        else
                                        {
                                            dmgBottomDef -= val + hpDamageBonus;
                                        }
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyAll:
                                {
                                    if (mode == AttackMode.ShieldOnly)
                                    {
                                        shieldDef -= val + shieldDamageBonus;
                                    }
                                    else
                                    {
                                        if (defenderHasShield && mode == AttackMode.Normal)
                                        {
                                            shieldDef -= val + shieldDamageBonus;
                                        }
                                        else
                                        {
                                            dmgTopDef -= val + hpDamageBonus;
                                            dmgBottomDef -= val + hpDamageBonus;
                                        }
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyOther:
                                {
                                    CharacterRuntimeData other = request.Defender == request.DefenderTeam.RuntimeData.CharacterTop
                                        ? request.DefenderTeam.RuntimeData.CharacterBottom
                                        : request.DefenderTeam.RuntimeData.CharacterTop;

                                    if (other == request.DefenderTeam.RuntimeData.CharacterTop)
                                    {
                                        dmgTopDef -= val + hpDamageBonus;
                                    }
                                    else
                                    {
                                        dmgBottomDef -= val + hpDamageBonus;
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyAttacker:
                                {
                                    if (request.Defender == request.DefenderTeam.RuntimeData.CharacterTop)
                                    {
                                        dmgTopDef -= val + hpDamageBonus;
                                    }
                                    else
                                    {
                                        dmgBottomDef -= val + hpDamageBonus;
                                    }

                                    break;
                                }

                            case CombatTargetType.Self:
                                {
                                    if (request.Attacker == request.AttackerTeam.RuntimeData.CharacterTop)
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
                                    shieldAtk += Mathf.Max(val, request.AttackerTeam.RuntimeData.MaxShield + shieldBonus);
                                    break;
                                }

                            case CombatTargetType.AllyLowestHP:
                                {
                                    CharacterRuntimeData top = request.AttackerTeam.RuntimeData.CharacterTop;
                                    CharacterRuntimeData bottom = request.AttackerTeam.RuntimeData.CharacterBottom;
                                    CharacterRuntimeData target = (top.Health <= bottom.Health && top.Health > 0) ? top : bottom;

                                    int maxHealth = ServiceLocator.Get<CharacterResolver>().GetById(target.Id).MaxHealth + hpBonus;

                                    val = Mathf.Max(val, maxHealth - target.Health);

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
