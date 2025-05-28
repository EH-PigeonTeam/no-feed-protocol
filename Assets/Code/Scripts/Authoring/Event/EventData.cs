using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Events
{
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
        private EventOption[] m_options;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => m_title;
        public Sprite Icon => m_icon;
        public string Text => m_text;
        public float Percent => m_Percent;
        public EventOption[] Options => m_options;

        #endregion
    }
}
