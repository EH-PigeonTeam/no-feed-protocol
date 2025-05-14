using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Code.Systems.Audio
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "AudioResources", menuName = "No Feed Protocol/AudioResources")]
    public class AudioResources : ScriptableObject
    {
        [Tooltip("The audio clips to play")]
        [SerializeField]
        private AudioClipData[] m_audioClips;

        public AudioClipData[] AudioClips => m_audioClips;
        public AudioClipData GetAudioClip(int index) => m_audioClips[index];
        public AudioClipData GetAudioClip(string name) => m_audioClips.FirstOrDefault(x => x.AudioClipName == name);
    }
}

