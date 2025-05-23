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
            ServiceLocator.Get<ScenesManager>().LoadScene(sceneName);
        }

        public void LoadSceneByIndex(int index)
        {
            ServiceLocator.Get<ScenesManager>().LoadScene(index);
        }
    }
}
