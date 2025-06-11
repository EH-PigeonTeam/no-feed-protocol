using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Loot;
using NoFeedProtocol.Runtime.Logic.Shop.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoFeedProtocol.Runtime.Logic.Shop
{
    [HideMonoScript]
    public class ShopKeeper : MonoBehaviour
    {
        #region Fields -----------

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField]
        private LootGeneratorComponent m_lootGenerator;

        [FoldoutGroup("References/UI")]
        [Tooltip("")]
        [SerializeField]
        private TMP_Text m_price;

        [FoldoutGroup("References/UI")]
        [Tooltip("")]
        [SerializeField]
        private TMP_Text m_cash;

        [FoldoutGroup("References/UI")]
        [Tooltip("")]
        [SerializeField]
        private TMP_Text m_description;

        [FoldoutGroup("References/Prefab")]
        [Tooltip("")]
        [SerializeField, AssetsOnly]
        private GameObject m_itemPrefab;

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField, SceneObjectsOnly]
        private Transform m_itemsParent;

        [BoxGroup("Setup")]
        [Tooltip("")]
        [SerializeField, MinValue(0)]
        private int m_numberOfItems = 10;

        [Header("Debug")]
        public List<Item> ItemsSelected;
        public int Cash;
        public int Price;

        private RunRuntimeData Run;
        private PlayerRuntimeData Player => Run.Player;

        #endregion

        public static event Action OnChange;

        #region Init -------------

        private void Awake()
        {
            ServiceLocator.Register(this);

            Run = ServiceLocator.Get<RuntimeDataStore>().GameData.Run;

            SetCash(Player.Coins);
        }

        private void Start()
        {
            Init();

            OnChange?.Invoke();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ShopKeeper>();
        }

        #endregion

        #region Private Methods ----------

        private void Init()
        {
            if (m_lootGenerator == null) return;

            LootResult loot = m_lootGenerator.GenerateLoot(
                Run.Map.HasLastNode ? (Run.Map.LastNode.Value.X + 1) : 0,
                Run.Map.LastNodeIndex, 
                Player.Items,
                m_numberOfItems
                );

            foreach (Item item in loot.Items)
            {
                GameObject go = Instantiate(m_itemPrefab, m_itemsParent);
                go.GetComponent<ItemShopViewer>().SetItem(item);
            }
        }

        #endregion

        #region Public Methods -----------

        public bool HasCash(int cash) => (Cash - Price) - cash > 0f;

        public void AddItem(Item item)
        {
            ItemsSelected.Add(item);
            AddPrice(item.Price);

            OnChange?.Invoke();
        }

        public void RemoveItem(Item item)
        {
            if (ItemsSelected.Contains(item))
            {
                ItemsSelected.Remove(item);
                AddPrice(-item.Price);
            }

            OnChange?.Invoke();
        }

        public void ShowDescription(string description)
        {
            if (m_description == null) return;

            m_description.text = description;
        }

        [Button("Set Cash", ButtonSizes.Large)]
        public void SetCash(int cash)
        {
            Cash = cash;
            m_cash.text = Cash.ToString();
        }

        public void AddPrice(int price)
        {
            Price += price;
            m_price.text = Price.ToString();
        }

        #endregion

    }
}

