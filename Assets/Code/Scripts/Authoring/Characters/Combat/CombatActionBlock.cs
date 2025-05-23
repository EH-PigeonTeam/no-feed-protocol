using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
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
}
