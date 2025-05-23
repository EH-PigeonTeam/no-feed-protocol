using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace Code.Systems.LoadingScene
{
    [System.Serializable]
    public class SceneData
    {
        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The name of the scene to load")]
        [SerializeField]
        private string m_sceneName;

        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The scene to load")]
        [SerializeField, InlineProperty, HideLabel]
        private SceneReference m_scene;

        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The load mode to use when loading the scene")]
        [SerializeField]
        private LoadSceneMode m_loadMode;

        public SceneData(string sceneName, LoadSceneMode loadMode)
        {
            m_sceneName = sceneName;
            m_loadMode = loadMode;
        }

        public string Name => m_sceneName;
        public string SceneName => m_scene?.SceneName ?? m_sceneName;
        public LoadSceneMode Mode => m_loadMode;

#if UNITY_EDITOR
        public void Validate()
        {
            m_scene.Validate();
        }
#endif
    }
}
