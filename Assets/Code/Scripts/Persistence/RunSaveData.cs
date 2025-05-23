using System;
using NoFeedProtocol.Persistence.Player;
using NoFeedProtocol.Persistence.Map;

namespace NoFeedProtocol.Persistence.Run
{
    [Serializable]
    public class RunSaveData
    {
        public PlayerSaveData Player;
        public MapSaveData Map;
    }
}

