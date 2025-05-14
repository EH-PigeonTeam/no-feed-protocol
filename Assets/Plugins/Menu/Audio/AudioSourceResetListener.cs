using UnityEngine;
using Sirenix.OdinInspector;

namespace PsychoGarden.Audio
{
    [HideMonoScript]
    public class AudioSourceResetListener : MonoBehaviour
    {
        [Tooltip("All AudioSources to be reset when the AudioResetEvent is triggered.")]
        [SerializeField, Required]
        private AudioSource[] m_audioSources;

        private void Awake()
        {
            this.m_audioSources = GetComponentsInChildren<AudioSource>();
        }

        private void OnEnable()
        {
            AudioResetEvent.OnAudioReset += HandleAudioReset;
        }

        private void OnDisable()
        {
            AudioResetEvent.OnAudioReset -= HandleAudioReset;
        }

        private void HandleAudioReset()
        {
            Debug.Log("Audio Reset");

            foreach (var source in this.m_audioSources)
            {
                if (source != null)
                {
                    bool wasEnabled = source.enabled;
                    source.enabled = false;
                    source.enabled = wasEnabled;
                }
            }
        }

    }
}
