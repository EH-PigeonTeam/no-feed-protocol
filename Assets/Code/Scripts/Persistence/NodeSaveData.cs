using System;
using System.Collections.Generic;

namespace NoFeedProtocol.Persistence.Map
{
    [Serializable]
    public class NodeSaveData
    {
        public string Id;
        public GridPosition Position;
        public List<GridPosition> Connections;
    }
}
