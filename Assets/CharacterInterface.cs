using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using NoFeedProtocol.Shared.Utilities;
using System.Collections.Generic;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Characters.Animation;
using NoFeedProtocol.Authoring.Characters;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Battle;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class CharacterInterface : MonoBehaviour
    {
        #region Fields -------------------------------------------

        [BoxGroup("Components")]
        [Tooltip("The health text")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_health;

        [BoxGroup("Components")]
        [Tooltip("The attack text")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_attack;

        [BoxGroup("Components")]
        [Tooltip("The attack to shield text")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_attackToShield;

        [BoxGroup("Components")]
        [Tooltip("The energy bar of the character")]
        [SerializeField, ChildGameObjectsOnly]
        private Slider m_energy;

        [BoxGroup("Components")]
        [Tooltip("The animator of the character")]
        [SerializeField, ChildGameObjectsOnly]
        private Animator m_animator;

        [BoxGroup("Components")]
        [Tooltip("The viewfinder of the character")]
        [SerializeField, ChildGameObjectsOnly]
        private GameObject m_viewfinder;

        [BoxGroup("Settings")]
        [SerializeField]
        private bool m_isEnemy;

        private BattleManager m_battleManager;

        #endregion

        public void Init(CharacterRuntimeData data, CharactersData charactersData)
        {
            Health(data.Health.ToString());

            var character = charactersData.GetById(data.Id);
            if (character == null)
                return;

            Attack(character.AttackPoints.ToString());
            AttackToShield(character.AttackPointsShield.ToString());
            EnergyMax(character.EnergyRequired);
            Energy(character.EnergyRequired);
            SetAnimator(character.Anim);

            if (this.m_isEnemy)
            {
                this.m_battleManager = ServiceLocator.Get<BattleManager>();

                this.m_battleManager.OnPlayerAiming += SetViewfinderActive;
            }
        }

        private void OnDisable()
        {
            if (this.m_isEnemy)
            {
                this.m_battleManager.OnPlayerAiming -= SetViewfinderActive;
            }
        }

        #region Methods ------------------------------------------

        public void Health(string value)
        {
            this.m_health.text = value.ToString();
        }

        public void Attack(string value)
        {
            this.m_attack.text = value.ToString();
        }

        public void AttackToShield(string value)
        {
            this.m_attackToShield.text = value.ToString();
        }

        public void EnergyMax(int value)
        {
            this.m_energy.maxValue = value;
        }

        public void Energy(float value)
        {
            this.m_energy.value = value;
        }

        public void SetAnimator(CharacterAnimationSet anim)
        {
            var clips = new Dictionary<string, AnimationClip>
            {
                { "Medic_Idle", anim.Idle },
                { "Medic_Attack", anim.Attack },
                { "Medic_Damage", anim.Damage },
                { "Medic_Death", anim.Death }
            };

            this.m_animator.runtimeAnimatorController = AnimatorInjector.InjectOverrides(
                this.m_animator.runtimeAnimatorController,
                clips
            );
        }

        private void SetViewfinderActive(bool isActive)
        {
            if (this.m_viewfinder.TryGetComponent(out CanvasGroupFade fade) && this.m_viewfinder.activeSelf)
            {
                fade.FadeOutAndDisable();
            }
            else
            {
                this.m_viewfinder.SetActive(isActive);
            }
        }

        #endregion
    }
}
