using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using Code.Systems.LoadingScene.VFX;
using System.Linq;

namespace Code.Systems.LoadingScene
{
    [DefaultExecutionOrder(-90)]
    [HideMonoScript]
    public class LoadSceneManager : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Canvas shown during loading")]
        [SerializeField, SceneObjectsOnly]
        private ImageFadeController m_transitionController;

        [BoxGroup("Settings")]
        [Tooltip("Fake loading time")]
        [SerializeField, Range(0f, 10f)]
        private float m_loadingTime;

        [ShowInInspector, ReadOnly]
        private readonly HashSet<string> m_preservedScenes = new();

        private const string CoreSystemsScene = "CoreSystems";

        private bool m_fadeIn = true;
        private bool m_fadeOut = true;
        private bool m_enableFakeTime = true;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            ServiceLocator.Register<LoadSceneManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<LoadSceneManager>();
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            LoadScenes(new[] { new SceneData(sceneName, mode) });
        }

        public void LoadScenes(IEnumerable<SceneData> scenes)
        {
            StartCoroutine(LoadScenesRoutine(scenes));
        }

        private IEnumerator LoadScenesRoutine(IEnumerable<SceneData> scenes)
        {
            List<SceneData> toLoad = new(scenes);
            bool hasSingle = toLoad.Any(s => s.Mode == LoadSceneMode.Single);

            // Build the set of scenes to preserve during unloading
            HashSet<string> preserveSet = new();
            preserveSet.Add(CoreSystemsScene); // CoreSystems is always preserved

            if (hasSingle)
            {
                // If there's any Single scene, unload everything except CoreSystems

                // Reset the internal preserved scenes set
                m_preservedScenes.Clear();
                m_preservedScenes.Add(CoreSystemsScene);

                // Add the Single scene to preserved scenes
                var singleScene = toLoad.First(s => s.Mode == LoadSceneMode.Single);
                m_preservedScenes.Add(singleScene.SceneName);
            }
            else
            {
                // If all scenes are Additive, preserve the currently tracked persistent scenes
                foreach (var scene in m_preservedScenes)
                    preserveSet.Add(scene);
            }

            // Play fade-in transition before unloading
            if (m_transitionController != null && m_fadeIn)
            {
                bool done = false;
                m_transitionController.PlayEntryTransition(() => done = true);
                yield return new WaitUntil(() => done);
            }

            // Unload all loaded scenes not in the preserve set
            yield return UnloadScenesRoutine(preserveSet);

            // Load each scene in Additive mode (even if originally requested as Single)
            foreach (var sceneData in toLoad)
            {
                var async = SceneManager.LoadSceneAsync(sceneData.SceneName, LoadSceneMode.Additive);
                async.allowSceneActivation = false;

                // Wait until scene is almost fully loaded
                while (async.progress < 0.9f)
                    yield return null;

                async.allowSceneActivation = true;
            }

            // Optional fake wait after loading
            if (m_enableFakeTime)
                yield return new WaitForSeconds(m_loadingTime);

            // Play fade-out transition
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

            // Reset internal flags for next transition
            m_fadeIn = true;
            m_fadeOut = true;
            m_enableFakeTime = true;
        }

        private IEnumerator UnloadScenesRoutine(HashSet<string> preserveSet)
        {
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneAt(i);

                Debug.Log($"<color=yellow>Checking</color> scene: {scene.name}");

                if (scene.isLoaded && !preserveSet.Contains(scene.name))
                {
                    Debug.Log($"[Unload] Unloading scene: {scene.name}");
                    yield return SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        #region VFX ---------------------------------------------------------

        public void Fade(bool fadeIn, bool fadeOut)
        {
            this.m_fadeIn = fadeIn;
            this.m_fadeOut = fadeOut;
        }

        public void FakeLoadingTime(bool enable)
        {
            this.m_enableFakeTime = enable;
        }

        #endregion

    }
}
