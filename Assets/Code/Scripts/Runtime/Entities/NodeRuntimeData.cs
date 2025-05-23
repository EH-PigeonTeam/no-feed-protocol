using NoFeedProtocol.Persistence.Map;
using System.Collections.Generic;

namespace NoFeedProtocol.Runtime.Entities
{
    [System.Serializable]
    public class NodeRuntimeData
    {
        public string Id;
        public GridPosition Position;
        public List<GridPosition> Connections;

        public NodeSaveData ToSaveData()
        {
            return new NodeSaveData
            {
                Id = Id,
                Position = Position,
                Connections = Connections
            };
        }

        public static NodeRuntimeData FromSaveData(NodeSaveData save)
        {
            return new NodeRuntimeData
            {
                Id = save.Id,
                Position = save.Position,
                Connections = save.Connections
            };
        }
    }
}