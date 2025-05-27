using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Logic.Enums;
using Code.Systems.Locator;

namespace NoFeedProtocol.Runtime.Logic
{
    [HideMonoScript]
    public class BattlePhaseManager : MonoBehaviour
    {
        [BoxGroup("Debug")]
        [ShowInInspector, ReadOnly]
        public BattlePhase CurrentPhase { get; private set; } = BattlePhase.None;

        /// <summary>
        /// Raised every time the phase changes.
        /// </summary>
        public event Action<BattlePhase> OnPhaseChanged;

        private void OnEnable()
        {
            ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<BattlePhaseManager>();
        }

        /// <summary>
        /// Changes the current phase and notifies subscribers.
        /// </summary>
        [Button]
        public void ChangePhase(BattlePhase newPhase)
        {
            if (CurrentPhase == newPhase)
                return;

            CurrentPhase = newPhase;
            Debug.Log($"[<color=magenta>BattlePhaseManager</color>] Phase changed to: {newPhase}");
            OnPhaseChanged?.Invoke(newPhase);
        }
    }
}

namespace NoFeedProtocol.Runtime.Logic.Enums
{
    public enum BattlePhase
    {
        None,
        Setup,
        TurnStart,
        Slot,
        Target,
        TurnEnd,
        BattleEnd
    }
}
