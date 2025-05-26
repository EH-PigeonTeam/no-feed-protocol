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
        private float m_symbolRevealDelay = 0.1f;

        [FoldoutGroup("Debug", expanded: true)]
        [Tooltip("The list of instantiated slot wheels.")]
        [ShowInInspector, ReadOnly]
        private List<SlotWheel> m_slotWheels;

        [FoldoutGroup("Debug")]
        [Tooltip("The list of instantiated spin indicators.")]
        [ShowInInspector, ReadOnly]
        private List<Indicator> m_indicators;

        /// <summary>
        /// Initializes the view with instantiated visual components.
        /// </summary>
        public void Setup(List<SlotWheel> wheels, List<Indicator> indicators)
        {
            m_slotWheels = wheels;
            m_indicators = indicators;
        }

        /// <summary>
        /// Displays the current symbols by updating each wheel's sprite.
        /// </summary>
        public void DisplaySymbols(IReadOnlyList<SlotSymbolData> symbols)
        {
            for (int i = 0; i < m_slotWheels.Count; i++)
            {
                if (i >= symbols.Count || symbols[i] == null)
                    continue;

                var image = m_slotWheels[i].GetComponent<Image>();
                if (image != null)
                {
                    image.DOFade(0f, 0f); // fade-out instantly
                    image.sprite = symbols[i].Sprite;
                    image.DOFade(1f, m_symbolRevealDelay); // fade-in animata
                }
            }
        }

        /// <summary>
        /// Visually locks a wheel by graying it out or disabling it.
        /// </summary>
        public void LockWheel(int index)
        {
            if (index < 0 || index >= m_slotWheels.Count)
                return;

            m_slotWheels[index].LockWheel(true);
        }

        /// <summary>
        /// Resets the view state of wheels and indicators.
        /// </summary>
        public void Restore()
        {
            foreach (var wheel in m_slotWheels)
            {
                wheel.Restore();
            }

            foreach (var indicator in m_indicators)
            {
                indicator.Restore();
            }
        }

        /// <summary>
        /// Activates a spin indicator at the given index.
        /// </summary>
        public void ActivateIndicator(int index)
        {
            if (index >= 0 && index < m_indicators.Count)
            {
                m_indicators[index].Activate();
            }
        }
    }
}
