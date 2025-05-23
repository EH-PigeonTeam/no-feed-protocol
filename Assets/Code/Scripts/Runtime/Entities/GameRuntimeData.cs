using System.Collections.Generic;
using UnityEngine;
using NoFeedProtocol.Persistence.Game;
using NoFeedProtocol.Runtime.Save;

namespace NoFeedProtocol.Runtime.Entities
{
    [System.Serializable]
    public class GameRuntimeData : ISaveable<GameSaveData>
    {
        public RunRuntimeData Run;
        public List<string> ItemIDsUnlocked;

        public GameSaveData ToSaveData()
        {
            return new GameSaveData
            {
                Run = Run.ToSaveData(),
                ItemIDsUnlocked = ItemIDsUnlocked
            };
        }

        public void FromSaveData(GameSaveData save)
        {
            Run = RunRuntimeData.FromSaveData(save.Run);
            ItemIDsUnlocked = save.ItemIDsUnlocked;
        }
    }
}