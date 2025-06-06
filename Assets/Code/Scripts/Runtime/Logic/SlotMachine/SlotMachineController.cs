using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Core.Gameplay.SlotMachine;
using Core.Gameplay.SlotMachine.Data;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Runtime.UI;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Orchestrates the full slot machine system: logic, view, and dynamic construction.
    /// </summary>
    [HideMonoScript]
    public class SlotMachineController : MonoBehaviour
    {
        public delegate void SpinResultHandler(SpinResult result);
        public event SpinResultHandler OnSpinCompleted;

        [BoxGroup("Slot Configuration")]
        [SerializeField]
        private bool m_isPlayerControlled = true;

        [FoldoutGroup("Slot Structure", expanded: true)]
        [Tooltip("The slot wheel prefab to instantiate.")]
        [SerializeField, AssetsOnly]
        private GameObject m_slotWheelPrefab;

        [FoldoutGroup("Slot Structure")]
        [Tooltip("The container for slot wheels.")]
        [SerializeField, ChildGameObjectsOnly]
        private Transform m_wheelContainer;

        [FoldoutGroup("Slot Structure")]
        [Tooltip("The container for spin indicators.")]
        [SerializeField, ChildGameObjectsOnly]
        private IndicatorManager m_indicatorManager;

        [BoxGroup("References")]
        [SerializeField] private SlotMachineView m_view;

        private SlotMachineLogic m_logic;
        private SlotMachineBuilder m_builder;

        private List<SlotWheel> m_wheels;

        [Button]
        private void Test(SlotMachineData data)
        {
            Setup(data, null);
        }

        #region Initialization --------------------------------------------------

        /// <summary>
        /// Sets up the slot machine based on data and items.
        /// </summary>
        public void Setup(SlotMachineData data, List<Item> items)
        {
            m_logic = new SlotMachineLogic();
            m_logic.Setup(data, items);
            m_logic.OnSpinCompleted += HandleSpinComplete;

            m_builder = new SlotMachineBuilder(
                m_slotWheelPrefab,
                m_wheelContainer
            );

            m_wheels = m_builder.CreateWheels(data.SlotWheelCount);
        }

        private void OnDisable()
        {
            m_logic.OnSpinCompleted -= HandleSpinComplete;
        }

        #endregion

        #region Public Interface ------------------------------------------------

        /// <summary>
        /// Spins the slot machine and updates the view.
        /// </summary>
        [Button]
        public void Spin()
        {
            if (m_isPlayerControlled)
            {
                if (m_logic.IsSpinLimitReached)
                {
                    return;
                }

                m_logic.Spin(GetLockedIndexes());
                m_view.DisplaySymbols(m_logic.CurrentSymbols, m_wheels);

                if (m_logic.Count == 1)
                {
                    m_view.LockLogic(m_wheels, false);
                }

                if (m_logic.IsSpinLimitReached)
                {
                    m_view.Lock(m_wheels);
                    this.m_indicatorManager?.UseAll();
                    return;
                }

                this.m_indicatorManager?.UseNext();
            }
            else
            {
                m_logic.Spin();
                m_view.DisplaySymbols(m_logic.CurrentSymbols, m_wheels);
            }
        }

        /// <summary>
        /// Returns the aggregated result from the current symbols.
        /// </summary>
        public SpinResult GetResult()
        {
            return m_logic.CalculateResult();
        }

        /// <summary>
        /// Resets the entire slot machine state and visuals.
        /// </summary>
        [Button]
        public void Reset()
        {
            if (m_wheels.IsNullOrEmpty() /*|| m_indicators.IsNullOrEmpty()*/)
            {
                return;
            }

            m_logic?.Reset();
            m_view?.Restore(m_wheels);

            this.m_indicatorManager?.Restore();
        }

        private List<int> GetLockedIndexes()
        {
            List<int> ints = new();
            for (int i = 0; i < m_wheels.Count; i++)
            {
                if (!m_wheels[i].IsLocked)
                {
                    continue;
                }
                ints.Add(i);
            }
            return ints;
        }

        #endregion

        #region Event Handlers --------------------------------------------------

        private void HandleSpinComplete(SpinResult result)
        {
            OnSpinCompleted?.Invoke(result);
        }

        #endregion

    }
}
