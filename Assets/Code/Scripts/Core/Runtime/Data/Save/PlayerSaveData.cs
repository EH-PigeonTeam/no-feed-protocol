using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Data.Save
{
    /// <summary>
    /// Compact runtime-saveable structure that holds all essential player data for saving and loading.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        #region Characters --------------------------------------------------

        [BoxGroup("Characters")]
        [Tooltip("Unique ID of the top character selected by the player.")]
        [SerializeField]
        private string m_characterTopID;

        [BoxGroup("Characters")]
        [Tooltip("Unique ID of the bottom character selected by the player.")]
        [SerializeField]
        private string m_characterBottomID;

        #endregion

        #region Stats -------------------------------------------------------

        [BoxGroup("Stats")]
        [Tooltip("Current team shared shield value.")]
        [SerializeField]
        private int m_shieldValue;

        [BoxGroup("Stats")]
        [Tooltip("Current amount of coins owned by the player.")]
        [SerializeField]
        private int m_coins;

        #endregion

        #region Progression -------------------------------------------------

        [BoxGroup("Progression")]
        [Tooltip("Current column index reached on the map (0-based).")]
        [SerializeField]
        private int m_currentColumnIndex;

        #endregion

        #region Inventory ---------------------------------------------------

        [BoxGroup("Inventory")]
        [Tooltip("List of unique item IDs currently owned by the player.")]
        [SerializeField]
        private List<string> m_ownedItemIDs = new();

        #endregion

        #region Constructors ------------------------------------------------

        public PlayerSaveData() { }

        public PlayerSaveData(string characterTopID, string characterBottomID, int shieldValue, int coins, int currentColumnIndex, List<string> ownedItemIDs)
        {
            this.m_characterTopID = characterTopID;
            this.m_characterBottomID = characterBottomID;
            this.m_shieldValue = shieldValue;
            this.m_coins = coins;
            this.m_currentColumnIndex = currentColumnIndex;
            this.m_ownedItemIDs = ownedItemIDs;
        }

        #endregion

        #region Public Properties -------------------------------------------

        public string CharacterTopID => this.m_characterTopID;
        public string CharacterBottomID => this.m_characterBottomID;
        public int ShieldValue => this.m_shieldValue;
        public int Coins => this.m_coins;
        public int CurrentColumnIndex => this.m_currentColumnIndex;
        public List<string> OwnedItemIDs => this.m_ownedItemIDs;

        #endregion
    }
}
