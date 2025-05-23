using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Code.Systems.LoadingScene
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "SceneResources", menuName = "No Feed Protocol/SceneResources")]
    public class SceneResources : ScriptableObject
    {
        [Tooltip("The audio clips to play")]
        [SerializeField]
        private SceneData[] m_scenesData;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var sceneData in m_scenesData)
            {
                sceneData.Validate();
            }
        }
#endif

        public SceneData[] ScenesData => m_scenesData;
        public SceneData GetScene(int index) => m_scenesData[index];
        public SceneData GetScene(string name) => m_scenesData.FirstOrDefault(x => x.Name == name);
    }
}