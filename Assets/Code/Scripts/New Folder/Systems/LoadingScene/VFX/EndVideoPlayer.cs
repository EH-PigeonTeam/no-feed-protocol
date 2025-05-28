using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;
using Code.Systems.Locator;
using Code.Systems.LoadingScene;

namespace Code.Scripts.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    public class EndVideoPlayer : MonoBehaviour
    {
        #region Fields --------------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("Scene to load after the video has finished playing.")]
        [SerializeField]
        private string sceneName;

        [Tooltip("Failsafe timeout in seconds in case the video gets stuck.")]
        [SerializeField]
        private float maxVideoDuration = 30f;

        private VideoPlayer m_videoPlayer;
        private double m_videoStartTime;
        private bool m_hasEnded;

        #endregion

        #region Unity Callbacks -----------------------------------

        private void Awake()
        {
            m_videoPlayer = GetComponent<VideoPlayer>();
            m_videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void Start()
        {
            m_videoStartTime = Time.timeAsDouble;
        }

        private void Update()
        {
            if (m_hasEnded) return;

            // Failsafe #1: Check if video finished
            if (m_videoPlayer.isPrepared && m_videoPlayer.frame > 0 &&
                m_videoPlayer.frame >= (long)m_videoPlayer.frameCount - 2)
            {
                OnVideoEnd(m_videoPlayer);
            }

            // Failsafe #2: Timeout after N seconds
            if (Time.timeAsDouble - m_videoStartTime > maxVideoDuration)
            {
                Debug.LogWarning("Video timeout reached. Forcing scene load.");
                OnVideoEnd(m_videoPlayer);
            }
        }

        private void OnDestroy()
        {
            m_videoPlayer.loopPointReached -= OnVideoEnd;
        }

        #endregion

        #region Logic ---------------------------------------------

        private void OnVideoEnd(VideoPlayer vp)
        {
            if (m_hasEnded) return;

            m_hasEnded = true;
            Debug.Log("Video ended, loading scene: " + sceneName);
            ServiceLocator.Get<ScenesManager>().LoadScene(sceneName);
        }

        [Button("Force Load Scene")]
        private void LoadScene() => ServiceLocator.Get<ScenesManager>().LoadScene(sceneName);

        #endregion
    }
}
