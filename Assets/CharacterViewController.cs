using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using NoFeedProtocol.Shared.Utilities;
using NoFeedProtocol.Authoring.Characters.Animation;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Battle.Players;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class CharacterViewController : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_health;
        [SerializeField] private TMP_Text m_attack;
        [SerializeField] private TMP_Text m_attackToShield;
        [SerializeField] private Slider m_energy;
        [SerializeField] private GameObject m_viewfinder;
        [SerializeField] private Animator m_animator;

        private int m_energyMax;

        /// <summary>
        /// Set static data only once: attack, energy required, etc.
        /// </summary>
        public void SetupStatic(CharacterStats stats)
        {
            Debug.Log("SetupStatic");

            m_attack.text = stats.Attack.ToString();
            m_attackToShield.text = stats.AttackToShield.ToString();

            if (!Mathf.Approximately(m_energy.maxValue, stats.EnergyRequired))
            {
                m_energy.maxValue = stats.EnergyRequired;
            }

            m_energyMax = stats.EnergyRequired;
        }

        /// <summary>
        /// Update only dynamic values.
        /// </summary>
        public void UpdateState(CharacterUIState state)
        {
            Debug.Log("UpdateState");

            if (m_health.text != state.Health.ToString())
            {
                m_health.text = state.Health.ToString();
            }

            if (!Mathf.Approximately(m_energy.value, state.Energy))
            {
                m_energy.value = state.Energy;
            }
        }

        public void SetViewfinder(bool active)
        {
            if (m_viewfinder.TryGetComponent(out CanvasGroupFade fade))
            {
                if (active) m_viewfinder.SetActive(true);
                else fade.FadeOutAndDisable();
            }
            else
            {
                m_viewfinder.SetActive(active);
            }
        }

        public void Setup(CharacterAnimationSet animSet, CharacterStats stats)
        {
            SetupStatic(stats);
            UpdateState(new CharacterUIState(stats));

            if (animSet != null)
            {
                var clips = new Dictionary<string, AnimationClip>
                {
                    { "Medic_Idle", animSet.Idle },
                    { "Medic_Attack", animSet.Attack },
                    { "Medic_Damage", animSet.Damage },
                    { "Medic_Death", animSet.Death }
                };

                m_animator.runtimeAnimatorController = AnimatorInjector.InjectOverrides(
                    m_animator.runtimeAnimatorController,
                    clips
                );
            }
        }

        public void PlayAttack() => m_animator.SetTrigger("Attack");
        public void PlayDamage() => m_animator.SetTrigger("Damage");
        public void PlayDeath() => m_animator.SetTrigger("Death");
    }

    public class CharacterStats
    {
        public int Health { get; set; }
        public int Attack { get; set; }
        public int AttackToShield { get; set; }
        public int Energy { get; set; }
        public int EnergyRequired { get; set; }

        public CharacterStats(int health, int attack, int attackToShield, int energy, int energyRequired)
        {
            Health = health;
            Attack = attack;
            AttackToShield = attackToShield;
            Energy = energy;
            EnergyRequired = energyRequired;
        }

        public CharacterStats(ICharacterStaticData characterData, CharacterRuntimeData characterRuntimeData)
        {
            Health = characterRuntimeData.Health;
            Attack = characterData.AttackPoints;
            AttackToShield = characterData.AttackPointsShield;
            EnergyRequired = characterData.EnergyRequired;
            Energy = characterRuntimeData.Energy;
        }
    }

}
