using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
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
}
