using NoFeedProtocol.Runtime.Entities;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace NoFeedProtocol.Runtime.Combat
{
    /// <summary>
    /// Represents all required data for a character attack.
    /// </summary>
    public struct TargetInfo
    {
        public CharacterRuntimeData Attacker;
        public PlayerRuntimeData AttackerTeam;

        public CharacterRuntimeData Target;
        public PlayerRuntimeData TargetTeam;

        public bool IsValid =>
            Attacker != null &&
            Target != null &&
            AttackerTeam != null &&
            TargetTeam != null;
    }

    /// <summary>
    /// Defines a way to resolve who attacks whom.
    /// </summary>
    public interface ITargetingStrategy
    {
        /// <summary>
        /// Resolves the current targeting data.
        /// </summary>
        TargetInfo Resolve();
    }

    [HideMonoScript]
    public class TargetingResolver : MonoBehaviour
    {
        private ITargetingStrategy m_strategy;

        /// <summary>
        /// Sets the targeting strategy (e.g., player or bot).
        /// </summary>
        public void SetStrategy(ITargetingStrategy strategy)
        {
            m_strategy = strategy;
        }

        /// <summary>
        /// Returns the current resolved target info.
        /// </summary>
        public TargetInfo GetTargetInfo()
        {
            if (m_strategy == null)
                throw new InvalidOperationException("Targeting strategy not set.");

            return m_strategy.Resolve();
        }

        private void OnEnable()
        {
            Code.Systems.Locator.ServiceLocator.Register<TargetingResolver>(this);
        }

        private void OnDisable()
        {
            Code.Systems.Locator.ServiceLocator.Unregister<TargetingResolver>();
        }
    }

    /// <summary>
    /// Targeting logic driven by player's selection from UI.
    /// </summary>
    public class PlayerTargetingStrategy : ITargetingStrategy
    {
        private CharacterRuntimeData m_attacker;
        private PlayerRuntimeData m_attackerTeam;

        private CharacterRuntimeData m_target;
        private PlayerRuntimeData m_targetTeam;

        public void SetAttacker(CharacterRuntimeData attacker, PlayerRuntimeData team)
        {
            m_attacker = attacker;
            m_attackerTeam = team;
        }

        public void SetTarget(CharacterRuntimeData target, PlayerRuntimeData team)
        {
            m_target = target;
            m_targetTeam = team;
        }

        public TargetInfo Resolve()
        {
            return new TargetInfo
            {
                Attacker = m_attacker,
                AttackerTeam = m_attackerTeam,
                Target = m_target,
                TargetTeam = m_targetTeam
            };
        }
    }
}
