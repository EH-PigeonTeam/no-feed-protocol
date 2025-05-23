using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.UI;

namespace Code.Systems.Audio
{
    [System.Serializable]
    public class AudioClipData
    {
        [FoldoutGroup("@m_audioClipName")]
        [Tooltip("The name used to identify the audio clip")]
        [SerializeField]
        private string m_audioClipName;

        [FoldoutGroup("@m_audioClipName")]
        [Tooltip("The audio clip to play")]
        [SerializeField] 
        private AudioClip m_audioClip;

        [FoldoutGroup("@m_audioClipName")]
        [Tooltip("The Volume of the audio clip")]
        [SerializeField]
        private AudioGroupType m_audioGroup;

        [FoldoutGroup("@m_audioClipName")]
        [Tooltip("The Volume of the audio clip")]
        [SerializeField, Range(0, 1)]
        private float m_volume;

        public string AudioClipName => m_audioClipName;
        public AudioClip Audio => m_audioClip;
        public float Volume => m_volume;
        public AudioGroupType AudioGroup => m_audioGroup;
    }
}

