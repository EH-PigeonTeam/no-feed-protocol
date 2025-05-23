using UnityEngine;

namespace NoFeedProtocol.Authoring.Characters.Animation
{
    [System.Serializable]
    public class CharacterAnimationSet
    {
        [Tooltip("Idle animation clip.")]
        [SerializeField]
        private AnimationClip m_idle;

        [Tooltip("Attack animation clip.")]
        [SerializeField]
        private AnimationClip m_attack;

        [Tooltip("Damage reaction animation clip.")]
        [SerializeField]
        private AnimationClip m_damage;

        [Tooltip("Death animation clip.")]
        [SerializeField]
        private AnimationClip m_death;

        public AnimationClip Idle => m_idle;
        public AnimationClip Attack => m_attack;
        public AnimationClip Damage => m_damage;
        public AnimationClip Death => m_death;
    }
}
