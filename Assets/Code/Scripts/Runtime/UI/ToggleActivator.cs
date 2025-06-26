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

        [BoxGroup("References")]
        [SerializeField]
        private GameObject m_mainMenu;

        [BoxGroup("References")]
        [SerializeField]
        private GameObject[] m_otherScreensMenu;

        public void Activate()
        {
            m_ui.SetActive(!m_ui.activeSelf);

            m_mainMenu.SetActive(m_ui.activeSelf);
            foreach (GameObject screen in m_otherScreensMenu)
            {
                screen.SetActive(false);
            }
        }
    }
}
