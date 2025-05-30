using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using PsychoGarden.Utils;
using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Services.Characters;
using NoFeedProtocol.Runtime.UI;
using Core.Gameplay.SlotMachine.Data;
using NoFeedProtocol.Runtime.Logic.Slot;
using NoFeedProtocol.Runtime.Services.Items;
using System.Collections.Generic;
using System.Linq;
using NoFeedProtocol.Runtime.Logic.Enums;
using DG.Tweening;

namespace NoFeedProtocol.Runtime.Logic.Battle.Players
{
    [HideMonoScript]
    [System.Serializable]
    public class PlayerController : MonoBehaviour
    {
        #region Exposed Members --------------------------------

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, InlineProperty, HideLabel]
        private PlayerViewController m_viewController;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, InlineProperty, HideLabel]
        private PlayerBuilder m_builder;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeReference, InlineProperty, HideLabel, TypeFilter("GetInputHandlerTypes")]
        private InputHandler m_inputHandler;

        private IEnumerable<Type> GetInputHandlerTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                t.IsSubclassOf(typeof(InputHandler)) &&
                !t.IsAbstract &&
                !t.IsInterface);
        }

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeReference, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private IPlayerAimingHandler m_aimingHandler;

        #endregion

        #region Private Members --------------------------------

        [HideInInspector]
        public PlayerRuntimeData RuntimeData;

        [HideInInspector]
        public ICharacterResolver Resolver;

        #endregion

        #region Initialization ---------------------------------

        public void Initialize(PlayerRuntimeData data, ICharacterResolver resolver)
        {
            RuntimeData = data;

            CharactersBuilder characters = this.m_builder.Setup(data, this.transform);
            this.m_viewController.Setup(
                characters.top?.GetComponent<CharacterViewController>(),
                characters.bottom?.GetComponent<CharacterViewController>(),
                data,
                resolver
            );

            this.m_inputHandler.Setup(this);

            this.Resolver = resolver;
        }

        private void OnDisable()
        {
            this.m_inputHandler.OnDispose();
        }

        #endregion

        #region Public Methods ---------------------------------

        public void UpdateUI(PlayerRuntimeData data) => m_viewController.UpdateUI(data);

        public void OnTurnStart()
        {
        }

        public void OnSlot()
        {
            this.m_inputHandler.OnSlot();
        }

        public void OnTurnEnd() { }

        #endregion

        #region Gizmos -----------------------------------------

        private void OnDrawGizmosSelected()
        {
            m_builder?.OnDrawGizmos();
        }

        #endregion
    }

    [System.Serializable]
    public class PlayerViewController
    {
        #region Exposed Members --------------------------------

        [FoldoutGroup("View")]
        [SerializeField, Required]
        private Slider m_shieldBar;

        [FoldoutGroup("View")]
        [ShowInInspector, ReadOnly]
        private CharacterViewController m_characterTop;

        [FoldoutGroup("View")]
        [ShowInInspector, ReadOnly]
        private CharacterViewController m_characterBottom;

        #endregion

        #region Initialization ---------------------------------

        public void Setup(
            CharacterViewController top,
            CharacterViewController bottom,
            PlayerRuntimeData data,
            ICharacterResolver resolver)
        {
            m_characterTop = top;
            m_characterBottom = bottom;

            ICharacterStaticData topData = resolver.GetById(data.CharacterTop.Id);
            ICharacterStaticData bottomData = resolver.GetById(data.CharacterBottom.Id);

            m_characterTop?.Setup(topData.Anim, new CharacterStats(topData, data.CharacterTop));
            m_characterBottom?.Setup(bottomData.Anim, new CharacterStats(bottomData, data.CharacterBottom));

            UpdateUI(data);
        }

        #endregion

        #region Public Methods ---------------------------------

        public void UpdateUI(PlayerRuntimeData data)
        {
            if (data == null)
            {
                return;
            }

            if (!Mathf.Approximately(m_shieldBar.maxValue, data.MaxShield))
            {
                m_shieldBar.maxValue = data.MaxShield;
            }

            if (!Mathf.Approximately(m_shieldBar.value, data.CurrentShield))
            {
                m_shieldBar.value = data.CurrentShield;
            }

            if (m_characterTop != null && data.CharacterTop != null)
            {
                m_characterTop.UpdateState(new CharacterUIState(data.CharacterTop.Health, data.CharacterTop.Energy));
            }

            if (m_characterBottom != null && data.CharacterBottom != null)
            {
                m_characterBottom.UpdateState(new CharacterUIState(data.CharacterBottom.Health, data.CharacterBottom.Energy));
            }
        }

        #endregion
    }

    [System.Serializable]
    public class PlayerBuilder
    {
        #region Exposed Members --------------------------------

        [FoldoutGroup("Builder/Top Character Transform")]
        [SerializeField, InlineProperty, HideLabel]
        private TransformData m_topTransform = TransformData.Default();

        [FoldoutGroup("Builder/Bottom Character Transform")]
        [SerializeField, InlineProperty, HideLabel]
        private TransformData m_bottomTransform = TransformData.Default();

        [FoldoutGroup("Builder")]
        [SerializeField, AssetsOnly, Required]
        private GameObject m_characterPrefab;

        #endregion

        #region Initialization ---------------------------------

        public CharactersBuilder Setup(PlayerRuntimeData data, Transform parent)
        {
            if (data == null)
            {
                Debug.LogError("[PlayerBuilder] Setup failed: PlayerRuntimeData is null");
                return default;
            }

            CharactersBuilder characters = new();

            if (data.CharacterTop != null && data.CharacterTop.Health > 0)
                characters.top = GenerateCharacter(parent, m_topTransform);

            if (data.CharacterBottom != null && data.CharacterBottom.Health > 0)
                characters.bottom = GenerateCharacter(parent, m_bottomTransform);

            return characters;
        }

        #endregion

        #region Private Methods --------------------------------

        private GameObject GenerateCharacter(Transform parent, TransformData transform)
        {
            GameObject character = GameObject.Instantiate(m_characterPrefab, parent);
            character.transform.SetLocalPositionAndRotation(
                transform.Position,
                Quaternion.Euler(transform.Rotation)
            );
            character.transform.localScale = transform.Scale;
            return character;
        }

        #endregion

        #region Gizmos -----------------------------------------

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(m_topTransform.Position, m_topTransform.Scale);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(m_bottomTransform.Position, m_bottomTransform.Scale);
        }

        #endregion
    }

    public class InputHandler
    {
        [FoldoutGroup("Slot Machine Configuration")]
        [SerializeField]
        protected SlotMachineData m_slotMachineConfig;

        [FoldoutGroup("Slot Machine Configuration")]
        [SerializeField]
        protected SlotMachineController m_slotMachine;

        protected PlayerController Player;

        public void Setup(PlayerController player)
        {
            this.Player = player;

            this.m_slotMachine.Setup(
                this.m_slotMachineConfig, 
                ServiceLocator.Get<ItemResolver>().GetByIds(this.Player.RuntimeData.Items));
            this.m_slotMachine.OnSpinCompleted += OnSpinCompleted;
        }

        public void OnDispose()
        {
            this.m_slotMachine.OnSpinCompleted -= OnSpinCompleted;
        }

        public virtual void OnSlot()
        {
            this.m_slotMachine.Reset();
        }

        protected virtual void OnSpinCompleted(SpinResult result)
        {
            Debug.Log($"Spin Completed {result} " + (ServiceLocator.Get<BattleManager>().IsPlayerTurn ? " (Player)" : " (Opponent)"));

            ApplySpinResultToCharacters(result);
            Player.UpdateUI(Player.RuntimeData);

            var phaseManager = ServiceLocator.Get<BattlePhaseManager>();
            if (ShouldEnterTargetPhase())
            {
                Debug.Log("Entering Target Phase");
                phaseManager.ChangePhase(BattlePhase.Target);
            }
            else
            {
                Debug.Log("Entering Turn End Phase");
                phaseManager.ChangePhase(BattlePhase.TurnEnd);
            }
        }

        protected bool HasCharacterRequiredEnergy(CharacterRuntimeData character)
        {
            if (character == null) return false;

            var required = Player.Resolver.GetById(character.Id).EnergyRequired;
            return character.Energy >= required;
        }

        protected bool ShouldEnterTargetPhase()
        {
            return HasCharacterRequiredEnergy(Player.RuntimeData.CharacterTop) ||
                   HasCharacterRequiredEnergy(Player.RuntimeData.CharacterBottom);
        }

        private void ApplySpinResultToCharacters(SpinResult result)
        {
            var top = Player.RuntimeData.CharacterTop;
            var bottom = Player.RuntimeData.CharacterBottom;

            if (result.EnergyTop > 0 && top != null)
                top.Energy += result.EnergyTop;

            if (result.EnergyBottom > 0 && bottom != null)
                bottom.Energy += result.EnergyBottom;

            if (result.ShieldRecovery > 0)
            {
                Player.RuntimeData.CurrentShield = Mathf.Min(
                    Player.RuntimeData.CurrentShield + result.ShieldRecovery,
                    Player.RuntimeData.MaxShield
                );
            }
        }
    }

    public class HumanInputHandler : InputHandler
    {
        
    }

    public class BotInputHandler : InputHandler
    {
        [Tooltip("Delay between each spin"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_spinDelay = 1f;

        public override void OnSlot()
        {
            base.OnSlot();

            DOVirtual.DelayedCall(m_spinDelay, () => m_slotMachine.Spin(), ignoreTimeScale: false);
        }
    }

    public interface IPlayerAimingHandler { }

    public class HumanAimingHandler : IPlayerAimingHandler
    {
    }

    public class BotAimingHandler : IPlayerAimingHandler
    {
    }

    public struct CharactersBuilder
    {
        public GameObject top;
        public GameObject bottom;
    }

    [Serializable]
    public readonly struct CharacterUIState
    {
        public readonly int Health;
        public readonly int Energy;

        public CharacterUIState(int health, int energy)
        {
            Health = health;
            Energy = energy;
        }

        public CharacterUIState(CharacterStats stats)
        {
            Health = stats.Health;
            Energy = stats.Energy;
        }
    }
}
