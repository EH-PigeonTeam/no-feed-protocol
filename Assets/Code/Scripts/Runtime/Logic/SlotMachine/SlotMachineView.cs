using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Core.Gameplay.SlotMachine;
using Core.Gameplay.SlotMachine.Data;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Manages the visual representation of the slot machine:
    /// wheels, indicators, and animations.
    /// </summary>
    [HideMonoScript]
    public class SlotMachineView : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("The delay between each symbol's reveal animation."), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        protected float m_symbolRevealDelay = 0.1f;

        [BoxGroup("Settings")]
        [Tooltip("The easing curve for symbol reveal animations.")]
        [SerializeField]
        protected Ease m_symbolRevealEase = Ease.Linear;

        /// <summary>
        /// Displays the current symbols by updating each wheel's sprite.
        /// </summary>
        public virtual void DisplaySymbols(IReadOnlyList<SlotSymbolData> symbols, List<SlotWheel> slotWheels)
        {
            for (int i = 0; i < slotWheels.Count; i++)
            {
                if (i >= symbols.Count || symbols[i] == null)
                    continue;

                if (slotWheels[i].TryGetComponent<Image>(out var image))
                {
                    image.DOFade(0f, 0f); // fade-out instantly
                    image.sprite = symbols[i].Sprite;
                    image.DOFade(1f, m_symbolRevealDelay).SetEase(m_symbolRevealEase); // fade-in animata
                }
            }
        }

        public void Restore(List<SlotWheel> slotWheels, List<Indicator> indicators)
        {
            LockView(slotWheels, false);

            RestoreIndicators(indicators);
        }

        public void Lock(List<SlotWheel> slotWheels, List<Indicator> indicators)
        {
            LockLogic(slotWheels, true);
            LockView(slotWheels, true);

            foreach (var indicator in indicators)
            {
                indicator.Activate();
            }
        }

        public void LockLogic(List<SlotWheel> slotWheels, bool isLocked)
        {
            foreach (var wheel in slotWheels)
            {
                wheel.interactable = !isLocked;
            }
        }

        private void LockView(List<SlotWheel> slotWheels, bool isLocked)
        {
            foreach (var wheel in slotWheels)
            {
                wheel.Lock(isLocked);
            }
        }

        public void SetActiveIndicator(int index, List<Indicator> indicators)
        {
            if (index >= 0 && index < indicators.Count)
            {
                indicators[index].Activate();
            }
        }

        public void RestoreIndicators(List<Indicator> indicators)
        {
            foreach (var indicator in indicators)
            {
                indicator.Restore();
            }
        }
    }
}
