using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Entities;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Services.Characters;

namespace NoFeedProtocol.Runtime.Logic.Battle
{
    [HideMonoScript]
    public class EnemyTeamGenerator : MonoBehaviour
    {
        #region Inspector Configuration --------------------------------------

        [BoxGroup("Enemy Config")]
        [SerializeField, Required]
        private CharactersEnemyData m_enemyDatabase;

        [BoxGroup("Override Selection")]
        [SerializeField]
        private bool m_manualOverride;

        [ShowIf("m_manualOverride")]
        [BoxGroup("Override Selection")]
        [SerializeField]
        private string m_overrideTop;

        [ShowIf("m_manualOverride")]
        [BoxGroup("Override Selection")]
        [SerializeField]
        private string m_overrideBottom;

        #endregion

        #region Public API --------------------------------------------------

        /// <summary>
        /// Generates the runtime data for the enemy team, either manually or randomly.
        /// </summary>
        public PlayerRuntimeData Generate()
        {
            EnemyCharacterResolver m_enemyDatabase = ServiceLocator.Get<EnemyCharacterResolver>();
            CharacterEnemyData top = m_manualOverride ? m_enemyDatabase.GetById(m_overrideTop) : GetRandomCharacter();
            CharacterEnemyData bottom = m_manualOverride ? m_enemyDatabase.GetById(m_overrideBottom) : GetRandomCharacter(exclude: top);

            return new PlayerRuntimeData
            {
                CharacterTop = CreateRuntime(top),
                CharacterBottom = CreateRuntime(bottom),

                CurrentShield = top.Shield + bottom.Shield,
                MaxShield = top.Shield + bottom.Shield,
                Items = new() // No items for enemy by default
            };
        }

        #endregion

        #region Internal Helpers --------------------------------------------

        private CharacterEnemyData GetRandomCharacter(CharacterEnemyData exclude = null)
        {
            var pool = m_enemyDatabase.Characters
                .Where(c => c != null && c != exclude)
                .ToList();

            float totalWeight = pool.Sum(c => c.Percent);
            float pick = UnityEngine.Random.Range(0f, totalWeight);

            float cumulative = 0f;
            foreach (var character in pool)
            {
                cumulative += character.Percent;
                if (pick <= cumulative)
                    return character;
            }

            Debug.LogWarning("[EnemyTeamGenerator] Fallback: returning first available enemy.");
            return pool.FirstOrDefault();
        }

        private CharacterRuntimeData CreateRuntime(CharacterEnemyData data)
        {
            return new CharacterRuntimeData
            {
                Id = data.Id,
                Health = data.MaxHealth,
                Energy = 0
            };
        }

        #endregion
    }
}
