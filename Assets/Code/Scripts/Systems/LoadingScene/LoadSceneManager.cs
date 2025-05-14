using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Code.Systems.Locator;
using Sirenix.OdinInspector;
using Code.Systems.LoadingScene.VFX;

namespace Code.Systems.LoadingScene
{
    [DefaultExecutionOrder(-90)]
    [HideMonoScript]
    public class LoadSceneManager : MonoBehaviour, ILoadSceneManager
    {
        [BoxGroup("Settings")]
        [Tooltip("Canvas shown during loading")]
        [SerializeField]
        private GameObject m_loadingCanvas;

        [BoxGroup("Settings")]
        [Tooltip("Fake loading time")]
        [SerializeField, Range(0f, 10f)]
        private float m_loadingTime;

        private CanvasGroupController m_canvasGroupController;

        private bool m_fadeIn = true;
        private bool m_fadeOut = true;
        private bool m_enableFakeTime = true;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (this.m_loadingCanvas != null)
            {
                this.m_canvasGroupController = this.m_loadingCanvas.GetComponentInChildren<CanvasGroupController>();
            }
        }

        private void OnEnable()
        {
            ServiceLocator.Register<ILoadSceneManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<ILoadSceneManager>();
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            this.StartCoroutine(this.LoadRoutine(sceneName, mode));
        }

        public void Fade(bool fadeIn, bool fadeOut)
        {
            this.m_fadeIn = fadeIn;
            this.m_fadeOut = fadeOut;
        }

        public void FakeLoadingTime(bool enable)
        {
            this.m_enableFakeTime = enable;
        }

        private IEnumerator LoadRoutine(string sceneName, LoadSceneMode mode)
        {
            if (this.m_loadingCanvas != null && this.m_fadeIn)
            {
                this.m_loadingCanvas.SetActive(true);
            }

            yield return null;

            // Unload all scenes except Bootstrap
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name != "Bootstrap" && scene.isLoaded)
                {
                    yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                }
            }

            AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                yield return null;
            }

            if (this.m_enableFakeTime)
            {
                yield return new WaitForSeconds(this.m_loadingTime); // Fake loading
            }

            if (this.m_fadeOut)
            {
                if (this.m_canvasGroupController != null)
                {
                    this.m_canvasGroupController.Hide();
                }

                yield return new WaitForSeconds(this.m_canvasGroupController.TransitionDuration);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            op.allowSceneActivation = true;

            // reset
            this.m_fadeIn = true;
            this.m_fadeOut = true;
            this.m_enableFakeTime = true;
        }
    }

    public interface ILoadSceneManager
    {
        void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive);
        void Fade(bool fadeIn, bool fadeOut);
        void FakeLoadingTime(bool enable);
    }
}
