using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;

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

        [BoxGroup("Settings")]
        [SerializeField]
        private TriggerEvent m_OnActive;

        [BoxGroup("Settings")]
        [SerializeField]
        private TriggerEvent m_OnInactive;

        public void Activate()
        {
            if (!m_ui.activeSelf)
            {
                m_OnActive?.Invoke(this.transform);
            }
            else
            {
                m_OnInactive?.Invoke(this.transform);
            }

            //m_ui.SetActive(!m_ui.activeSelf);

            m_mainMenu.SetActive(m_ui.activeSelf);
            foreach (GameObject screen in m_otherScreensMenu)
            {
                screen.SetActive(false);
            }
        }
    }
}
