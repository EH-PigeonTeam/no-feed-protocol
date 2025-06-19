using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using NoFeedProtocol.Authoring.Items;
using Code.Systems.Locator;

namespace NoFeedProtocol.Runtime.Logic.Shop.UI
{
    [HideMonoScript]
    public class ItemShopViewer : MonoBehaviour
    {
        #region Public Members ----------------------------------------------

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private Image m_background;

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private Image m_icon;

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private TMP_Text m_price;

        [FoldoutGroup("References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly]
        private GameObject m_lock;

        [BoxGroup("Setup")]
        [Tooltip("")]
        [SerializeField]
        private Color m_colorSelected = Color.red;

        #endregion

        #region Private Members ---------------------------------------------

        private bool m_isSelected = false;

        private Color m_defaultColor;

        private Item m_item;
        private ShopKeeper m_shopKeeper;

        #endregion

        #region Initialization ----------------------------------------------

        private void Start()
        {
            if (m_icon == null) return;

            m_defaultColor = m_icon.color;
        }

        private void OnEnable()
        {
            ShopKeeper.OnChange += CanBuyable;
        }

        private void OnDisable()
        {
            ShopKeeper.OnChange -= CanBuyable;
        }

        private void CanBuyable()
        {
            if (m_isSelected) return;

            bool canBuy = m_shopKeeper.HasCash(m_item.Price);

            m_lock.SetActive(!canBuy);
            GetComponent<Button>().interactable = canBuy;
        }

        #endregion

        #region Public Methods ----------------------------------------------

        public void SetItem(Item item)
        {
            m_shopKeeper = ServiceLocator.Get<ShopKeeper>();

            m_item = item;
            m_icon.sprite = item.Icon;
            m_price.text = item.Price.ToString();
        }

        public void ToggleSelection()
        {
            if (m_isSelected)
            {
                Deselected();
            }
            else
            {
                Selected();
            }

            m_background.color = m_isSelected ? m_colorSelected : m_defaultColor;
        }

        public void Selected()
        {
            ShowDescription();

            m_isSelected = true;
            m_shopKeeper.AddItem(m_item);
        }

        public void Deselected()
        {
            m_isSelected = false;
            m_shopKeeper.RemoveItem(m_item);
        }

        public void ShowDescription()
        {
            m_shopKeeper.ShowDescription(m_item.Description);
        }

        #endregion

    }
}
