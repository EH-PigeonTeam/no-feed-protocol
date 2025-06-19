using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NoFeedProtocol.Persistence.Map;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Entities
{
    [Serializable]
    public class MapRuntimeData
    {
        [SerializeField]
        private GridPosition? m_lastNode = null;
        public List<NodeRuntimeData> Nodes = new(); 
        public bool LastNodeCompleted = false;

        [ShowInInspector]
        public bool HasLastNode => LastNode.HasValue;
        public int LastNodeIndex => Nodes[^1].Position.X;

        public GridPosition? LastNode
        {
            get => m_lastNode;
            set => m_lastNode = value;
        }

        public MapSaveData ToSaveData()
        {
            var nodeToSave = LastNode.HasValue && LastNode.Value.X == 0 && LastNode.Value.Y == 0
                ? (GridPosition?)null
                : LastNode;

            Debug.Log($"Saving map with last node {nodeToSave}");

            return new MapSaveData
            {
                LastNode = new OptionalGridPosition(nodeToSave),
                Nodes = Nodes.Select(n => n.ToSaveData()).ToList(),
                LastNodeCompleted = LastNodeCompleted
            };
        }

        public static MapRuntimeData FromSaveData(MapSaveData save)
        {
            return new MapRuntimeData
            {
                LastNode = save.LastNode.ToNullable,
                Nodes = save.Nodes.Select(NodeRuntimeData.FromSaveData).ToList(),
                LastNodeCompleted = save.LastNodeCompleted
            };
        }
    }
}
