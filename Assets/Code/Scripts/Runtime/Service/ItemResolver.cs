using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Items;
using Code.Systems.Locator;

namespace NoFeedProtocol.Runtime.Services.Items
{
    [HideMonoScript]
    public class ItemResolver : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField, InlineEditor]
        private ItemsData m_database;

        /// <summary>
        /// Resolves a single character by its unique ID.
        /// </summary>
        public Item GetById(string id)
        {
            var item = m_database.Items.FirstOrDefault(i => i.Id == id);
#if UNITY_EDITOR
            if (item == null)
                Debug.LogWarning($"[ItemResolver] Item with ID '{id}' not found.");
#endif
            return item;
        }

        /// <summary>
        /// Resolves multiple items by their unique IDs.
        /// </summary>
        public List<Item> GetByIds(IEnumerable<string> ids)
        {
            return ids
                .Select(GetById)
                .Where(item => item != null)
                .ToList();
        }

        /// <summary>
        /// Calculates the total value for a given <paramref name="statType"/> across the specified items.
        /// </summary>
        /// <param name="ids">A collection of item IDs to resolve.</param>
        /// <param name="statType">The <see cref="StatType"/> whose effect values should be summed.</param>
        /// <returns>
        /// The sum of all <see cref="AbilityEffect.Value"/> where <see cref="AbilityEffect.Stat"/>
        /// matches the provided <paramref name="statType"/>.
        /// </returns>
        public int GetTotalValueForStat(IEnumerable<string> ids, StatType statType)
        {
            return ids
                .Select(id => GetById(id))
                .Where(item => item != null)
                .SelectMany(item => item.GetAbility.Effects)
                .Where(effect => effect.Stat == statType)
                .Sum(effect => effect.Value);
        }

        private void OnEnable()
        {
            ServiceLocator.Register<ItemResolver>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<ItemResolver>();
        }
    }
}
