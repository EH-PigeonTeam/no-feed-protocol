using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Battle.Players;
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
            var attackerStatic = request.AttackerTeam.Resolver.GetById(request.Attacker.Id);
            var defenderStatic = request.DefenderTeam.Resolver.GetById(request.Defender.Id);
            var behavior = attackerStatic.CombatBehavior;

            int dmgTopDef = 0, dmgBottomDef = 0, shieldDef = 0;
            int dmgTopAtk = 0, dmgBottomAtk = 0, shieldAtk = 0;

            foreach (var trigger in behavior.Triggers)
            {
                if (trigger.Trigger != situation)
                    continue;

                foreach (var block in trigger.Actions)
                {
                    if (!IsConditionMet(block, request.AttackerTeam, request.DefenderTeam, request.Attacker))
                        continue;

                    foreach (var action in block.Sequence)
                    {
                        int baseValue = action.OverrideValue
                            ? action.Value
                            : attackerStatic.AttackPoints;

                        int modified = Mathf.RoundToInt(baseValue * action.Modifier);

                        switch (action.Target)
                        {
                            case CombatTargetType.EnemyTargeted:
                                {
                                    bool defenderHasShield = request.DefenderTeam.RuntimeData.CurrentShield > 0;
                                    int finalValue = action.OverrideValue
                                        ? action.Value
                                        : GetContextualValue(attackerStatic, block.Conditions, defenderHasShield);

                                    int val = Mathf.RoundToInt(finalValue * action.Modifier);

                                    if (defenderHasShield && !action.OverrideValue)
                                    {
                                        shieldDef -= val;
                                    }
                                    else
                                    {
                                        if (request.Defender == request.DefenderTeam.RuntimeData.CharacterTop)
                                            dmgTopDef -= val;
                                        else
                                            dmgBottomDef -= val;
                                    }
                                    break;
                                }

                            case CombatTargetType.EnemyAll:
                                dmgTopDef -= modified;
                                dmgBottomDef -= modified;
                                break;

                            case CombatTargetType.EnemyOther:
                                {
                                    var other = request.Defender == request.DefenderTeam.RuntimeData.CharacterTop
                                        ? request.DefenderTeam.RuntimeData.CharacterBottom
                                        : request.DefenderTeam.RuntimeData.CharacterTop;

                                    if (other == request.DefenderTeam.RuntimeData.CharacterTop)
                                        dmgTopDef -= modified;
                                    else
                                        dmgBottomDef -= modified;
                                    break;
                                }

                            case CombatTargetType.EnemyAttacker:
                                {
                                    var target = request.Defender;

                                    if (target == request.DefenderTeam.RuntimeData.CharacterTop)
                                        dmgTopDef -= modified;
                                    else
                                        dmgBottomDef -= modified;
                                    break;
                                }

                            case CombatTargetType.Self:
                                {
                                    if (request.Attacker == request.AttackerTeam.RuntimeData.CharacterTop)
                                        dmgTopAtk += modified;
                                    else
                                        dmgBottomAtk += modified;
                                    break;
                                }

                            case CombatTargetType.SelfShield:
                                shieldAtk += modified;
                                break;

                            case CombatTargetType.AllyLowestHP:
                                {
                                    var top = request.AttackerTeam.RuntimeData.CharacterTop;
                                    var bottom = request.AttackerTeam.RuntimeData.CharacterBottom;
                                    var target = (top.Health <= bottom.Health) ? top : bottom;

                                    if (target == top)
                                        dmgTopAtk += modified;
                                    else
                                        dmgBottomAtk += modified;
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
            CombatConditionType conditions,
            bool targetHasShield)
        {
            bool explicitWithShield = conditions.HasFlag(CombatConditionType.WithShield);
            bool explicitWithoutShield = conditions.HasFlag(CombatConditionType.WithOutShield);

            if (explicitWithShield)
                return attackerStatic.AttackPointsShield;

            if (explicitWithoutShield)
                return attackerStatic.AttackPoints;

            // Implicit runtime decision
            return targetHasShield ? attackerStatic.AttackPointsShield : attackerStatic.AttackPoints;
        }

        private static bool IsConditionMet(
            CombatActionBlock block,
            PlayerController attackerTeam,
            PlayerController defenderTeam,
            CharacterRuntimeData self)
        {
            var type = block.Conditions;

            if (type == CombatConditionType.Always)
                return true;

            bool hasShield = attackerTeam.RuntimeData.CurrentShield > 0;

            if (type.HasFlag(CombatConditionType.WithShield) && !hasShield)
                return false;

            if (type.HasFlag(CombatConditionType.WithOutShield) && hasShield)
                return false;

            if (type.HasFlag(CombatConditionType.SelfHpBelow) && self.Health >= block.Value)
                return false;

            if (type.HasFlag(CombatConditionType.HasEnemyAtLeast))
            {
                int alive = 0;
                if (defenderTeam.RuntimeData.CharacterTop?.Health > 0)
                    alive++;
                if (defenderTeam.RuntimeData.CharacterBottom?.Health > 0)
                    alive++;

                if (alive < block.Value)
                    return false;
            }

            if (type.HasFlag(CombatConditionType.HasAllyAtLeast))
            {
                int alive = 0;
                if (attackerTeam.RuntimeData.CharacterTop?.Health > 0)
                    alive++;
                if (attackerTeam.RuntimeData.CharacterBottom?.Health > 0)
                    alive++;

                if (alive < block.Value)
                    return false;
            }

            return true;
        }
    }
}
