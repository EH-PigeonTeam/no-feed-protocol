using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Gameplay.SlotMachine
{
    public class SlotWheel : ButtonAudio
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

        /// <summary>
        /// Locks or unlocks the wheel
        /// </summary>
        /// <param name="state">If true, the wheel is locked</param>
        public void Lock(bool state) => SetWheel(state);
    }

    public interface IResettable
    {
        void Restore();
    }
}
