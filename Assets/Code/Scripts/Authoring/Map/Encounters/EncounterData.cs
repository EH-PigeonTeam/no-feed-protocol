using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Map
{
    [Serializable]
    public class EncounterData
    {
        #region Encounter Data ---------------------------------------------

        [FoldoutGroup("$m_type")]
        [SerializeField, ReadOnly]
        private string m_id;

        [FoldoutGroup("$m_type")]
        [Tooltip("Type of encounter.")]
        [SerializeField]
        private EncounterType m_type;

        [FoldoutGroup("$m_type")]
        [Tooltip("Scene to load when this encounter starts.")]
        [SerializeField]
        private string m_sceneName;

        [FoldoutGroup("$m_type")]
        [Tooltip("Scene to load when this encounter starts.")]
        [SerializeField, Range(0, 1)]
        private float m_percentage = 1f;

        #endregion

        #region Visual ------------------------------------------------------

        [FoldoutGroup("$m_type/Visual")]
        [Tooltip("Icon used in the map UI.")]
        [SerializeField, PreviewField(80)]
        private Sprite m_icon;

        #endregion

        #region OnValidate --------------------------------------------------

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(m_id))
            {
                m_id = Guid.NewGuid().ToString();
            }
        }
#endif

        #endregion

        #region Public Properties -------------------------------------------

        public string Id => m_id;
        public string SceneName => m_sceneName;
        public EncounterType Type => m_type;
        public float Percentage => m_percentage;
        public Sprite Icon => m_icon;

        #endregion
    }
}
