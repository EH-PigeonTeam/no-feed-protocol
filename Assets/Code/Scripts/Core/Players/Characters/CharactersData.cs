using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Players
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Characters", menuName = "No Feed Protocol/Characters")]
    public class CharactersData : ScriptableObject
    {
        [Tooltip("Characters Definitions")]
        [SerializeField]
        private CharacterData[] m_characters = new CharacterData[0];

        public CharacterData[] Characters => m_characters;
        public CharacterData this[int index] => m_characters[index];
    }

    [System.Serializable]
    public class CharacterData
    {
        [FoldoutGroup("@m_name")]
        [Tooltip("")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

        [FoldoutGroup("@m_name/Animations")]
        [Tooltip("")]
        [SerializeField, InlineProperty, HideLabel]
        private CharacterAnim m_anim;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("")]
        [SerializeField]
        private int m_maxHealth;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("")]
        [SerializeField]
        private int m_attackPointsShield;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("")]
        [SerializeField]
        private int m_attackPoints;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_shield;

        [FoldoutGroup("@m_name/Stats")]
        [Tooltip("")]
        [SerializeField, Range(1, 10)]
        private int m_actionPoints = 1;

        //[FoldoutGroup("@m_name")]
        //[Tooltip("")]
        //[SerializeField]
        //private Combat Combat;

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public CharacterAnim Anim => m_anim;
        public int MaxHealth => m_maxHealth;
        public int AttackPointsShield => m_attackPointsShield;
        public int AttackPoints => m_attackPoints;
        public int Shield => m_shield;
        public int ActionPoints => m_actionPoints;
    }

    [System.Serializable]
    public class CharacterAnim
    {
        public AnimationClip Idle;
        public AnimationClip Attack;
        public AnimationClip Damage;
        public AnimationClip Dead;
    }

    public class Player
    {

    }

    public class Combat
    {

    }
}
