using System;
using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.Systems.Save;
using PsychoGarden.UI;
using PsychoGarden.Utils;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace PsychoGarden.Audio
{
    [HideMonoScript]
    public class SettingsApplier : MonoBehaviour
    {
        #region Fields --------------------------------------------

        [Header("Audio")]
        [SerializeField] private AudioMixer m_audioMixer;

        [Header("Graphics")]
        [SerializeField] private VolumeProfile m_graphicsVolume;

        [Header("Accessibility")]
        [SerializeField] private VolumeProfile m_accessibilityVolume;

        public static readonly Dictionary<AudioGroupType, string> MixerParameterMap = new()
        {
            { AudioGroupType.Master, "Master" },
            { AudioGroupType.Music, "Music" },
            { AudioGroupType.SFX, "SFX" }
        };

        #endregion

        #region Unity Callbacks -----------------------------------

        private void Start()
        {
            this.ApplyAudioSettings();
            this.ApplyGraphicsSettings();
            this.ApplyAccessibilitySettings();
        }

        #endregion

        #region Audio Settings -------------------------------------

        private void ApplyAudioSettings()
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
                    float db = mute.Muted ? -80f : AudioHelper.NormalizedToDecibels(
                        data.Volumes.Find(v => v.Group == mute.Group)?.Volume ?? 1f);
                    m_audioMixer.SetFloat(param, db);
                }
            }

            AudioConfiguration config = AudioSettings.GetConfiguration();
            config.speakerMode = data.AudioMode;
            AudioSettings.Reset(config);
        }

        [Serializable]
        public class AudioSaveData
        {
            public List<AudioVolumeData> Volumes = new();
            public List<AudioMuteData> Mutes = new();
            public AudioSpeakerMode AudioMode;
        }

        [Serializable]
        public class AudioVolumeData
        {
            public AudioGroupType Group;
            public float Volume;
        }

        [Serializable]
        public class AudioMuteData
        {
            public AudioGroupType Group;
            public bool Muted;
        }

        #endregion

        #region Graphics Settings ----------------------------------

        private void ApplyGraphicsSettings()
        {
            const string key = "GraphicsSettingsData";
            if (!SaveSystem.Exists(key)) return;

            var data = SaveSystem.Load<GraphicsSaveData>(key);
            if (data == null) return;

            var res = ResolutionHelper.GetResolutionByIndex(data.ResolutionIndex);
            Screen.SetResolution(res.width, res.height, data.ScreenMode);

            this.ApplyScreenMode(data.ScreenMode);
            QualitySettings.SetQualityLevel(data.QualityLevel);
            Application.targetFrameRate = data.FrameRate;

            if (m_graphicsVolume != null &&
                m_graphicsVolume.TryGet<ColorAdjustments>(out var ca))
            {
                ca.postExposure.value = Mathf.Lerp(-7f, 1f, data.Brightness);
            }
        }

        private void ApplyScreenMode(FullScreenMode mode)
        {
            Screen.fullScreenMode = mode;
            Screen.fullScreen = mode != FullScreenMode.Windowed;
        }

        [Serializable]
        private class GraphicsSaveData
        {
            public int ResolutionIndex;
            public FullScreenMode ScreenMode;
            public int QualityLevel;
            public int FrameRate;
            public float Brightness;
        }

        #endregion

        #region Accessibility Settings -----------------------------

        private void ApplyAccessibilitySettings()
        {
            const string key = "AccessibilitySettingsData";
            if (!SaveSystem.Exists(key)) return;

            var data = SaveSystem.Load<AccessibilitySaveData>(key);
            if (data == null) return;

            if (m_accessibilityVolume == null) return;

            if (!m_accessibilityVolume.TryGet<ChannelMixer>(out var mixer))
            {
                Debug.LogError("ChannelMixer not found in volume profile.");
                return;
            }

            var preset = ColorBlindPresets.GetPreset(data.Mode);

            mixer.redOutRedIn.value = preset.red.x;
            mixer.redOutGreenIn.value = preset.red.y;
            mixer.redOutBlueIn.value = preset.red.z;

            mixer.greenOutRedIn.value = preset.green.x;
            mixer.greenOutGreenIn.value = preset.green.y;
            mixer.greenOutBlueIn.value = preset.green.z;

            mixer.blueOutRedIn.value = preset.blue.x;
            mixer.blueOutGreenIn.value = preset.blue.y;
            mixer.blueOutBlueIn.value = preset.blue.z;
        }

        [Serializable]
        private class AccessibilitySaveData
        {
            public ColorBlindMode Mode;
        }

        #endregion
    }
}
