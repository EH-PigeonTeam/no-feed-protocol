using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Characters;

namespace NoFeedProtocol.Runtime.Services.Characters
{
    [HideMonoScript]
    public class CharacterResolver : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField, InlineEditor]
        private CharactersData m_database;

        /// <summary>
        /// Resolves a single character by its unique ID.
        /// </summary>
        public CharacterData GetById(string id)
        {
            CharacterData character = m_database.Characters.FirstOrDefault(i => i.Id == id);
#if UNITY_EDITOR
            if (character == null)
                Debug.LogWarning($"[ItemResolver] Item with ID '{id}' not found.");
#endif

            return character;
        }

        /// <summary>
        /// Resolves multiple items by their unique IDs.
        /// </summary>
        public List<CharacterData> GetByIds(List<string> ids)
        {
            return ids
                .Select(GetById)
                .Where(characters => characters != null)
                .ToList();
        }

        private void OnEnable()
        {
            ServiceLocator.Register<CharacterResolver>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<CharacterResolver>();
        }
    }
}
