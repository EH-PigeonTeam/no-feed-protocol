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
                    var mode = GetAttackMode(block, request.AttackerTeam, request.DefenderTeam, request.Attacker);
                    if (mode == AttackMode.Invalid)
                        continue;

                    foreach (var action in block.Sequence)
                    {
                        int finalValue = action.OverrideValue
                            ? action.Value
                            : GetContextualValue(attackerStatic, mode);

                        int val = Mathf.RoundToInt(finalValue * action.Modifier);

                        switch (action.Target)
                        {
                            case CombatTargetType.EnemyTargeted:
                                {
                                    bool defenderHasShield = request.DefenderTeam.RuntimeData.CurrentShield > 0;

                                    if (mode == AttackMode.ShieldOnly)
                                    {
                                        shieldDef -= val;
                                    }
                                    else
                                    {
                                        if (defenderHasShield && mode == AttackMode.Normal)
                                            shieldDef -= val;
                                        else if (request.Defender == request.DefenderTeam.RuntimeData.CharacterTop)
                                            dmgTopDef -= val;
                                        else
                                            dmgBottomDef -= val;
                                    }

                                    break;
                                }

                            case CombatTargetType.EnemyAll:
                                dmgTopDef -= val;
                                dmgBottomDef -= val;
                                break;

                            case CombatTargetType.EnemyOther:
                                {
                                    var other = request.Defender == request.DefenderTeam.RuntimeData.CharacterTop
                                        ? request.DefenderTeam.RuntimeData.CharacterBottom
                                        : request.DefenderTeam.RuntimeData.CharacterTop;

                                    if (other == request.DefenderTeam.RuntimeData.CharacterTop)
                                        dmgTopDef -= val;
                                    else
                                        dmgBottomDef -= val;
                                    break;
                                }

                            case CombatTargetType.EnemyAttacker:
                                {
                                    if (request.Defender == request.DefenderTeam.RuntimeData.CharacterTop)
                                        dmgTopDef -= val;
                                    else
                                        dmgBottomDef -= val;
                                    break;
                                }

                            case CombatTargetType.Self:
                                {
                                    if (request.Attacker == request.AttackerTeam.RuntimeData.CharacterTop)
                                        dmgTopAtk += val;
                                    else
                                        dmgBottomAtk += val;
                                    break;
                                }

                            case CombatTargetType.SelfShield:
                                shieldAtk += val;
                                break;

                            case CombatTargetType.AllyLowestHP:
                                {
                                    var top = request.AttackerTeam.RuntimeData.CharacterTop;
                                    var bottom = request.AttackerTeam.RuntimeData.CharacterBottom;
                                    var target = (top.Health <= bottom.Health) ? top : bottom;

                                    if (target == top)
                                        dmgTopAtk += val;
                                    else
                                        dmgBottomAtk += val;
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
                AttackMode.Override => 0, // già gestito altrove
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
            var type = block.Conditions;

            if (type == CombatConditionType.Always)
                return block.Conditions == CombatConditionType.SelfHpBelow ? AttackMode.Override : AttackMode.Normal;

            bool defenderHasShield = defenderTeam.RuntimeData.CurrentShield > 0;

            if (type.HasFlag(CombatConditionType.WithShield))
                return defenderHasShield ? AttackMode.ShieldOnly : AttackMode.Invalid;

            if (type.HasFlag(CombatConditionType.WithOutShield))
                return defenderHasShield ? AttackMode.Invalid : AttackMode.Normal;

            if (type.HasFlag(CombatConditionType.SelfHpBelow) && self.Health >= block.Value)
                return AttackMode.Invalid;

            if (type.HasFlag(CombatConditionType.HasEnemyAtLeast))
            {
                int alive = 0;
                if (defenderTeam.RuntimeData.CharacterTop?.IsAlive == true) alive++;
                if (defenderTeam.RuntimeData.CharacterBottom?.IsAlive == true) alive++;
                if (alive < block.Value) return AttackMode.Invalid;
            }

            if (type.HasFlag(CombatConditionType.HasAllyAtLeast))
            {
                int alive = 0;
                if (attackerTeam.RuntimeData.CharacterTop?.IsAlive == true) alive++;
                if (attackerTeam.RuntimeData.CharacterBottom?.IsAlive == true) alive++;
                if (alive < block.Value) return AttackMode.Invalid;
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
