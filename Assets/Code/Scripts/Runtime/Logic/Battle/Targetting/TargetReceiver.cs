using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Entities;
using System;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Battle;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class TargetReceiver : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField, Required, ChildGameObjectsOnly]
        private GameObject m_button;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
        }

        public void SetActive(bool active)
        {
            m_button.gameObject.SetActive(active);
        }
    }
}
