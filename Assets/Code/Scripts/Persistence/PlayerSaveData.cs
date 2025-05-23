using System;
using System.Collections.Generic;
using NoFeedProtocol.Persistence.Character;

namespace NoFeedProtocol.Persistence.Player
{
    [Serializable]
    public class PlayerSaveData
    {
        public CharacterSaveData CharacterTop;
        public CharacterSaveData CharacterBottom;

        public int Shield;

        public int Coins;

        public List<string> OwnedItemIDs = new();
    }
}
