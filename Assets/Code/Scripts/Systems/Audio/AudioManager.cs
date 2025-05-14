using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using PsychoGarden.UI;

namespace Code.Systems.Audio
{
    [HideMonoScript]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [BoxGroup("Audio Settings")]
        [Tooltip("The audio clips to play")]
        [SerializeField]
        private string m_firstMusicClip;

        [BoxGroup("Audio Settings")]
        [Tooltip("The audio clips to play")]
        [SerializeField, InlineEditor]
        private AudioResources m_audioData;

        [BoxGroup("Audio Settings/Audio Source")]
        [Tooltip("The audio source to play music through")]
        [SerializeField, Required]
        private AudioSource m_audioSourceMusic;

        [BoxGroup("Audio Settings/Audio Source")]
        [Tooltip("The audio source to play sound effects through")]
        [SerializeField, Required]
        private AudioSource m_audioSourceSFX;

        private void Awake()
        {
            this.m_audioSourceMusic.playOnAwake = true;
            this.m_audioSourceMusic.loop = true;

            PlayAudioClip(this.m_firstMusicClip);

            this.m_audioSourceSFX.playOnAwake = false;
            this.m_audioSourceSFX.loop = false;
        }

        private void OnEnable()
        {
            ServiceLocator.Register<AudioManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<AudioManager>();
        }

        #region Public Methods ---------------------------------------------------------

        /// <summary>
        /// Plays the specified AudioClip through the AudioSource, with an optional volume override.
        /// </summary>
        /// <param name="audioClip">The AudioClip to play.</param>
        /// <param name="volume">The volume at which to play the clip (default is 1f).</param>
        /// <param name="type">The AudioGroupType to play the clip through (default is AudioGroupType.Music)</param>
        public void PlayAudioClip(AudioClip audioClip, float volume = 1f, AudioGroupType audioGroup = AudioGroupType.Music)
        {
            PlaySound(audioClip, volume, audioGroup);
        }

        /// <summary>
        /// Plays an AudioClip by its name, as defined in the AudioData source.
        /// </summary>
        /// <param name="audioClipName">The name identifier of the AudioClip.</param>
        public void PlayAudioClip(string audioClipName)
        {
            AudioClipData audioClip = this.m_audioData.GetAudioClip(audioClipName);
            
            if (audioClip == null)
            {
                Debug.LogWarning($"AudioManager: Could not find audio clip with name {audioClipName}");
                return;
            }

            PlaySound(audioClip.Audio, audioClip.Volume, audioClip.AudioGroup);
        }

        /// <summary>
        /// Plays an AudioClip by its index, as defined in the AudioData source.
        /// </summary>
        /// <param name="index">The index of the AudioClip in the AudioData collection.</param>
        public void PlayAudioClip(int index)
        {
            AudioClipData audioClip = this.m_audioData.GetAudioClip(index);

            if (audioClip == null)
            {
                Debug.LogWarning($"AudioManager: Could not find audio clip with name {index}");
                return;
            }

            PlaySound(audioClip.Audio, audioClip.Volume, audioClip.AudioGroup);
        }

        #endregion

        #region Internal Methods -------------------------------------------------------

        internal void PlaySound(AudioClip audioClip, float volume, AudioGroupType audioGroup)
        {
            if (audioClip != null)
            {
                AudioSource audioSource = (audioGroup == AudioGroupType.Music) ? this.m_audioSourceMusic : this.m_audioSourceSFX;
                audioSource.SetScheduledEndTime(0); // Stop playback immediately
                audioSource.volume = volume;
                audioSource.clip = audioClip;
                audioSource.PlayScheduled(AudioSettings.dspTime);
            }
        }

        #endregion

    }
}

