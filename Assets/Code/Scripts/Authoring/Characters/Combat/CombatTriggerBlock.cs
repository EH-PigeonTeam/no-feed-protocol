using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
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
}
