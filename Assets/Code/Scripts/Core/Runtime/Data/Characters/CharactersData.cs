using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeelProtocol.Runtime.Data.Combat;
using UnityEditor;

namespace NoFeelProtocol.Runtime.Data.Characters
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Characters", menuName = "No Feel Protocol/Characters")]
    public class CharactersData : ScriptableObject
    {
        #region Character Definitions ---------------------------------------

        [BoxGroup("Definitions")]
        [Tooltip("All characters available in the game.")]
        [SerializeField]
        private CharacterData[] m_characters = new CharacterData[0];

        #endregion

        #region OnValidate --------------------------------------------------

        private void OnValidate()
        {
            foreach (var character in m_characters)
                character.OnValidate();
        }

        #endregion

        #region Public Properties -------------------------------------------

        public CharacterData[] Characters => this.m_characters;
        public CharacterData this[int index] => this.m_characters[index];
        
        /// <summary>
        /// Finds a character by its unique ID.
        /// </summary>
        public CharacterData GetById(string id)
        {
            foreach (var character in m_characters)
            {
                if (character.Id == id)
                    return character;
            }

            Debug.LogWarning($"Character ID '{id}' not found.");
            return null;
        }

        #endregion
    }

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
        /// <summary>
        /// Ensures a unique ID is assigned automatically in editor.
        /// </summary>
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
        private CharacterAnim m_anim;

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
        [Tooltip("How many action points are needed to trigger attack.")]
        [SerializeField, Range(1, 10)]
        private int m_actionPoints = 1;

        #endregion

        #region Combat Behavior ---------------------------------------------

        [FoldoutGroup("@m_name/Combat")]
        [Tooltip("Passive or reactive combat behavior.")]
        [SerializeField, InlineProperty, HideLabel]
        private CharacterCombatBehavior m_combatBehavior;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => this.m_name;
        public Sprite Icon => this.m_icon;
        public Sprite Icon2 => this.m_icon2;
        public string Description => this.m_description;
        public CharacterAnim Anim => this.m_anim;
        public int MaxHealth => this.m_maxHealth;
        public int AttackPointsShield => this.m_attackPointsShield;
        public int AttackPoints => this.m_attackPoints;
        public int Shield => this.m_shield;
        public int ActionPoints => this.m_actionPoints;
        public CharacterCombatBehavior CombatBehavior => this.m_combatBehavior;

        #endregion
    }

    [Serializable]
    public class CharacterAnim
    {
        [Tooltip("Idle animation clip.")]
        public AnimationClip Idle;

        [Tooltip("Attack animation clip.")]
        public AnimationClip Attack;

        [Tooltip("Damage reaction animation clip.")]
        public AnimationClip Damage;

        [Tooltip("Death animation clip.")]
        public AnimationClip Dead;
    }
}
