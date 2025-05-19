using System.Linq;
using NoFeedProtocol.Runtime.Data.Save;
using NoFeedProtocol.Runtime.Data.Characters;
using NoFeedProtocol.Runtime.Data.Items;

namespace NoFeedProtocol.Runtime.Logic.Conversion
{
    /// <summary>
    /// Converts player runtime data into savable format and vice versa.
    /// </summary>
    public static class PlayerDataConverter
    {
        /// <summary>
        /// Converts a runtime player into a save structure.
        /// </summary>
        public static PlayerSaveData ToSaveData(RuntimePlayerData runtime)
        {
            return new PlayerSaveData(
                runtime.CharacterTop.Id,
                runtime.CharacterBottom.Id,
                runtime.CurrentShield,
                runtime.Coins,
                runtime.CurrentColumnIndex,
                runtime.OwnedItems.Select(i => i.Id).ToList()
            );
        }

        /// <summary>
        /// Converts save data into a full runtime structure.
        /// </summary>
        public static RuntimePlayerData ToRuntimeData(
            PlayerSaveData save,
            CharactersData characterDatabase,
            ItemsData itemsDatabase)
        {
            CharacterData characterTop = characterDatabase.GetById(save.CharacterTopID);
            CharacterData characterBottom = characterDatabase.GetById(save.CharacterBottomID);

            RuntimePlayerData runtime = new RuntimePlayerData(
                characterTop,
                characterBottom,
                save.ShieldValue,
                save.Coins,
                save.CurrentColumnIndex
            );

            runtime.OwnedItems.AddRange(
                itemsDatabase.Items.Where(i => save.OwnedItemIDs.Contains(i.Id))
            );

            return runtime;
        }
    }
}
