using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Events
{
    /// <summary>
    /// Represents a single answer or interaction choice in a narrative event.
    /// </summary>
    [Serializable]
    public class EventOption
    {
        [FoldoutGroup("Option")]
        [Tooltip("The text shown to the player for this option.")]
        [SerializeField]
        private string m_text;

        [FoldoutGroup("Option")]
        [Tooltip("The outcome of selecting this option.")]
        [SerializeField, InlineProperty, HideLabel]
        private EventConsequence[] m_consequence;

        [FoldoutGroup("Option")]
        [Tooltip("Optional item granted if this option succeeds.")]
        [SerializeField]
        private string m_itemID;

        public string Text => m_text;
        public EventConsequence[] Consequence => m_consequence;
        public string ItemReward => m_itemID;
    }
}
