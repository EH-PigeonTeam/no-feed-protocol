using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Events
{
    [Serializable]
    public class EventConsequence
    {
        [FoldoutGroup("$m_outcome")]
        [SerializeField]
        private EventOutcomeType m_outcome;

        [FoldoutGroup("$m_outcome")]
        [SerializeField, HideIf("m_outcome", EventOutcomeType.None)]
        private int m_value;

        public EventOutcomeType Outcome => m_outcome;
        public int Value => m_value;
    }
}
