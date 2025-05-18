using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using System.Collections.Generic;

namespace Code.Systems.LoadingScene
{
    [DefaultExecutionOrder(-80)]
    [HideMonoScript]
    public class SceneManager : MonoBehaviour
    {
        [BoxGroup("Scene Settings")]
        [Tooltip("The first scene to load")]
        [SerializeField]
        private string m_firstScene;

        [BoxGroup("Scene Settings")]
        [Tooltip("The scene metadata")]
        [SerializeField, InlineEditor, Required]
        private SceneResources m_sceneData;

        [ShowInInspector, ReadOnly]
        private readonly HashSet<string> m_persistentScenes = new();
        private const string CoreSystemsScene = "CoreSystems";

        private void Start()
        {
            ServiceLocator.Get<ILoadSceneManager>().Fade(false, true);
            ServiceLocator.Get<ILoadSceneManager>().FakeLoadingTime(false);
            LoadScene(m_firstScene);
        }

        private void OnEnable()
        {
            ServiceLocator.Register<SceneManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<SceneManager>();
        }

        #region Public Methods ---------------------------------------------------------

        public void LoadScene(string sceneName)
        {
            SceneData sceneData = m_sceneData.GetScene(sceneName);
            if (sceneData == null)
            {
                Debug.LogError($"Scene '{sceneName}' not found in SceneResources.");
                return;
            }

            ChangeScene(sceneData);
        }

        public void LoadScene(int index)
        {
            SceneData sceneData = m_sceneData.GetScene(index);
            if (sceneData == null)
            {
                Debug.LogError($"Scene index '{index}' not found in SceneResources.");
                return;
            }

            ChangeScene(sceneData);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode)
        {
            ChangeScene(new SceneData(sceneName, mode, false));
        }

        public void LoadScenes(string[] sceneNames)
        {
            List<SceneData> sceneDatas = new();

            foreach (string name in sceneNames)
            {
                var data = m_sceneData.GetScene(name);
                if (data == null)
                {
                    Debug.LogError($"Scene '{name}' not found in SceneResources.");
                    return;
                }

                sceneDatas.Add(data);
            }

            SceneData mainScene = sceneDatas.Find(s => s.LoadMode == LoadSceneMode.Single);

            if (mainScene != null)
            {
                m_persistentScenes.Clear();
                m_persistentScenes.Add(CoreSystemsScene);
            }

            foreach (var scene in sceneDatas)
            {
                if (scene.IsPersistent)
                {
                    m_persistentScenes.Add(scene.SceneName);
                    m_persistentScenes.Add(CoreSystemsScene);
                }
            }

            ServiceLocator.Get<ILoadSceneManager>()
                .LoadScenes(sceneDatas, mainScene, m_persistentScenes);
        }

        #endregion

        #region Internal Methods -------------------------------------------------------

        internal void ChangeScene(SceneData sceneData)
        {
            if (sceneData.LoadMode == LoadSceneMode.Single)
            {
                m_persistentScenes.Clear();

                ServiceLocator.Get<ILoadSceneManager>()
                    .LoadScene(sceneData.SceneName, LoadSceneMode.Single, new[] { CoreSystemsScene });

                m_persistentScenes.Add(sceneData.Name);
            }
            else
            {
                ServiceLocator.Get<ILoadSceneManager>()
                    .LoadScene(sceneData.SceneName, LoadSceneMode.Additive, m_persistentScenes);
            }
        }

        #endregion
    }
}
