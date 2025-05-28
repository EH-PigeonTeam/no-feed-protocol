using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.Systems.Save;
using UnityEngine.Audio;
using static PsychoGarden.Audio.SettingsApplier;
using PsychoGarden.Utils;

namespace PsychoGarden.Audio
{
    [HideMonoScript]
    public class AudioSourceResetListener : MonoBehaviour
    {
        [Tooltip("All AudioSources to be reset when the AudioResetEvent is triggered.")]
        [SerializeField, Required]
        private AudioSource[] m_audioSources;

        [Tooltip("AudioMixer to be reset when the AudioResetEvent is triggered.")]
        [SerializeField, Required]
        private AudioMixer m_audioMixer;

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

            ReapplyVolumeAfterReset();
        }

        private void ReapplyVolumeAfterReset()
        {
            const string key = "AudioSettingsData";
            if (!SaveSystem.Exists(key)) return;

            var data = SaveSystem.Load<AudioSaveData>(key);
            if (data == null) return;

            foreach (var volume in data.Volumes)
            {
                if (MixerParameterMap.TryGetValue(volume.Group, out var param))
                {
                    float db = AudioHelper.NormalizedToDecibels(volume.Volume);
                    m_audioMixer.SetFloat(param, db);
                }
            }

            foreach (var mute in data.Mutes)
            {
                if (MixerParameterMap.TryGetValue(mute.Group, out var param))
                {
                    float volume = data.Volumes.Find(v => v.Group == mute.Group)?.Volume ?? 1f;
                    float db = mute.Muted ? -80f : AudioHelper.NormalizedToDecibels(volume);
                    m_audioMixer.SetFloat(param, db);
                }
            }
        }
    }
}
