using System;
using System.Collections.Generic;

namespace NoFeedProtocol.Persistence.Map
{
    [Serializable]
    public class MapSaveData
    {
        public OptionalGridPosition LastNode;
        public List<NodeSaveData> Nodes;
    }
}