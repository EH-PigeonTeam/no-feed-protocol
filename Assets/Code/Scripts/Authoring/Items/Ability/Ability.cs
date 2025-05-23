using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    [Serializable]
    public class Ability
    {
        [FoldoutGroup("Ability")]
        [Tooltip("The trigger moment of the ability")]
        [SerializeField]
        private AbilityTriggerMoment m_triggerMoment;

        [FoldoutGroup("Ability")]
        [Tooltip("The condition of the ability")]
        [SerializeField, InlineProperty, HideLabel]
        private AbilityCondition m_Condition;

        [FoldoutGroup("Ability")]
        [Tooltip("The effects of the ability")]
        [SerializeField]
        private List<AbilityEffect> m_effects;

        public AbilityTriggerMoment TriggerMoment => m_triggerMoment;
        public AbilityCondition Condition => m_Condition;
        public List<AbilityEffect> Effects => m_effects;
    }
}
