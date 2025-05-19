using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Data.Characters;
using NoFeedProtocol.Runtime.Data.Items;

namespace NoFeedProtocol.Runtime.Data.Save
{
    [Serializable]
    [HideMonoScript]
    public class RuntimePlayerData
    {
        #region Serialized Fields --------------------------------------------

        [SerializeField, InlineProperty, LabelWidth(100)]
        private CharacterData m_characterTop;

        [SerializeField, InlineProperty, LabelWidth(100)]
        private CharacterData m_characterBottom;

        [SerializeField]
        private int m_currentShield;

        [SerializeField]
        private int m_coins;

        [SerializeField]
        private int m_currentColumnIndex;

        [SerializeField]
        private List<Item> m_ownedItems = new();

        #endregion

        #region Public Properties --------------------------------------------

        public CharacterData CharacterTop => m_characterTop;
        public CharacterData CharacterBottom => m_characterBottom;

        public int CurrentShield
        {
            get => m_currentShield;
            set => m_currentShield = value;
        }

        public int Coins
        {
            get => m_coins;
            set => m_coins = value;
        }

        public int CurrentColumnIndex
        {
            get => m_currentColumnIndex;
            set => m_currentColumnIndex = value;
        }

        public List<Item> OwnedItems => m_ownedItems;

        #endregion

        #region Constructor --------------------------------------------------

        public RuntimePlayerData(
            CharacterData characterTop,
            CharacterData characterBottom,
            int shield,
            int coins,
            int columnIndex)
        {
            m_characterTop = characterTop;
            m_characterBottom = characterBottom;
            m_currentShield = shield;
            m_coins = coins;
            m_currentColumnIndex = columnIndex;
        }

        #endregion
    }
}
