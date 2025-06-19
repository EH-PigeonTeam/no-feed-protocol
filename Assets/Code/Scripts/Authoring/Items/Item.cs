using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Items.Abilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NoFeedProtocol.Authoring.Items
{
    [Serializable]
    public class Item
    {
        #region Unique ID ---------------------------------------------------

        [FoldoutGroup("@m_name")]
        [SerializeField, ReadOnly]
        private string m_id;

        /// <summary>
        /// Unique, non-editable ID used for lookup and save data.
        /// </summary>
        public string Id => m_id;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(m_id))
            {
                m_id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(Selection.activeObject);
            }
        }
#endif

        #endregion

        #region Basic Info --------------------------------------------------

        [FoldoutGroup("@m_name")]
        [Tooltip("The name of the item")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The icon of the item")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("The description of the item")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, Range(0, 1)]
        private float m_percent = 1f;

        [FoldoutGroup("@m_name")]
        [Tooltip("The rarity of the item")]
        [SerializeField]
        private ItemRarity m_rarity;

        [FoldoutGroup("@m_name")]
        [Tooltip("The price of the item (for sale in the shop)"), SuffixLabel("Valore", SdfIconType.CurrencyBitcoin)]
        [SerializeField, MinValue(0)]
        private int m_price = 0;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, InlineProperty, HideLabel]
        private ItemAbility ability;

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public float Percent => m_percent;
        public int Price => m_price;
        public ItemRarity Rarity => m_rarity;
        public ItemAbility GetAbility => ability;

        #endregion
    }
}
