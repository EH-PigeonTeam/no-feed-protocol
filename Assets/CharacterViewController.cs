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
using DG.Tweening;

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

        [FoldoutGroup("Shield Animation")]
        [Tooltip("The time it takes to fade in the shield")]
        [SerializeField, ChildGameObjectsOnly]
        private Image m_shield;

        [FoldoutGroup("Shield Animation")]
        [Tooltip("The time it takes to fade in the shield"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_fadeInDuration = 0.5f;

        [FoldoutGroup("Shield Animation")]
        [Tooltip("The time it takes to fade out the shield"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_fadeOutDuration = 0.5f;

        [FoldoutGroup("Shield Animation")]
        [Tooltip("The time it takes to wait between the shield fade in and fade out"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_delayBetween = 2f;

        private int m_energyMax;
        private int m_shieldvalue = 0;

        private void Start()
        {
            if (m_shield != null)
            {
                Color color = m_shield.color;
                color.a = 0f;
                m_shield.color = color;
            }
        }

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
            if (int.Parse(m_health.text) <= 0)
            {
                return;
            }

            Debug.Log("UpdateState");

            Debug.Log($"<color=magenta>{state.Shield} | {state.Shield > 0} =========================");

            if (/*state.Shield > 0 && */state.Shield < m_shieldvalue /*&& state.Health < int.Parse(m_health.text)*/)
            {
                Sequence seq = DOTween.Sequence();

                seq.SetUpdate(true);
                seq.SetAutoKill(true);

                seq.AppendCallback(() => Debug.Log("DOFade Started"));

                seq.Append(m_shield.DOFade(1f, m_fadeInDuration)
                    .SetEase(Ease.InOutSine));

                seq.AppendCallback(() => Debug.Log("FadeInComplete"));

                seq.AppendInterval(m_delayBetween);

                seq.AppendCallback(() => Debug.Log("FadeOut"));

                seq.Append(m_shield.DOFade(0f, m_fadeOutDuration)
                    .SetEase(Ease.InOutSine));

                PlayDamage();
            }
            else if (state.Health < int.Parse(m_health.text))
            {
                PlayDamage();
            }

            if (m_health.text != state.Health.ToString())
            {
                m_health.text = state.Health.ToString();
            }

            if (!Mathf.Approximately(m_energy.value, state.Energy))
            {
                m_energy.value = state.Energy;
            }

            if (state.Health <= 0) PlayDeath();

            m_shieldvalue = state.Shield;
        }

        public void SetViewfinder(bool active)
        {
            if (int.Parse(m_health.text) <= 0)
            {
                return;
            }

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

        public CharacterStats(ICharacterStaticData characterData, CharacterRuntimeData characterRuntimeData, int energyRequired)
        {
            Health = characterRuntimeData.Health;
            Attack = characterData.AttackPoints;
            AttackToShield = characterData.AttackPointsShield;
            EnergyRequired = energyRequired;
            Energy = characterRuntimeData.Energy;
        }
    }

}
