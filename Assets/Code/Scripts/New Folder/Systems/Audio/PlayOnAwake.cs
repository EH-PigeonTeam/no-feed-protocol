using Code.Systems.Locator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Systems.Audio
{
    [HideMonoScript]
    public class PlayOnAwake : MonoBehaviour
    {
        [BoxGroup("Audio")]
        [Tooltip("The audio clip to play")]
        [SerializeField]
        private string m_audioClip = null;

        private void Awake()
        {
            ServiceLocator.Get<AudioManager>().PlayAudioClip(m_audioClip);
        }
    }
}
