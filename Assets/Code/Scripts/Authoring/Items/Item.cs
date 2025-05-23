using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Items.Abilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NoFeedProtocol.Authoring.Items
{
    [Serializable]
    public class Item : ISerializationCallbackReceiver
    {
        #region Unique ID ---------------------------------------------------

        [FoldoutGroup("@m_name")]
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

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { }

        #endregion

        #region Basic Info --------------------------------------------------

        [FoldoutGroup("@m_name")]
        [Tooltip("The name of the item")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The icon of the item")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("The description of the item")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, Range(0, 1)]
        private float m_Percent = 1f;

        // Logic
        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, InlineProperty, HideLabel]
        private Ability ability;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public float Percent => m_Percent;
        public Ability GetAbility => ability;

        #endregion
    }
}
