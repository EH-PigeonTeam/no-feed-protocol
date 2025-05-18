using System.Collections.Generic;
using NoFeelProtocol.Runtime.Data.Characters;
using NoFeelProtocol.Runtime.Data.Items;

namespace NoFeelProtocol.Runtime.Data.Save
{
    /// <summary>
    /// Runtime structure representing the full player state during gameplay.
    /// Populated at run start from saved or selected data.
    /// </summary>
    public class RuntimePlayerData
    {
        #region Properties --------------------------------------------------

        public CharacterData CharacterTop { get; }
        public CharacterData CharacterBottom { get; }

        public int CurrentShield { get; set; }
        public int Coins { get; set; }
        public int CurrentColumnIndex { get; set; }

        public List<Item> OwnedItems { get; } = new();

        #endregion

        #region Constructor -------------------------------------------------

        public RuntimePlayerData(
            CharacterData characterTop,
            CharacterData characterBottom,
            int shield,
            int coins,
            int columnIndex)
        {
            this.CharacterTop = characterTop;
            this.CharacterBottom = characterBottom;
            this.CurrentShield = shield;
            this.Coins = coins;
            this.CurrentColumnIndex = columnIndex;
        }

        #endregion
    }
}
