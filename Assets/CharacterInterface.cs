using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using NoFeedProtocol.Shared.Utilities;
using System.Collections.Generic;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Characters.Animation;
using NoFeedProtocol.Authoring.Characters;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class CharacterInterface : MonoBehaviour
    {
        #region Fields -------------------------------------------

        [SerializeField]
        private TMP_Text m_health;

        [SerializeField]
        private TMP_Text m_attack;

        [SerializeField]
        private TMP_Text m_attackToShield;

        [SerializeField]
        private Slider m_energy;

        [SerializeField]
        private Animator m_animator;

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

        #endregion
    }
}
