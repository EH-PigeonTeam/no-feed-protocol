using NoFeedProtocol.Persistence.Run;
using System;

namespace NoFeedProtocol.Runtime.Entities
{
    [Serializable]
    public class RunRuntimeData
    {
        public PlayerRuntimeData Player;
        public MapRuntimeData Map;

        public RunSaveData ToSaveData()
        {
            return new RunSaveData
            {
                Player = Player?.ToSaveData(),
                Map = Map?.ToSaveData()
            };
        }

        public static RunRuntimeData FromSaveData(RunSaveData save)
        {
            return new RunRuntimeData
            {
                Player = PlayerRuntimeData.FromSaveData(save.Player),
                Map = MapRuntimeData.FromSaveData(save.Map)
            };
        }
    }
}
