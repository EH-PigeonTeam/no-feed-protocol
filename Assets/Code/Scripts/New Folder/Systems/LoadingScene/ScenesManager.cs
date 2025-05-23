using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using System.Collections.Generic;

namespace Code.Systems.LoadingScene
{
    [DefaultExecutionOrder(-80)]
    [HideMonoScript]
    public class ScenesManager : MonoBehaviour
    {
        [BoxGroup("Scene Settings")]
        [Tooltip("The first scene to load")]
        [SerializeField]
        private string m_firstScene;

        [BoxGroup("Scene Settings")]
        [Tooltip("The scene metadata")]
        [SerializeField, InlineEditor, Required]
        private SceneResources m_sceneData;

        private void Start()
        {
            ServiceLocator.Get<LoadSceneManager>().Fade(false, true);
            ServiceLocator.Get<LoadSceneManager>().FakeLoadingTime(false);
            LoadScene(m_firstScene);
        }

        private void OnEnable()
        {
            ServiceLocator.Register<ScenesManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<ScenesManager>();
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
            ChangeScene(new SceneData(sceneName, mode));
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

            ServiceLocator.Get<LoadSceneManager>().LoadScenes(sceneDatas);
        }

        #endregion

        #region Internal Methods -------------------------------------------------------

        internal void ChangeScene(SceneData sceneData)
        {
            var loadSceneManager = ServiceLocator.Get<LoadSceneManager>();

            loadSceneManager.LoadScene(sceneData.SceneName, sceneData.Mode);
        }

        #endregion
    }
}
