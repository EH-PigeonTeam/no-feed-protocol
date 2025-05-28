using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Logic.Turns;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Slot;
using Core.Gameplay.SlotMachine.Data;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Services.Items;
using NoFeedProtocol.Runtime.Logic.Enums;
using System;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Services.Characters;

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
        [Tooltip("The player's slot machine controller.")]
        [SerializeField]
        private SlotMachineController m_playerSlot;

        [FoldoutGroup("Slot Machine Configuration")]
        [Tooltip("The enemy's slot machine controller.")]
        [SerializeField]
        private SlotMachineController m_enemySlot;

        [SerializeField, Required]
        private TurnManager m_turnManager;

        [SerializeField, Required]
        private BannerController m_bannerController;

        private ItemResolver m_itemResolver;
        private BattlePhaseManager m_phaseManager;

        #endregion

        public event Action<bool> OnPlayerAiming;
        public static event Action OnPlayerTurn;

        #region Unity Lifecycle ----------------------------------------------

        private void Start()
        {
            m_itemResolver = ServiceLocator.Get<ItemResolver>();
            m_phaseManager = ServiceLocator.Get<BattlePhaseManager>();

            m_phaseManager.OnPhaseChanged += HandlePhaseChanged;

            InitializeBattle();
        }

        private void OnEnable()
        {
            ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            if (m_phaseManager != null)
                m_phaseManager.OnPhaseChanged -= HandlePhaseChanged;

            ServiceLocator.Unregister<BattleManager>();
        }

        #endregion

        #region Initialization and State Setup -------------------------------

        private void InitializeBattle()
        {
            m_playerSlot.Setup(m_slotMachineConfig, m_itemResolver.GetByIds(m_battleData.PlayerTeam.Items));
            m_playerSlot.OnSpinCompleted += OnPlayerSpinCompleted;

            m_enemySlot.Setup(m_slotMachineConfig, m_itemResolver.GetByIds(m_battleData.EnemyTeam.Items));
            m_enemySlot.OnSpinCompleted += OnEnemySpinCompleted;

            m_phaseManager.ChangePhase(BattlePhase.Setup);
        }

        private void StartFirstTurn()
        {
            m_turnManager.StartFirstTurn();
            m_phaseManager.ChangePhase(BattlePhase.TurnStart);
        }

        #endregion

        #region Public Control Methods ---------------------------------------

        public void EndTurn()
        {
            OnPlayerAiming?.Invoke(false);

            m_turnManager.NextTurn();
            m_phaseManager.ChangePhase(BattlePhase.TurnStart);
        }

        public bool IsPlayerWinning()
        {
            return this.m_battleData.PlayerTeam.CharactersAreAlive() && !this.m_battleData.EnemyTeam.CharactersAreAlive();
        }

        public BattleRuntimeData BattleRuntimeData => m_battleData;
        public bool IsPlayerTurn => m_turnManager.CurrentTurn == TeamSide.Player;

        #endregion

        #region Event Handlers -----------------------------------------------

        private void HandlePhaseChanged(BattlePhase phase)
        {
            switch (phase)
            {
                case BattlePhase.Setup:
                    StartFirstTurn();
                    break;

                case BattlePhase.TurnStart:
                    this.m_phaseManager.ChangePhase(BattlePhase.Slot);


                    break;

                case BattlePhase.Slot:
                    this.m_turnManager.NextTurn();
                    OnPlayerTurn?.Invoke();
                    break;

                case BattlePhase.Target:

                    OnPlayerAiming?.Invoke(IsPlayerTurn);
                    break;

                case BattlePhase.TurnEnd:
                    EndTurn();
                    break;

                case BattlePhase.BattleEnd:
                    this.m_bannerController.ShowScreen(IsPlayerWinning());
                    break;
            }
        }

        private void OnPlayerSpinCompleted(SlotResult result)
        {
            Debug.Log("PLAYER Spin done: " + result);

            if (HasCharacterRequiredEnergy(m_battleData.PlayerTeam.CharacterTop))
            {
                m_phaseManager.ChangePhase(BattlePhase.Target);
            }
            else if (HasCharacterRequiredEnergy(m_battleData.PlayerTeam.CharacterBottom))
            {
                m_phaseManager.ChangePhase(BattlePhase.Target);
            }
        }

        private void OnEnemySpinCompleted(SlotResult result)
        {
            Debug.Log("ENEMY Spin done: " + result);

            if (HasCharacterRequiredEnergy(m_battleData.EnemyTeam.CharacterTop))
            {
                m_phaseManager.ChangePhase(BattlePhase.Target);
            }
            else if (HasCharacterRequiredEnergy(m_battleData.EnemyTeam.CharacterBottom))
            {
                m_phaseManager.ChangePhase(BattlePhase.Target);
            }
        }

        private bool HasCharacterRequiredEnergy(CharacterRuntimeData character)
        {
            return character.Energy >= ServiceLocator.Get<CharacterResolver>().GetById(character.Id).EnergyRequired;
        }

        #endregion
    }
}
