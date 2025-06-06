using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Gameplay.SlotMachine
{
    public class SlotWheel : ButtonAudio
    {
        [Header("Slot Wheel")]
        [SerializeField] private Image m_display;
        [SerializeField] private GameObject m_wheelObject;
        [SerializeField] private GameObject m_wheelObjectActive;

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
            this.m_wheelObject?.SetActive(state);
            this.m_wheelObjectActive?.SetActive(!state);
        }

        /// <summary>
        /// Locks or unlocks the wheel
        /// </summary>
        /// <param name="state">If true, the wheel is locked</param>
        public void Lock(bool state) => SetWheel(state);

        public Image Display => this.m_display;
    }

    public interface IResettable
    {
        void Restore();
    }
}
