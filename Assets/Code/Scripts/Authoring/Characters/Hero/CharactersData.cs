using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Characters
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "Characters", menuName = "No Feed Protocol/Characters")]
    public class CharactersData : ScriptableObject
    {
        #region Character Definitions ---------------------------------------

        [BoxGroup("Definitions")]
        [Tooltip("All characters available in the game.")]
        [SerializeField]
        private CharacterData[] m_characters = new CharacterData[0];

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
}