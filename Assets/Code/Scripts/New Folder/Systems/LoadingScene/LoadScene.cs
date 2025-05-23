using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;

namespace Code.Systems.LoadingScene
{
    [HideMonoScript]
    public class LoadScene : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            ServiceLocator.Get<SceneManager>().LoadScene(sceneName);
        }

        public void LoadSceneByIndex(int index)
        {
            ServiceLocator.Get<SceneManager>().LoadScene(index);
        }
    }
}
