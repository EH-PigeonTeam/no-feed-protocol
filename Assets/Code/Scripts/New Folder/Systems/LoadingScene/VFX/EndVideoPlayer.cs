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
        [BoxGroup("Settings")]
        [Tooltip("Scene to load after the video has finished playing.")]
        [SerializeField]
        private string sceneName;

        [Button("Load Scene")]
        private void LoadScene() => ServiceLocator.Get<ScenesManager>().LoadScene(sceneName);

        private VideoPlayer videoPlayer;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            ServiceLocator.Get<ScenesManager>().LoadScene(sceneName);
        }
    }
}
