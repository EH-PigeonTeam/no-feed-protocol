using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Gameplay.SlotMachine
{
    public class SlotWheel : ButtonAudio, IResettable
    {
        [Header("Slot Wheel")]
        [SerializeField] private GameObject m_wheelObject;

        public bool IsLocked => m_wheelObject != null && this.m_wheelObject.activeSelf;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if(!this.interactable)
                return;

            ToggleWheel();
        }

        private void ToggleWheel()
        {
            SetWheel(!this.m_wheelObject.activeSelf);
        }

        public void SetWheel(bool state)
        {
            if (this.m_wheelObject == null)
                return;

            this.m_wheelObject.SetActive(state);
        }

        public void LockWheel(bool state)
        {
            SetWheel(state);
            this.interactable = !state;
        }

        public void Restore()
        {
            LockWheel(false);
        }
    }

    public interface IResettable
    {
        void Restore();
    }
}
