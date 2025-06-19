using System;
using System.Collections.Generic;
using System.Linq;
using NoFeedProtocol.Authoring.Items;
using NoFeedProtocol.Persistence.Player;
using Unity.VisualScripting;

namespace NoFeedProtocol.Runtime.Entities
{
    [Serializable]
    public class PlayerRuntimeData
    {
        public CharacterRuntimeData CharacterTop;
        public CharacterRuntimeData CharacterBottom;
        public int MaxShield;
        public int CurrentShield;
        public int Coins;
        public List<string> Items = new();

        public PlayerSaveData ToSaveData()
        {
            return new PlayerSaveData
            {
                CharacterTop = CharacterTop.ToSaveData(),
                CharacterBottom = CharacterBottom.ToSaveData(),
                MaxShield = MaxShield,
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
                MaxShield = save.MaxShield,
                CurrentShield = save.Shield,
                Coins = save.Coins,
                Items = new List<string>(save.OwnedItemIDs)
            };
        }

        public bool CharactersAreAlive()
        {
            return CharacterTop.IsAlive || CharacterBottom.IsAlive;
        }

        public void AddItems(IEnumerable<Item> items) => Items.AddRange(items.Select(i => i.Id));
        public void AddCoins(int coins) => Coins += coins;

        public PlayerRuntimeData Clone() => new PlayerRuntimeData 
        {
            CharacterTop = CharacterTop.Clone(),
            CharacterBottom = CharacterBottom.Clone(),
            MaxShield = MaxShield,
            CurrentShield = CurrentShield,
            Coins = Coins,
            Items = new List<string>(Items)
        };
    }
}
