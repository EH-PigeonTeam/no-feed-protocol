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
        [SerializeField, SceneObjectsOnly]
        private ImageFadeController m_transitionController;

        [BoxGroup("Settings")]
        [Tooltip("Fake loading time")]
        [SerializeField, Range(0f, 10f)]
        private float m_loadingTime;

        private bool m_fadeIn = true;
        private bool m_fadeOut = true;
        private bool m_enableFakeTime = true;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
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
            if (this.m_transitionController != null && this.m_fadeIn)
            {
                bool done = false;
                this.m_transitionController.PlayEntryTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }

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
                Debug.Log("Fake loading time");
                yield return new WaitForSeconds(this.m_loadingTime); // Fake loading
                Debug.Log("Fake loading time done");
            }

            op.allowSceneActivation = true;

            if (this.m_fadeOut && this.m_transitionController != null)
            {
                bool done = false;
                this.m_transitionController.PlayExitTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            // Reset
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
