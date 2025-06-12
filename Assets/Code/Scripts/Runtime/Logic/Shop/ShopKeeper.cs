using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Loot;
using NoFeedProtocol.Runtime.Logic.Shop.UI;
using DG.Tweening;


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

        [FoldoutGroup("References/UI/Description")]
        [Tooltip("")]
        [SerializeField]
        private TMP_Text m_description;

        [FoldoutGroup("References/UI/Description")]
        [Tooltip("")]
        [SerializeField, MinValue(0f)]
        private float m_descriptionFadeOut = 0.33f;

        [FoldoutGroup("References/UI/Description")]
        [Tooltip("")]
        [SerializeField, MinValue(0f)]
        private float m_descriptionFadeIn = 1f;

        [FoldoutGroup("References/UI/Description")]
        [Tooltip("")]
        [SerializeField, MinValue(0f)]
        private Ease m_descriptionEase = Ease.Linear;

        [FoldoutGroup("References/UI/Buy Button")]
        [Tooltip("")]
        [SerializeField]
        private TMP_Text m_textButtonBuy;

        [FoldoutGroup("References/UI/Buy Button")]
        [Tooltip("")]
        [SerializeField]
        private string m_textButtonBuyDefault = "Skip";

        [FoldoutGroup("References/UI/Buy Button")]
        [Tooltip("")]
        [SerializeField]
        private string m_textButtonBuySelected = "Buy";

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

        // Debug
        [FoldoutGroup("Debug")]
        [Tooltip("")]
        [ShowInInspector, ReadOnly]
        private List<Item> ItemsSelected = new();

        [FoldoutGroup("Debug")]
        [Tooltip("")]
        [ShowInInspector, ReadOnly]
        private int Cash;

        [FoldoutGroup("Debug")]
        [Tooltip("")]
        [ShowInInspector, ReadOnly]
        private int Price;

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

            m_textButtonBuy.text = m_textButtonBuyDefault;
        }

        private void Change(Item item, int mul = 1)
        {
            if (mul == -1)
            {
                ItemsSelected.Remove(item);
            }
            else
            {
                ItemsSelected.Add(item);
            }

            AddPrice(item.Price * mul);

            m_textButtonBuy.text = ItemsSelected.Count > 0 ? m_textButtonBuySelected : m_textButtonBuyDefault;

            OnChange?.Invoke();
        }

        #endregion

        #region Public Methods -----------

        public bool HasCash(int cash) => (Cash - Price) - cash > 0f;

        public void AddItem(Item item)
        {
            Change(item);
        }

        public void RemoveItem(Item item)
        {
            Change(item, -1);
        }

        public void ShowDescription(string description)
        {
            if (m_description == null)
            {
                return;
            }

            if (m_description.text == description)
            {
                return;
            }

            //Sequence seq = DOTween.Sequence();
            //seq.Append(m_description.rectTransform.DOAnchorPosY(-50f, 0.3f).SetRelative());
            //seq.AppendCallback(() => m_description.text = description);
            //seq.Append(m_description.rectTransform.DOAnchorPosY(50f, 0f).SetRelative());
            //seq.Append(m_description.rectTransform.DOAnchorPosY(0f, 0.3f).SetRelative());

            //m_description.DOText("", 0.3f).OnComplete(() =>
            //{
            //    m_description.DOText(description, 1f);
            //});

            Sequence seq = DOTween.Sequence();

            seq.Join(m_description.DOFade(0f, m_descriptionFadeOut));

            seq.AppendCallback(() =>
            {
                // Reset transform & text before type-in
                m_description.alpha = 1f;
                m_description.text = "";
            });

            // Enter animation: typewriter effect
            seq.Append(
                m_description.DOText(
                    description, 
                    m_descriptionFadeIn
                )
                .SetEase(m_descriptionEase)
            );

            //m_description.text = description;
        }

        [FoldoutGroup("Debug")]
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

        public void BuyItems()
        {
            Player.AddItems(ItemsSelected);
            Player.AddCoins(-Price);
        }

        #endregion

    }
}

