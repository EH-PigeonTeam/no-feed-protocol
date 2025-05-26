using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Logic.Turns;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Slot;
using Core.Gameplay.SlotMachine.Data;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Services.Items;

namespace NoFeedProtocol.Runtime.Logic.Battle
{
    [HideMonoScript]
    public class BattleManager : MonoBehaviour
    {
        #region Serialized References ----------------------------------------

        [SerializeField, Required]
        private BattleRuntimeData m_battleData;

        [FoldoutGroup("Slot Machine Configuration", expanded: true)]
        [Tooltip("The configuration for the slot machines.")]
        [SerializeField] 
        private SlotMachineData m_slotMachineConfig;

        [FoldoutGroup("Slot Machine Configuration")]
        [Tooltip("The items to be used in the player's slot machine.")]
        [SerializeField] 
        private SlotMachineController m_playerSlot;

        [FoldoutGroup("Slot Machine Configuration")]
        [Tooltip("The items to be used in the enemy's slot machine.")]
        [SerializeField] 
        private SlotMachineController m_enemySlot;

        [SerializeField, Required]
        private TurnManager m_turnManager;

        //[SerializeField, Required]
        //private CombatResolver m_combatResolver;

        //[SerializeField, Required]
        //private BattleEventSystem m_eventSystem;

        private ItemResolver m_itemResolver;

        #endregion

        #region Unity Lifecycle ----------------------------------------------

        private void Start()
        {
            m_itemResolver = ServiceLocator.Get<ItemResolver>();

            InitializeBattle();
        }

        #endregion

        #region Initialization and State Setup -------------------------------

        /// <summary>
        /// Prepara tutti i sistemi per iniziare la battaglia.
        /// </summary>
        private void InitializeBattle()
        {
            m_battleData.InitializeFromRuntime();

            m_playerSlot.Setup(this.m_slotMachineConfig, this.m_itemResolver.GetByIds(this.m_battleData.PlayerTeam.Items));
            m_enemySlot.Setup(this.m_slotMachineConfig, this.m_itemResolver.GetByIds(this.m_battleData.EnemyTeam.Items));
            //m_combatResolver.Setup(m_battleData, m_eventSystem);

            StartFirstTurn();
        }

        private void StartFirstTurn()
        {
            m_turnManager.StartFirstTurn();
        }

        #endregion

        #region Public Control Methods ---------------------------------------

        /// <summary>
        /// Passa al turno successivo.
        /// </summary>
        public void EndTurn()
        {
            m_turnManager.NextTurn();
        }

        #endregion
    }
}