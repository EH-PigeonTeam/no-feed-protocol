using UnityEngine;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Services.Resolvers;
using Code.Systems.Locator;
using Sirenix.OdinInspector;
using System.Linq;

namespace NoFeedProtocol.Runtime.Services.Characters
{
    [HideMonoScript]
    public class EnemyCharacterResolver : GenericResolver<CharacterEnemyData, CharactersEnemyData>, ICharacterResolver
    {
        private void OnEnable() => ServiceLocator.Register(this);
        private void OnDisable() => ServiceLocator.Unregister<EnemyCharacterResolver>();

        public override CharacterEnemyData GetById(string id)
        {
            var character = m_database.Characters.FirstOrDefault(c => c.Id == id);

#if UNITY_EDITOR
            if (character == null)
                Debug.LogWarning($"[EnemyCharacterResolver] Enemy Character with ID '{id}' not found.");
#endif
            return character;
        }

        ICharacterStaticData ICharacterResolver.GetById(string id) => GetById(id);
    }
}
