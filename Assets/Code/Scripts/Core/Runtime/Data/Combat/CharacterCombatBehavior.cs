using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Data.Combat
{
    [Serializable]
    public class CharacterCombatBehavior
    {
        [SerializeField]
        private CombatTriggerBlock[] m_triggers;

        public CombatTriggerBlock[] Triggers => this.m_triggers;
    }

    [Serializable]
    public class CombatTriggerBlock
    {
        [FoldoutGroup("$m_trigger")]
        [Tooltip("When this block activates")]
        [SerializeField]
        private CombatTriggerType m_trigger;

        [FoldoutGroup("$m_trigger")]
        [Tooltip("List of conditional action blocks")]
        [SerializeField]
        private CombatActionBlock[] m_actions;

        public CombatTriggerType Trigger => this.m_trigger;

        public CombatActionBlock[] Actions => this.m_actions;
    }

    [Serializable]
    public class CombatActionBlock
    {
        [FoldoutGroup("@GetName()")]
        [ValidateInput("IsShieldConditionValid", "WithShield and WithOutShield cannot be enabled at the same time.")]
        [Tooltip("Conditions to activate this action block")]
        [SerializeField]
        private CombatConditionType m_conditions;

        [FoldoutGroup("@GetName()")]
        [Tooltip("Conditions to activate this action block")]
        [SerializeField, ShowIf("m_conditions", CombatConditionType.SelfHpBelow)]
        private int m_value;

        [FoldoutGroup("@GetName()")]
        [Tooltip("Ordered list of actions to execute")]
        [SerializeField]
        private CombatAction[] m_sequence;

        public CombatConditionType Conditions => this.m_conditions;
        public int Value => this.m_value;
        public CombatAction[] Sequence => this.m_sequence;

#if UNITY_EDITOR
        private string GetName()
        {
            return "Execute " + this.m_conditions.ToString();
        }

        private bool IsShieldConditionValid(CombatConditionType value)
        {
            bool hasWith = (value & CombatConditionType.WithShield) != 0;
            bool hasWithout = (value & CombatConditionType.WithOutShield) != 0;

            return !(hasWith && hasWithout);
        }
#endif
    }

    [Serializable]
    public class CombatAction
    {
        [FoldoutGroup("@GetName()")]
        [Tooltip("Type of the action to execute")]
        [SerializeField]
        private CombatActionType m_type;

        [FoldoutGroup("@GetName()")]
        [Tooltip("Target affected by the action")]
        [SerializeField]
        private CombatTargetType m_target;

        [HorizontalGroup("@GetName()/value", width: 16f)]
        [Tooltip("Override value for this action (optional)")]
        [SerializeField, HideLabel]
        private bool m_overrideValue;

        [HorizontalGroup("@GetName()/value"), EnableIf("m_overrideValue")]
        [Tooltip("Custom value if override is enabled")]
        [SerializeField, LabelWidth(-24)]
        private int m_value;

        [FoldoutGroup("@GetName()")]
        [Tooltip("Damage/heal multiplier (default = 1.0)")]
        [SerializeField]
        private float m_modifier = 1f;

        public CombatActionType Type => this.m_type;
        public CombatTargetType Target => this.m_target;
        public bool OverrideValue => this.m_overrideValue;
        public int Value => this.m_value;
        public float Modifier => this.m_modifier;

#if UNITY_EDITOR
        private int GetValueNormalized()
        {
            return (int)(this.m_value * this.m_modifier);
        }

        private string GetName()
        {
            return this.m_type + " | " + this.m_target + (this.m_overrideValue ? $" | {GetValueNormalized()}" : "");
        }
#endif
    }

    public enum CombatTriggerType
    {
        OnAttackReady,
        OnHit,
        OnTurnStart,
        OnTurnEnd,
        OnDeath
    }

    [Flags]
    public enum CombatConditionType
    {
        Always = 0,
        WithShield = 1 << 0,
        WithOutShield = 1 << 1,
        HasEnemyAtLeast = 1 << 2,
        HasAllyAtLeast = 1 << 3,
        SelfHpBelow = 1 << 4
    }

    public enum CombatActionType
    {
        Damage,
        Heal,
        RegenShield
    }

    public enum CombatTargetType
    {
        Self,
        EnemyTargeted,
        EnemyOther,
        EnemyAll,
        EnemyAttacker,
        AllyLowestHP
    }
}
