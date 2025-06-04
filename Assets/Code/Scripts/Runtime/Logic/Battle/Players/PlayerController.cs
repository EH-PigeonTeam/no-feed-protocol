using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
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
using NoFeedProtocol.Authoring.Characters.Combat;

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

        [BoxGroup("Settings/Input", showLabel: false)]
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

        [BoxGroup("Settings/Aiming", showLabel: false)]
        [Tooltip("")]
        [SerializeReference, InlineProperty, HideLabel, TypeFilter("GetAimingHandlerTypes")]
        private AimingHandler m_aimingHandler;

        private IEnumerable<Type> GetAimingHandlerTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                t.IsSubclassOf(typeof(AimingHandler)) &&
                !t.IsAbstract &&
                !t.IsInterface);
        }

        #endregion

        #region Private Members --------------------------------

        //[HideInInspector]
        public PlayerRuntimeData RuntimeData;

        [HideInInspector]
        public ICharacterResolver Resolver;

        public PlayerViewController PlayerView => m_viewController;

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
            this.m_aimingHandler.Setup(this);

            this.Resolver = resolver;
        }

        private void OnDisable()
        {
            this.m_inputHandler.OnDispose();
            this.m_aimingHandler.OnDispose();
        }

        #endregion

        #region Public Methods ---------------------------------

        public void UpdateUI(PlayerRuntimeData data) => m_viewController.UpdateUI(data);

        public void ApplyCombatDeltas(CombatResult result)
        {
            Debug.LogWarning($"Damage ===============================");

            RuntimeData.CharacterTop.Health = Mathf.Max(0, RuntimeData.CharacterTop.Health + result.HealthTop);
            RuntimeData.CharacterBottom.Health = Mathf.Max(0, RuntimeData.CharacterBottom.Health + result.HealthBottom);
            RuntimeData.CurrentShield = Mathf.Max(0, RuntimeData.CurrentShield + result.ShieldChange);

            PlayerView?.UpdateUI(RuntimeData);
        }

        public void OnTurnStart() { }

        public void OnSlot()
        {
            this.m_inputHandler.OnSlot();
        }

        public void OnAiming()
        {
            this.m_aimingHandler.EnterTargetPhase();
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

        public Button GetCharacterTop() => m_characterTop?.GetComponentInChildren<Button>(true);

        public Button GetCharacterBottom() => m_characterBottom?.GetComponentInChildren<Button>(true);

        public CharacterViewController CharacterTopViewController => m_characterTop;

        public CharacterViewController CharacterBottomViewController => m_characterBottom;

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
        [SerializeField, SceneObjectsOnly]
        private Camera m_uiCamera;

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

            if (character.transform.TryGetComponentInChildren(out Canvas canvas, true))
            {
                canvas.worldCamera = this.m_uiCamera;
            }

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

    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component parent, out T result, bool includeInactive = false) where T : Component
        {
            result = parent.GetComponentInChildren<T>(includeInactive);
            return result != null;
        }
    }

    public class InputHandler
    {
        [SerializeField]
        protected SlotMachineData m_slotMachineConfig;

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

        public override void OnSlot()
        {
            base.OnSlot();
        }
    }

    public class BotInputHandler : InputHandler
    {
        [Tooltip("Delay between each spin"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_spinDelay = 1f;

        public override void OnSlot()
        {
            base.OnSlot();

            DOVirtual.DelayedCall(m_spinDelay, () =>
            {
                m_slotMachine.Spin();
            });
        }
    }

    public class AimingHandler
    {
        [Tooltip("Opponent controller reference (used for targeting logic).")]
        [SerializeField, Required]
        protected PlayerController m_opponent;

        protected PlayerController Owner { get; private set; }
        public CharacterRuntimeData Attacker { get; protected set; }

        protected PlayerController Opponent => m_opponent;

        protected readonly Queue<CharacterRuntimeData> m_attackQueue = new();

        public virtual void Setup(PlayerController owner)
        {
            Owner = owner;
        }

        public virtual void EnterTargetPhase()
        {
            m_attackQueue.Clear();

            if (HasReadyToAttack(Owner.RuntimeData.CharacterTop))
                m_attackQueue.Enqueue(Owner.RuntimeData.CharacterTop);

            if (HasReadyToAttack(Owner.RuntimeData.CharacterBottom))
                m_attackQueue.Enqueue(Owner.RuntimeData.CharacterBottom);

            ProcessNextAttacker();
        }

        public virtual void OnDispose() { }

        protected virtual bool HasReadyToAttack(CharacterRuntimeData character)
        {
            return character.HasReadyToAttack(Owner.Resolver.GetById(character.Id).EnergyRequired);
        }

        protected virtual void TargetSelected(CharacterRuntimeData opponent)
        {
            Attacker.Energy = 0;
            Owner.PlayerView.UpdateUI(Owner.RuntimeData);

            if (Owner.RuntimeData.CharacterTop == Attacker)
            {
                Owner.PlayerView.CharacterTopViewController.PlayAttack();
            }
            else
            {
                Owner.PlayerView.CharacterBottomViewController.PlayAttack();
            }

            CombatResolver.Resolve(new CombatRequest(Attacker, Owner, opponent, Opponent));

            ProcessNextAttacker();
        }

        protected virtual void AttackSelected(CharacterRuntimeData attacker)
        {
            Attacker = attacker;
        }
        protected virtual void ProcessNextAttacker() { }
    }

    public class HumanAimingHandler : AimingHandler
    {
        [ShowInInspector, ReadOnly]
        private Button m_top;

        [ShowInInspector, ReadOnly]
        private Button m_bottom;

        public override void Setup(PlayerController owner)
        {
            base.Setup(owner);

            CharacterRuntimeData top = Opponent?.RuntimeData?.CharacterTop;
            CharacterRuntimeData bottom = Opponent?.RuntimeData?.CharacterBottom;

            m_top = top != null ? Opponent.PlayerView.GetCharacterTop() : null;
            m_bottom = bottom != null ? Opponent.PlayerView.GetCharacterBottom() : null;

            m_top?.onClick.AddListener(() => TargetSelected(top));
            m_bottom?.onClick.AddListener(() => TargetSelected(bottom));
        }

        protected override void AttackSelected(CharacterRuntimeData attacker)
        {
            base.AttackSelected(attacker);
            TargetShow(true);
        }

        private void TargetShow(bool show)
        {
            if (Owner.RuntimeData.CharacterTop.IsAlive)
            {
                m_top?.transform?.parent?.gameObject.SetActive(show);
            }

            if (Owner.RuntimeData.CharacterBottom.IsAlive)
            {
                m_bottom?.transform?.parent?.gameObject.SetActive(show);
            }
        }

        public override void OnDispose()
        {
            m_top?.onClick.RemoveAllListeners();
            m_bottom?.onClick.RemoveAllListeners();
        }
        protected override void ProcessNextAttacker()
        {
            if (m_attackQueue.Count == 0)
            {
                TargetShow(false);
                ServiceLocator.Get<BattlePhaseManager>().ChangePhase(BattlePhase.TurnEnd);
                Debug.Log("All attacks processed. Ending target phase.");
                return;
            }

            Attacker = m_attackQueue.Dequeue();
            Debug.Log($"Attacker Ready: {Attacker.Id}");

            TargetShow(true);
        }

        protected override void TargetSelected(CharacterRuntimeData opponent)
        {
            Debug.Log($"Target Selected: {opponent?.Id}");

            base.TargetSelected(opponent);
        }
    }

    public class BotAimingHandler : AimingHandler
    {
        protected override void ProcessNextAttacker()
        {
            if (m_attackQueue.Count == 0)
            {
                Debug.Log("Bot finished all attacks");
                ServiceLocator.Get<BattlePhaseManager>().ChangePhase(BattlePhase.TurnEnd);
                return;
            }

            Attacker = m_attackQueue.Dequeue();

            var attackerData = Owner.Resolver.GetById(Attacker.Id);
            var aiming = attackerData.AimingBehavior;

            var target = PickTarget(aiming);
            if (target == null)
            {
                Debug.LogWarning("Bot found no valid target.");
                ProcessNextAttacker();
                return;
            }

            TargetSelected(target);
        }

        private CharacterRuntimeData PickTarget(AimingBehavior aiming)
        {
            var top = Opponent.RuntimeData.CharacterTop;
            var bottom = Opponent.RuntimeData.CharacterBottom;

            return aiming.TargetStrategy switch
            {
                TargetStrategy.RandomSplit => UnityEngine.Random.value < aiming.Value ? top : bottom,
                TargetStrategy.PreferLowestHP => GetLowerHP(top, bottom),
                TargetStrategy.PreferHighestHP => GetHigherHP(top, bottom),
                TargetStrategy.All => top ?? bottom, // fallback for now
                _ => null,
            };
        }

        private CharacterRuntimeData GetLowerHP(CharacterRuntimeData a, CharacterRuntimeData b)
        {
            if (a == null || a.Health <= 0f) return b;
            if (b == null || b.Health <= 0f) return a;

            return a.Health <= b.Health ? a : b;
        }

        private CharacterRuntimeData GetHigherHP(CharacterRuntimeData a, CharacterRuntimeData b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return a.Health >= b.Health ? a : b;
        }

        protected override void TargetSelected(CharacterRuntimeData opponent)
        {
            Debug.Log($"[BOT] Target Selected: {opponent?.Id}");

            base.TargetSelected(opponent);
        }
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
        public readonly bool HasShield;

        public CharacterUIState(int health, int energy, bool hasShield = false)
        {
            Health = health;
            Energy = energy;
            HasShield = hasShield;
        }

        public CharacterUIState(CharacterStats stats)
        {
            Health = stats.Health;
            Energy = stats.Energy;
            HasShield = true;
        }
    }
}
