using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Gameplay.SlotMachine;
using Core.Gameplay.SlotMachine.Data;
using NoFeedProtocol.Authoring.Items;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Orchestrates the full slot machine system: logic, view, and dynamic construction.
    /// </summary>
    [HideMonoScript]
    public class SlotMachineController : MonoBehaviour
    {
        [BoxGroup("Slot Configuration")]
        [SerializeField] private bool m_isPlayerControlled = true;

        [FoldoutGroup("Slot Structure", expanded: true)]
        [Tooltip("The slot wheel prefab to instantiate.")]
        [SerializeField, AssetsOnly] 
        private GameObject m_slotWheelPrefab;

        [FoldoutGroup("Slot Structure")]
        [Tooltip("The indicator prefab to instantiate.")]
        [SerializeField, AssetsOnly] 
        private GameObject m_indicatorPrefab;

        [FoldoutGroup("Slot Structure")]
        [Tooltip("The container for slot wheels.")]
        [SerializeField, ChildGameObjectsOnly] 
        private Transform m_wheelContainer;

        [FoldoutGroup("Slot Structure")]
        [Tooltip("The container for spin indicators.")]
        [SerializeField, ChildGameObjectsOnly] 
        private Transform m_indicatorContainer;

        [BoxGroup("References")]
        [SerializeField] private SlotMachineView m_view;

        private SlotMachineLogic m_logic;
        private SlotMachineBuilder m_builder;

        private List<SlotWheel> m_wheels;
        private List<Indicator> m_indicators;

        #region Initialization --------------------------------------------------

        /// <summary>
        /// Sets up the slot machine based on data and items.
        /// </summary>
        public void Setup(SlotMachineData data, List<Item> items)
        {
            m_logic = new SlotMachineLogic();
            m_logic.Setup(data, items);

            m_builder = new SlotMachineBuilder(
                m_slotWheelPrefab,
                m_indicatorPrefab,
                m_wheelContainer,
                m_indicatorContainer
            );

            m_wheels = m_builder.CreateWheels(m_logic.CurrentSymbols.Count);

            m_indicators = m_isPlayerControlled && m_indicatorContainer != null
                ? m_builder.CreateIndicators(data.SpinCount)
                : new List<Indicator>();

            if (m_view != null)
                m_view.Setup(m_wheels, m_indicators);
        }

        #endregion

        #region Public Interface ------------------------------------------------

        /// <summary>
        /// Spins the slot machine and updates the view.
        /// </summary>
        public void Spin()
        {
            if (m_logic.IsSpinLimitReached)
                return;

            m_logic.Spin();
            m_view.DisplaySymbols(m_logic.CurrentSymbols);

            if (m_isPlayerControlled)
                m_view.ActivateIndicator(m_logic.CurrentSymbols.Count - 1);
        }

        /// <summary>
        /// Locks a specific wheel.
        /// </summary>
        public void LockWheel(int index)
        {
            m_logic.LockWheel(index);
            m_view.LockWheel(index);
        }

        /// <summary>
        /// Returns the aggregated result from the current symbols.
        /// </summary>
        public SlotResult GetResult()
        {
            return m_logic.CalculateResult();
        }

        /// <summary>
        /// Resets the entire slot machine state and visuals.
        /// </summary>
        public void Reset()
        {
            m_logic.Reset();
            m_view.Restore();
        }

        /// <summary>
        /// Returns true if the slot machine reached its max spin count.
        /// </summary>
        public bool IsLocked => m_logic.IsSpinLimitReached;

        #endregion
    }
}
