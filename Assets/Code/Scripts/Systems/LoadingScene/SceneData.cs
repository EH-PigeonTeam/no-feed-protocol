using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace Code.Systems.LoadingScene
{
    [System.Serializable]
    public class SceneData
    {
        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The name of the scene")]
        [SerializeField]
        private string m_sceneName;

        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The scene to load")]
        [SerializeField, InlineProperty, HideLabel]
        private SceneReference m_scene;

        [FoldoutGroup("@m_sceneName")]
        [Tooltip("The load mode")]
        [SerializeField]
        private LoadSceneMode m_loadMode;

        public void Validate()
        {
#if UNITY_EDITOR
            m_scene.Validate();
#endif
        }

        public string Name => m_sceneName;
        public string SceneName => m_scene.SceneName;
        public LoadSceneMode LoadMode => m_loadMode;
    }
}