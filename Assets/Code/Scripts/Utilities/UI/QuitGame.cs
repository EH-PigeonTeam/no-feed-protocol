using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
