using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using NoFeedProtocol.Shared.Data.Run;
using NoFeedProtocol.Runtime.Data.Run;
using System;
using Random = UnityEngine.Random;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [HideMonoScript]
    public class MapGenerator : MonoBehaviour
    {
        #region Configuration ----------------------------------------------

        [BoxGroup("Configuration"), SerializeField, Required, InlineEditor]
        private EncountersData m_encounterDB;

        [BoxGroup("Configuration"), SerializeField, Required, AssetsOnly]
        private GameObject m_nodePrefab;

        [BoxGroup("Configuration"), SerializeField, Required]
        private RectTransform m_container;

        [BoxGroup("Configuration"), SerializeField]
        private Boundary m_startNodes;

        [BoxGroup("Configuration"), SerializeField]
        private Boundary m_endNodes;

        [BoxGroup("Configuration"), SerializeField, MinMaxSlider(1, 10)]
        private Vector2Int m_intermediateNodeRange = new(2, 6);

        [BoxGroup("Configuration"), SerializeField, Range(3, 20)]
        private int m_totalColumns = 5;

        [FoldoutGroup("Configuration/Appearance"), SerializeField]
        private float m_spacingX = 300f;

        [FoldoutGroup("Configuration/Appearance"), SerializeField]
        private float m_spacingY = 200f;

        #endregion

        #region Public API -------------------------------------------------

        /// <summary>
        /// Generates and returns a 2D map of connected RunNodeStates.
        /// </summary>
        public RunNodeState[,] GenerateMap()
        {
            var jaggedMap = GenerateJaggedMap();
            ConnectNodes(jaggedMap);
            FilterUnreachableNodes(jaggedMap);

            ValidateMap(jaggedMap);

            return ConvertTo2D(jaggedMap);
        }

        #endregion

        #region Generation Steps -------------------------------------------

        private RunNodeState[][] GenerateJaggedMap()
        {
            var map = new RunNodeState[m_totalColumns][];

            for (int col = 0; col < m_totalColumns; col++)
            {
                int count = GetNodeCountForColumn(col);
                map[col] = new RunNodeState[count];

                EncounterType? forcedType = GetForcedType(col);

                for (int row = 0; row < count; row++)
                {
                    var def = GetRandomDefinition(forcedType);
                    var pos = new Vector3(col, row, 0f);
                    var nodeGO = Instantiate(m_nodePrefab, m_container);

                    if (!nodeGO.TryGetComponent(out RunNodeState node))
                    {
                        Debug.LogError("[MapGenerator] Node prefab missing RunNodeState.");
                        Destroy(nodeGO);
                        continue;
                    }

                    node.Initialize(def.SceneName, def.Icon, pos);
                    map[col][row] = node;

                    PositionNode(nodeGO.GetComponent<RectTransform>(), col, row, count);
                }
            }

            return map;
        }

        private void ConnectNodes(RunNodeState[][] map)
        {
            for (int col = 0; col < map.Length - 1; col++)
            {
                var fromNodes = map[col];
                var toNodes = map[col + 1];

                foreach (var from in fromNodes)
                {
                    if (from == null) continue;

                    int connectionCount = Random.Range(1, Mathf.Min(3, toNodes.Length + 1));
                    var targets = GetRandomTargets(toNodes, connectionCount);
                    from.SetConnections(targets);
                }
            }
        }

        private void FilterUnreachableNodes(RunNodeState[][] map)
        {
            var reachable = new HashSet<RunNodeState>();

            for (int col = 0; col < map.Length - 1; col++)
            {
                foreach (var node in map[col])
                {
                    if (node?.Nodes == null) continue;
                    foreach (var target in node.Nodes)
                        if (target != null) reachable.Add(target);
                }
            }

            for (int col = 1; col < map.Length; col++)
            {
                for (int i = 0; i < map[col].Length; i++)
                {
                    var node = map[col][i];
                    if (node != null && !reachable.Contains(node))
                    {
                        Destroy(node.gameObject);
                        map[col][i] = null;
                    }
                }
            }

            foreach (var col in map)
            {
                foreach (var node in col)
                {
                    if (node == null || node.Nodes == null)
                        continue;

                    node.SetConnections(node.Nodes.FindAll(n => n != null));
                }
            }
        }

        private void ValidateMap(RunNodeState[][] map)
        {
            if (map[0].Length == 0 || map[m_totalColumns - 1].Length == 0)
                throw new InvalidOperationException("Map generation failed: empty start or end nodes.");
        }

        private RunNodeState[,] ConvertTo2D(RunNodeState[][] map)
        {
            int maxRows = 0;
            foreach (var col in map)
                maxRows = Mathf.Max(maxRows, col.Length);

            var result = new RunNodeState[m_totalColumns, maxRows];
            for (int col = 0; col < m_totalColumns; col++)
            {
                for (int row = 0; row < map[col].Length; row++)
                    result[col, row] = map[col][row];
            }
            return result;
        }

        #endregion

        #region Utilities --------------------------------------------------

        private int GetNodeCountForColumn(int col)
        {
            if (col == 0) return m_startNodes.Count;
            if (col == m_totalColumns - 1) return m_endNodes.Count;
            return Random.Range(m_intermediateNodeRange.x, m_intermediateNodeRange.y + 1);
        }

        private EncounterType? GetForcedType(int col)
        {
            if (col == 0) return m_startNodes.Type;
            if (col == m_totalColumns - 1) return m_endNodes.Type;
            return null;
        }

        private EncounterDefinition GetRandomDefinition(EncounterType? requiredType)
        {
            var pool = m_encounterDB.Encounters;

            if (requiredType.HasValue)
            {
                var filtered = Array.FindAll(pool, e => e.Type == requiredType);
                if (filtered.Length > 0)
                    pool = filtered;
                else
                    Debug.LogWarning($"No encounters of type {requiredType} found. Using full pool.");
            }

            float totalWeight = 0f;
            foreach (var e in pool)
                totalWeight += e.Percentage;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var e in pool)
            {
                cumulative += e.Percentage;
                if (roll <= cumulative)
                    return e;
            }

            return pool[0]; // Fallback
        }

        private List<RunNodeState> GetRandomTargets(RunNodeState[] nodes, int count)
        {
            var indices = new HashSet<int>();
            while (indices.Count < count)
                indices.Add(Random.Range(0, nodes.Length));

            var result = new List<RunNodeState>();
            foreach (var i in indices)
                if (nodes[i] != null) result.Add(nodes[i]);

            return result;
        }

        private void PositionNode(RectTransform rect, int col, int row, int countInCol)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;

            var rectSize = m_container.rect;
            float cellWidth = rectSize.width / m_totalColumns;
            float cellHeight = rectSize.height / countInCol;

            float posX = (col + 0.5f) * cellWidth - rectSize.width * 0.5f;
            float posY = -((row + 0.5f) * cellHeight) + rectSize.height * 0.5f;

            rect.position = m_container.TransformPoint(new Vector3(posX, posY, 0f));
        }

        #endregion
    }

    [Serializable]
    public class Boundary
    {
        [BoxGroup("Configuration"), SerializeField, Range(1, 10)]
        private int m_count = 4;

        [BoxGroup("Configuration"), SerializeField]
        private EncounterType m_type;

        public int Count => m_count;
        public EncounterType Type => m_type;
    }
}
