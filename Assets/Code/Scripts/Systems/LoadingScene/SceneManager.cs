using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using UnityEngine.SceneManagement;

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
        [Tooltip("The audio clips to play")]
        [SerializeField, InlineEditor, Required]
        private SceneResources m_sceneData;

        private void Start()
        {
            ServiceLocator.Get<ILoadSceneManager>().Fade(false, true);
            ServiceLocator.Get<ILoadSceneManager>().FakeLoadingTime(false);

            LoadScene(this.m_firstScene);
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

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            ChangeScene(sceneName, mode);
        }

        public void LoadScene(string sceneName)
        {
            SceneData sceneData = this.m_sceneData.GetScene(sceneName);

            if (sceneData == null)
            {
                Debug.LogErrorFormat("Scene {0} not found", sceneName);
                return;
            }

            ChangeScene(sceneData.SceneName, sceneData.LoadMode);
        }

        public void LoadScene(int index)
        {
            SceneData sceneData = this.m_sceneData.GetScene(index);

            if (sceneData == null)
            {
                Debug.LogErrorFormat("Scene {0} not found", index);
                return;
            }

            ChangeScene(sceneData.SceneName, sceneData.LoadMode);
        }

        #endregion

        #region Internal Methods -------------------------------------------------------

        internal void ChangeScene(string name, LoadSceneMode mode)
        {
            if (name != null && name.Length > 0)
            {
                ServiceLocator.Get<ILoadSceneManager>().LoadScene(name, mode);
            }
        }

        #endregion

    }
}