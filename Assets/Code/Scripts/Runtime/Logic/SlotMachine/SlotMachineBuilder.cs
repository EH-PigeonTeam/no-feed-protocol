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
        private readonly Transform m_wheelContainer;

        public SlotMachineBuilder(
            GameObject wheelPrefab,
            Transform wheelContainer)
        {
            m_slotWheelPrefab = wheelPrefab;
            m_wheelContainer = wheelContainer;
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
    }
}
