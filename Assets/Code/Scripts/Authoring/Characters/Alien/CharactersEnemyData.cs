using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Characters
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Characters", menuName = "No Feed Protocol/Characters (Enemy)")]
    public class CharactersEnemyData : ScriptableObject
    {
        #region Character Definitions ---------------------------------------

        [BoxGroup("Definitions")]
        [Tooltip("All characters available in the game.")]
        [SerializeField]
        private CharacterEnemyData[] m_characters = new CharacterEnemyData[0];

        #endregion

        #region OnValidate --------------------------------------------------

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var character in m_characters)
                character.OnValidate();
        }
#endif

        #endregion

        #region Public Properties -------------------------------------------

        public CharacterEnemyData[] Characters => this.m_characters;
        public CharacterEnemyData this[int index] => this.m_characters[index];

        /// <summary>
        /// Finds a character by its unique ID.
        /// </summary>
        public CharacterEnemyData GetById(string id)
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
}