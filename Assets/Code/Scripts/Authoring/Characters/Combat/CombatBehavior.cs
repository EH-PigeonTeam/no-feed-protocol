using System;
using UnityEngine;

namespace NoFeedProtocol.Authoring.Characters.Combat
{
    [Serializable]
    public class CombatBehavior
    {
        [SerializeField]
        private CombatTriggerBlock[] m_triggers;

        public CombatTriggerBlock[] Triggers => this.m_triggers;
    }
}
