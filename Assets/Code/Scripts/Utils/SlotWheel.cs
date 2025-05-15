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

            ToggleWheel();
        }

        private void ToggleWheel()
        {
            if (this.m_wheelObject == null)
                return;

            bool newState = !this.m_wheelObject.activeSelf;
            this.m_wheelObject.SetActive(newState);
        }

        public void Restore()
        {
            this.m_wheelObject.SetActive(false);
        }
    }

    public interface IResettable
    {
        void Restore();
    }
}
