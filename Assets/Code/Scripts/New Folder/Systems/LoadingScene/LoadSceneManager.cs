using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
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

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, IEnumerable<string> preserveScenes = null)
        {
            StartCoroutine(LoadRoutine(sceneName, mode, preserveScenes));
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

        private IEnumerator LoadRoutine(string sceneName, LoadSceneMode mode, IEnumerable<string> preserveScenes)
        {
            var preserveSet = new HashSet<string>(preserveScenes ?? new[] { "CoreSystems" });

            if (this.m_transitionController != null && this.m_fadeIn)
            {
                bool done = false;
                this.m_transitionController.PlayEntryTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.isLoaded && !preserveSet.Contains(scene.name))
                {
                    yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                }
            }

            AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                yield return null;
            }

            if (this.m_enableFakeTime)
            {
                yield return new WaitForSeconds(this.m_loadingTime);
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

            this.m_fadeIn = true;
            this.m_fadeOut = true;
            this.m_enableFakeTime = true;
        }

        public void LoadScenes(IEnumerable<SceneData> scenes, SceneData mainScene = null, IEnumerable<string> preserveScenes = null)
        {
            StartCoroutine(LoadMultipleRoutine(scenes, mainScene, preserveScenes));
        }

        private IEnumerator LoadMultipleRoutine(IEnumerable<SceneData> scenes, SceneData mainScene, IEnumerable<string> preserveScenes)
        {
            var preserveSet = new HashSet<string>(preserveScenes ?? new[] { "CoreSystems" });

            if (m_transitionController != null && m_fadeIn)
            {
                bool done = false;
                m_transitionController.PlayEntryTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }

            // Unload if mainScene is defined
            if (mainScene != null)
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    if (scene.isLoaded && !preserveSet.Contains(scene.name))
                    {
                        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            List<SceneData> toLoad = new(scenes);

            if (mainScene != null && !toLoad.Contains(mainScene))
            {
                toLoad.Insert(0, mainScene); // ensure main scene is first if not present
            }

            foreach (var sceneData in toLoad)
            {
                var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneData.SceneName, LoadSceneMode.Additive);
                async.allowSceneActivation = false;

                while (async.progress < 0.9f)
                {
                    yield return null;
                }

                if (m_enableFakeTime)
                {
                    yield return new WaitForSeconds(m_loadingTime);
                }

                async.allowSceneActivation = true;
            }

            if (m_fadeOut && m_transitionController != null)
            {
                bool done = false;
                m_transitionController.PlayExitTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            m_fadeIn = true;
            m_fadeOut = true;
            m_enableFakeTime = true;
        }

    }

    public interface ILoadSceneManager
    {
        void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, IEnumerable<string> preserveScenes = null);
        void LoadScenes(IEnumerable<SceneData> scenes, SceneData mainScene = null, IEnumerable<string> preserveScenes = null);

        void Fade(bool fadeIn, bool fadeOut);
        void FakeLoadingTime(bool enable);
    }
}
