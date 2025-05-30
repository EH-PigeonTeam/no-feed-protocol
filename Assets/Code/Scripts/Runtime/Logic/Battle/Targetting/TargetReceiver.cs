using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Entities;
using System;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class TargetReceiver : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField, Required]
        private Button m_button;

        private bool m_isTop;
        private CharacterRuntimeData m_characterData;

        /// <summary>
        /// Invoked when this character is selected as a target.
        /// Params: isTop, characterData, receiver
        /// </summary>
        public static event Action<bool, CharacterRuntimeData, TargetReceiver> OnTargetSelected;

        public void Setup(bool isTop, CharacterRuntimeData characterData)
        {
            m_isTop = isTop;
            m_characterData = characterData;
        }

        private void OnEnable()
        {
            m_button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            m_button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnTargetSelected?.Invoke(m_isTop, m_characterData, this);
        }

        public void SetActive(bool active)
        {
            m_button.gameObject.SetActive(active);
        }
    }
}
