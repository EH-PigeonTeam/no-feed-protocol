using System.Collections.Generic;
using UnityEngine;
using Core.Gameplay.SlotMachine;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Responsible for building the visual components of a slot machine based on configuration.
    /// </summary>
    public class SlotMachineBuilder
    {
        private readonly GameObject m_slotWheelPrefab;
        private readonly GameObject m_indicatorPrefab;
        private readonly Transform m_wheelContainer;
        private readonly Transform m_indicatorContainer;

        public SlotMachineBuilder(
            GameObject wheelPrefab,
            GameObject indicatorPrefab,
            Transform wheelContainer,
            Transform indicatorContainer)
        {
            m_slotWheelPrefab = wheelPrefab;
            m_indicatorPrefab = indicatorPrefab;
            m_wheelContainer = wheelContainer;
            m_indicatorContainer = indicatorContainer;
        }

        /// <summary>
        /// Instantiates slot wheel GameObjects and returns their components.
        /// </summary>
        public List<SlotWheel> CreateWheels(int wheelCount)
        {
            var result = new List<SlotWheel>();

            for (int i = 0; i < wheelCount; i++)
            {
                var obj = Object.Instantiate(m_slotWheelPrefab, m_wheelContainer);
                if (obj.TryGetComponent(out SlotWheel wheel))
                    result.Add(wheel);
                else
                    Debug.LogError($"SlotWheel prefab is missing SlotWheel component: {obj.name}");
            }

            return result;
        }

        /// <summary>
        /// Instantiates spin indicator GameObjects and returns their components.
        /// </summary>
        public List<Indicator> CreateIndicators(int spinCount)
        {
            var result = new List<Indicator>();

            for (int i = 0; i < spinCount; i++)
            {
                var obj = Object.Instantiate(m_indicatorPrefab, m_indicatorContainer);
                if (obj.TryGetComponent(out Indicator indicator))
                    result.Add(indicator);
                else
                    Debug.LogError($"Indicator prefab is missing Indicator component: {obj.name}");
            }

            return result;
        }
    }
}
