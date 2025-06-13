using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Battle.Players;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Enums;
using NoFeedProtocol.Runtime.Logic.Turns;
using NoFeedProtocol.Runtime.Services.Characters;
using NoFeedProtocol.Runtime.Services.Items;

namespace NoFeedProtocol.Runtime.Logic.Battle
{
    [HideMonoScript]
    public class BattleManager : MonoBehaviour
    {
        #region Serialized References ----------------------------------------

        [BoxGroup("Battle", ShowLabel = false)]
        [BoxGroup("Battle/Data", ShowLabel = false)]
        [SerializeField, Required]
        private BattleRuntimeData m_battleData;

        [BoxGroup("Battle/Player", ShowLabel = false)]
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private PlayerController m_PlayerController;

        [BoxGroup("Battle/Enemy", ShowLabel = false)]
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private PlayerController m_EnemyController;

        [BoxGroup("Battle/Enemy", ShowLabel = false)]
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private EnemyTeamGenerator m_enemyTeamGenerator;

        [BoxGroup("Battle")]
        [SerializeField, Required, InlineProperty, HideLabel]
        private TurnManager m_turnManager;

        [BoxGroup("Battle")]
        [SerializeField, Required]
        private BannerController m_bannerController;

        private ItemResolver m_itemResolver;
        private BattlePhaseManager m_phaseManager;

        #endregion

        #region Unity Lifecycle ----------------------------------------------

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

        private void Start()
        {
            m_phaseManager = ServiceLocator.Get<BattlePhaseManager>();

            m_phaseManager.OnPhaseChanged += HandlePhaseChanged;

            m_battleData.Set(
                ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player.Clone(),
                m_enemyTeamGenerator.Generate()
            );

            InitializeBattle();
        }

        #endregion

        #region Initialization and State Setup -------------------------------

        private void InitializeBattle()
        {
            m_EnemyController.Initialize(m_battleData.EnemyTeam, ServiceLocator.Get<EnemyCharacterResolver>());
            m_PlayerController.Initialize(m_battleData.PlayerTeam, ServiceLocator.Get<CharacterResolver>());

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
            if (!m_battleData.PlayerTeam.CharactersAreAlive() || !m_battleData.EnemyTeam.CharactersAreAlive())
            {
                m_phaseManager.ChangePhase(BattlePhase.BattleEnd);
            }

            m_turnManager.NextTurn();
            m_phaseManager.ChangePhase(BattlePhase.TurnStart);
        }

        public bool IsPlayerWinning()
        {
            return m_battleData.PlayerTeam.CharactersAreAlive() &&
                   !m_battleData.EnemyTeam.CharactersAreAlive();
        }

        public BattleRuntimeData BattleRuntimeData => m_battleData;
        public bool IsPlayerTurn => m_turnManager.CurrentTurn == TeamSide.Player;

        #endregion

        #region Event Handlers -----------------------------------------------

        private void HandlePhaseChanged(BattlePhase phase)
        {
            PlayerController playerController = IsPlayerTurn ? m_PlayerController : m_EnemyController;

            //playerController.UpdateUI(IsPlayerTurn ? m_battleData.PlayerTeam : m_battleData.EnemyTeam);

            switch (phase)
            {
                case BattlePhase.Setup:
                    StartFirstTurn();
                    break;

                case BattlePhase.TurnStart:
                    playerController.OnTurnStart();
                    m_phaseManager.ChangePhase(BattlePhase.Slot);
                    break;

                case BattlePhase.Slot:
                    playerController.OnSlot();
                    break;

                case BattlePhase.Target:
                    playerController.OnAiming();
                    break;

                case BattlePhase.TurnEnd:
                    playerController.OnTurnEnd();
                    EndTurn();
                    break;

                case BattlePhase.BattleEnd:
                    m_bannerController.ShowScreen(IsPlayerWinning());
                    ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player = m_battleData.PlayerTeam;
                    break;
            }
        }

        #endregion
    }
}
