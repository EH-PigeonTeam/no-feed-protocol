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
    public class CharacterData
    {
        #region Unique ID ---------------------------------------------------

        [FoldoutGroup("@m_name")]
        [SerializeField, ReadOnly]
        private string m_id;

        /// <summary>
        /// Unique, non-editable ID used for lookup and save data.
        /// </summary>
        public string Id => this.m_id;

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

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { }

        #endregion

        #region Basic Info --------------------------------------------------

        [FoldoutGroup("@m_name")]
        [Tooltip("Unique name identifier for the character.")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("Main icon for UI display.")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("Alternate icon variant.")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon2;

        [FoldoutGroup("@m_name")]
        [Tooltip("Flavor text or lore description.")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

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

        #region Combat Behavior ---------------------------------------------

        [FoldoutGroup("@m_name/Combat")]
        [Tooltip("Passive or reactive combat behavior.")]
        [SerializeField, InlineProperty, HideLabel]
        private CombatBehavior m_combatBehavior;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => this.m_name;
        public Sprite Icon => this.m_icon;
        public Sprite Icon2 => this.m_icon2;
        public string Description => this.m_description;
        public CharacterAnimationSet Anim => this.m_anim;
        public int MaxHealth => this.m_maxHealth;
        public int AttackPointsShield => this.m_attackPointsShield;
        public int AttackPoints => this.m_attackPoints;
        public int Shield => this.m_shield;
        public int EnergyRequired => this.m_energyRequired;
        public CombatBehavior CombatBehavior => this.m_combatBehavior;

        #endregion
    }
}