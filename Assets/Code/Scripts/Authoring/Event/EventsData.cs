using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace NoFeedProtocol.Authoring.Events
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "EventsData", menuName = "No Feed Protocol/EventsData")]
    public class EventsData : ScriptableObject
    {
        [BoxGroup("Events")]
        [Tooltip("Events in the game")]
        [SerializeField]
        private EventData[] m_events;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var eventData in m_events)
                eventData.OnValidate();
        }
#endif

        public EventData[] Events => m_events;

        public EventData GetEvent(int index) => m_events[index];

        /// <summary>
        /// Finds an event by its unique ID.
        /// </summary>
        public EventData GetById(string id)
        {
            foreach (var eventData in m_events)
            {
                if (eventData.Id == id)
                    return eventData;
            }

            Debug.LogWarning($"Event ID '{id}' not found.");
            return null;
        }
    }

    [Serializable]
    public class EventData
    {
        #region Unique ID ---------------------------------------------------

        [FoldoutGroup("@m_title")]
        [SerializeField, ReadOnly]
        private string m_id;

        /// <summary>
        /// Unique, non-editable ID used for lookup and save data.
        /// </summary>
        public string Id => m_id;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(m_id))
            {
                m_id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(Selection.activeObject);
            }
        }
#endif

        #endregion

        #region Basic Info --------------------------------------------------

        [FoldoutGroup("@m_title")]
        [Tooltip("The name of the item")]
        [SerializeField]
        private string m_title;

        [FoldoutGroup("@m_title")]
        [Tooltip("The icon of the item")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_title")]
        [Tooltip("The text of the item")]
        [SerializeField, TextArea(4, 10)]
        private string m_text;

        [FoldoutGroup("@m_title")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, Range(0, 1)]
        private float m_Percent = 1f;

        [FoldoutGroup("@m_title")]
        [Tooltip("The chooses of the item")]
        [SerializeField]
        private EventOption[] m_conditions;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => m_title;
        public Sprite Icon => m_icon;
        public string Text => m_text;
        public float Percent => m_Percent;
        public EventOption[] Options => m_conditions;

        #endregion
    }

    /// <summary>
    /// Type of result produced by an event option.
    /// </summary>
    public enum EventOutcomeType
    {
        None,
        Health,
        Shield,
        Money
    }

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
        private EventConsequence m_consequence;

        [FoldoutGroup("Option")]
        [Tooltip("Optional item granted if this option succeeds.")]
        [SerializeField]
        private string m_itemID;

        public string Text => m_text;
        public EventConsequence Consequence => m_consequence;
        public string ItemReward => m_itemID;
    }
}
