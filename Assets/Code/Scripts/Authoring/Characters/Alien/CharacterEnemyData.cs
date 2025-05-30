using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Characters.Combat;
using NoFeedProtocol.Authoring.Characters.Animation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NoFeedProtocol.Authoring.Characters
{
    [Serializable]
    public class CharacterEnemyData : ICharacterStaticData
    {
        #region Unique ID ---------------------------------------------------

        [FoldoutGroup("@m_name")]
        [SerializeField, ReadOnly]
        private string m_id;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(m_id))
            {
                m_id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(Selection.activeObject);
            }
        }
#endif

        #endregion

        #region Basic Info --------------------------------------------------

        [FoldoutGroup("@m_name")]
        [Tooltip("Unique name identifier for the character.")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage chance of this character appearing in the game.")]
        [SerializeField, Range(0f, 1f)]
        private float m_percent = 1f;

        #endregion

        #region Animations --------------------------------------------------

        [FoldoutGroup("@m_name/Animations")]
        [Tooltip("Animation set for character actions.")]
        [SerializeField, InlineProperty, HideLabel]
        private CharacterAnimationSet m_anim;

        #endregion

        #region Stats -------------------------------------------------------

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("Maximum health points.")]
        [SerializeField]
        private int m_maxHealth;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("Attack damage applied to enemy shield.")]
        [SerializeField]
        private int m_attackPointsShield;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("Attack damage applied to enemy health.")]
        [SerializeField]
        private int m_attackPoints;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("Initial shield contribution for team.")]
        [SerializeField, MinValue(0)]
        private int m_shield;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("The amount of energy required to attack.")]
        [SerializeField, Range(1, 10)]
        private int m_energyRequired = 1;

        #endregion

        #region Behaviors ---------------------------------------------------

        [FoldoutGroup("@m_name/Combat")]
        [Tooltip("Passive or reactive combat behavior.")]
        [SerializeField, InlineProperty, HideLabel]
        private CombatBehavior m_combatBehavior;

        [FoldoutGroup("@m_name/Aiming")]
        [Tooltip("Passive or reactive combat behavior.")]
        [SerializeField, InlineProperty, HideLabel]
        private AimingBehavior m_aimingBehavior;

        #endregion

        #region Public Properties -------------------------------------------

        public string Id => this.m_id;
        public string Name => this.m_name;
        public float Percent => this.m_percent;
        public CharacterAnimationSet Anim => this.m_anim;
        public int MaxHealth => this.m_maxHealth;
        public int AttackPointsShield => this.m_attackPointsShield;
        public int AttackPoints => this.m_attackPoints;
        public int Shield => this.m_shield;
        public int EnergyRequired => this.m_energyRequired;
        public CombatBehavior CombatBehavior => this.m_combatBehavior;
        public AimingBehavior AimingBehavior => this.m_aimingBehavior;

        #endregion
    }
}