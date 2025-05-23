using System;
using System.Collections.Generic;
using NoFeedProtocol.Authoring.Map;
using NoFeedProtocol.Runtime.Entities;
using UnityEngine;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    public static class MapGenerator
    {
        public static NodeRuntimeData[,] Generate(MapStrutcture structure, MapReference reference, EncountersData encountersData)
        {
            int columns = structure.Columns;
            int[] rowsPerColumn = new int[columns];
            NodeRuntimeData[,] nodes;

            // Step 1: Determine the number of rows for each column
            for (int x = 0; x < columns; x++)
            {
                if (x == 0)
                    rowsPerColumn[x] = structure.Start.Rows;
                else if (x == columns - 1)
                    rowsPerColumn[x] = structure.End.Rows;
                else
                    rowsPerColumn[x] = UnityEngine.Random.Range(structure.Rows.x, structure.Rows.y + 1);
            }

            // Step 2: Allocate array
            int maxRows = 0;
            foreach (int r in rowsPerColumn)
                if (r > maxRows) maxRows = r;

            nodes = new NodeRuntimeData[columns, maxRows];

            // Step 3: Prepare encounter pool
            List<EncounterData> encounters = new(encountersData.Encounters);
            float totalWeight = 0f;
            foreach (var e in encounters)
                totalWeight += e.Percentage;

            // Step 4: Generate all nodes
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rowsPerColumn[x]; y++)
                {
                    EncounterData selectedEncounter = x switch
                    {
                        0 => SelectEncounterByType(encounters, structure.Start.Type),
                        var c when c == columns - 1 => SelectEncounterByType(encounters, structure.End.Type),
                        _ => SelectEncounterWeighted(encounters, totalWeight)
                    };

                    nodes[x, y] = new NodeRuntimeData
                    {
                        Id = selectedEncounter.Id,
                        Position = new GridPosition(x, y),
                        Connections = new List<GridPosition>()
                    };
                }
            }

            // Step 5: Generate connections forward
            HashSet<GridPosition> reachable = new();

            for (int x = 0; x < columns - 1; x++)
            {
                int currHeight = rowsPerColumn[x];
                int nextHeight = rowsPerColumn[x + 1];

                for (int y = 0; y < currHeight; y++)
                {
                    var current = nodes[x, y];
                    int connectionCount = UnityEngine.Random.Range(1, Mathf.Min(3, nextHeight) + 1);

                    HashSet<int> targets = new();
                    while (targets.Count < connectionCount)
                    {
                        int targetY = UnityEngine.Random.Range(0, nextHeight);
                        if (targets.Add(targetY))
                        {
                            var targetPos = new GridPosition(x + 1, targetY);
                            current.Connections.Add(targetPos);
                            reachable.Add(targetPos);
                        }
                    }
                }
            }

            // Step 6: Ensure every node (except column 0) is reachable
            for (int x = 1; x < columns; x++)
            {
                for (int y = 0; y < rowsPerColumn[x]; y++)
                {
                    var pos = new GridPosition(x, y);
                    if (!reachable.Contains(pos))
                    {
                        // Force-link from a random node in previous column
                        int prevY = UnityEngine.Random.Range(0, rowsPerColumn[x - 1]);
                        nodes[x - 1, prevY].Connections.Add(pos);
                        reachable.Add(pos);
                    }
                }
            }

            return nodes;
        }

        private static EncounterData SelectEncounterByType(List<EncounterData> encounters, EncounterType type)
        {
            List<EncounterData> filtered = encounters.FindAll(e => e.Type == type);
            if (filtered.Count == 0)
                throw new Exception($"No encounters found for type {type}");
            return filtered[UnityEngine.Random.Range(0, filtered.Count)];
        }

        private static EncounterData SelectEncounterWeighted(List<EncounterData> encounters, float totalWeight)
        {
            float rand = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var e in encounters)
            {
                cumulative += e.Percentage;
                if (rand <= cumulative)
                    return e;
            }

            return encounters[^1]; // Fallback
        }
    }
}
