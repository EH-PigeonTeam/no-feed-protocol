using System;
using System.Collections.Generic;
using NoFeedProtocol.Persistence.Run;

namespace NoFeedProtocol.Persistence.Game
{
    [Serializable]
    public class GameSaveData
    {
        public RunSaveData Run;
        public List<string> ItemIDsUnlocked;
    }
}
