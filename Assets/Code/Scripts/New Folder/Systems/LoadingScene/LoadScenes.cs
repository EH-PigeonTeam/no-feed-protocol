using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;

namespace Code.Systems.LoadingScene
{
    [HideMonoScript]
    public class LoadScenes : MonoBehaviour
    {
        [BoxGroup("Scenes")]
        [Tooltip("Names of scenes to load.")]
        [SerializeField]
        private string[] m_sceneNames;

        public void Interact()
        {
            LoadScenesByName(this.m_sceneNames);
        }

        public void LoadScenesByName(string[] sceneNames)
        {
            ServiceLocator.Get<SceneManager>().LoadScenes(sceneNames);
        }
    }
}
