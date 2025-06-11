using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Runtime.UI;

namespace NoFeedProtocol.Runtime.Logic.Loot
{
    [HideMonoScript]
    public class LootViewer : MonoBehaviour
    {
        #region Fields and Configuration -------------------

        [BoxGroup("References")]
        [SerializeField]
        private LootGeneratorComponent m_lootGenerator;

        [BoxGroup("References")]
        [SerializeField, SceneObjectsOnly]
        private Transform m_lootParent;

        [BoxGroup("References")]
        [SerializeField, AssetsOnly]
        private Sprite m_coinSprite;

        [FoldoutGroup("References/Prefabs")]
        [Tooltip("")]
        [SerializeField, AssetsOnly]
        private GameObject m_coinsPrefab;

        [FoldoutGroup("References/Prefabs")]
        [Tooltip("")]
        [SerializeField, AssetsOnly]
        private GameObject m_itemPrefab;

        [Button("Test")]
        private void Show()
        {
            ShowLoot();
        }

        private LootResult loot;

        #endregion

        #region Methods and Generation --------------------

        public void ShowLoot()
        {
            RunRuntimeData run = ServiceLocator.Get<RuntimeDataStore>().GameData.Run;

            loot = m_lootGenerator.GenerateLoot(
                run.Map.HasLastNode ? (run.Map.LastNode.Value.X + 1) : 0,
                run.Map.LastNodeIndex,
                run.Player.Items
                );

            GameObject coin = Instantiate(m_coinsPrefab, m_lootParent);
            coin.GetComponent<LootGraphics>().SetItem(m_coinSprite, $"x{loot.Coins} Coins");

            foreach (Item item in loot.Items)
            {
                GameObject go = Instantiate(m_itemPrefab, m_lootParent);
                go.GetComponent<LootGraphics>().SetItem(item.Icon, item.Name);
            }
        }

        public void AddLoot()
        {
            PlayerRuntimeData player = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player;

            player.Coins += loot.Coins;
            player.AddItems(loot.Items);
        }

        #endregion

    }
}
