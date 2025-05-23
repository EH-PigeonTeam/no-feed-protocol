using System;
using System.Collections.Generic;
using NoFeedProtocol.Persistence.Player;

namespace NoFeedProtocol.Runtime.Entities
{
    [Serializable]
    public class PlayerRuntimeData
    {
        public CharacterRuntimeData CharacterTop;
        public CharacterRuntimeData CharacterBottom;
        public int CurrentShield;
        public int Coins;
        public List<string> Items = new();

        public PlayerSaveData ToSaveData()
        {
            return new PlayerSaveData
            {
                CharacterTop = CharacterTop.ToSaveData(),
                CharacterBottom = CharacterBottom.ToSaveData(),
                Shield = CurrentShield,
                Coins = Coins,
                OwnedItemIDs = new List<string>(Items)
            };
        }

        public static PlayerRuntimeData FromSaveData(PlayerSaveData save)
        {
            return new PlayerRuntimeData
            {
                CharacterTop = CharacterRuntimeData.FromSaveData(save.CharacterTop),
                CharacterBottom = CharacterRuntimeData.FromSaveData(save.CharacterBottom),
                CurrentShield = save.Shield,
                Coins = save.Coins,
                Items = new List<string>(save.OwnedItemIDs)
            };
        }
    }
}
