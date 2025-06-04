using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class ToggleActivator : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField]
        private GameObject m_ui;

        public void Activate()
        {
            m_ui.SetActive(!m_ui.activeSelf);
        }
    }
}
