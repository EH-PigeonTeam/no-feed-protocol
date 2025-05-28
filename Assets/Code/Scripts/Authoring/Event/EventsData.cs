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
}
