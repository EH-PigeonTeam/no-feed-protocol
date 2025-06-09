using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using NoFeedProtocol.Authoring.Items;

namespace NoFeedProtocol.Runtime.Logic.Loot
{
    [HideMonoScript]
    public class LootGeneratorComponent : MonoBehaviour
    {
        #region Fields and Configuration -------------------

        [BoxGroup("Coins")]
        [Tooltip("Range of coins to drop")]
        [MinMaxSlider(0, 1000, true)]
        [SerializeField]
        private Vector2Int m_coinsRange = new Vector2Int(10, 100);

        [BoxGroup("Items")]
        [Tooltip("Number of items to drop")]
        [MinValue(0)]
        [SerializeField]
        private int m_itemDropCount = 1;

        [BoxGroup("Items")]
        [Tooltip("Item data source")]
        [SerializeField]
        private ItemsData m_itemsData;

        [BoxGroup("Rarity Weight Ranges")]
        [Tooltip("Weight range for Common rarity (at level 0 and max) ")]
        [MinMaxSlider(0f, 1f, true)]
        [SerializeField]
        private Vector2 m_commonWeightRange = new Vector2(1f, 0.5f);

        [BoxGroup("Rarity Weight Ranges")]
        [Tooltip("Weight range for Uncommon rarity")]
        [MinMaxSlider(0f, 1f, true)]
        [SerializeField]
        private Vector2 m_uncommonWeightRange = new Vector2(0f, 0.3f);

        [BoxGroup("Rarity Weight Ranges")]
        [Tooltip("Weight range for Rare rarity")]
        [MinMaxSlider(0f, 1f, true)]
        [SerializeField]
        private Vector2 m_rareWeightRange = new Vector2(0f, 0.15f);

        [BoxGroup("Rarity Weight Ranges")]
        [Tooltip("Weight range for Legendary rarity")]
        [MinMaxSlider(0f, 1f, true)]
        [SerializeField]
        private Vector2 m_legendaryWeightRange = new Vector2(0f, 0.05f);

        [BoxGroup("Rarity Weight Ranges")]
        [Tooltip("Include OnlyInShop items when in shop mode?")]
        [SerializeField]
        private bool m_includeOnlyInShop = false;
        #endregion

        #region Methods and Generation --------------------

        /// <summary>
        /// Generates loot for the current run.
        /// </summary>
        /// <param name="currentLevel">Current level index (0-based)</param>
        /// <param name="maxLevel">Maximum number of levels in run</param>
        /// <param name="existingItemIds">IDs of items already owned by player</param>
        /// <returns>LootResult containing coins and item drops</returns>
        public LootResult GenerateLoot(int currentLevel, int maxLevel, IEnumerable<string> existingItemIds)
        {
            int coins = UnityEngine.Random.Range(m_coinsRange.x, m_coinsRange.y + 1);
            HashSet<string> existingSet = new(existingItemIds);
            List<Item> pool = new();

            foreach (var item in m_itemsData.Items)
            {
                if (existingSet.Contains(item.Id))
                {
                    continue;
                }

                if (item.Rarity == ItemRarity.OnlyInShop && !m_includeOnlyInShop)
                {
                    continue;
                }

                float rarityWeight = GetRarityWeight(item.Rarity, currentLevel, maxLevel);
                float totalChance = item.Percent * rarityWeight;

                if (totalChance <= 0f)
                {
                    continue;
                }

                // Add multiple entries proportional to chance
                int entries = Mathf.CeilToInt(totalChance * 100);
                for (int i = 0; i < entries; i++)
                {
                    pool.Add(item);
                }
            }

            List<Item> drops = new();
            for (int i = 0; i < m_itemDropCount && pool.Count > 0; i++)
            {
                int idx = UnityEngine.Random.Range(0, pool.Count);
                Item selected = pool[idx];
                drops.Add(selected);

                // Remove all instances of this item for uniqueness
                pool.RemoveAll(x => x.Id == selected.Id);
            }

            return new LootResult(coins, drops);
        }

        private float GetRarityWeight(ItemRarity rarity, int currentLevel, int maxLevel)
        {
            float t = maxLevel > 0 ? (float)currentLevel / maxLevel : 0f;
            Vector2 range = rarity switch
            {
                ItemRarity.Common => m_commonWeightRange,
                ItemRarity.Uncommon => m_uncommonWeightRange,
                ItemRarity.Rare => m_rareWeightRange,
                ItemRarity.Leggendary => m_legendaryWeightRange,
                _ => Vector2.zero,
            };
            return Mathf.Lerp(range.x, range.y, t);
        }
        #endregion
    }

    /// <summary>
    /// Result of a loot generation process.
    /// </summary>
    public readonly struct LootResult
    {
        /// <summary>Number of coins awarded.</summary>
        public int Coins { get; }

        /// <summary>List of items awarded.</summary>
        public List<Item> Items { get; }

        public LootResult(int coins, List<Item> items)
        {
            Coins = coins;
            Items = items;
        }
    }
}
