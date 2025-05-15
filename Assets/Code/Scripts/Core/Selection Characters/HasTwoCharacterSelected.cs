using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;
using Code.Systems.Locator;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class HasTwoCharacterSelected : MonoBehaviour
    {
        [BoxGroup("References")]
        [Tooltip("")]
        [SerializeField]
        private TriggerEvent m_interactionEvent;

        private CharacterSelectionController m_characterSelectionController;

        public void Start()
        {
            this.m_characterSelectionController = ServiceLocator.Get<CharacterSelectionController>();
        }

        public void Interact()
        {
            if (!this.m_characterSelectionController.HasTwoCharacterSelected())
                return;

            this.m_interactionEvent?.Invoke(this.transform);
        }
    }
}
