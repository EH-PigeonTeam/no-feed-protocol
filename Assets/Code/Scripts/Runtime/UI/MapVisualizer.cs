using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoFeedProtocol.Authoring.Map;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Map;
using NoFeedProtocol.Runtime.UI.Utilities;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;

public static class MapVisualizer
{
    public static Dictionary<GridPosition, ButtonAudio> Initialize(
        List<NodeRuntimeData> nodes,
        MapStrutcture structure,
        MapReference reference,
        EncounterData[] encounters,
        GridPosition? lastNode)
    {
        RectTransform parentRect = reference.NodeParent.GetComponent<RectTransform>();
        Vector2 parentSize = parentRect.rect.size;

        int columns = structure.Columns;
        int maxRows = GetMaxRowCount(nodes);

        float cellWidth = parentSize.x / columns;
        float cellHeight = parentSize.y / maxRows;

        // NEW: Calcolo offset per centrare la griglia nel parent (pivot 0.5,0.5)
        Vector2 gridSize = new Vector2(cellWidth * columns, cellHeight * maxRows);
        Vector2 offset = -gridSize / 2f;

        Dictionary<GridPosition, ButtonAudio> activeButtons = new();

        Dictionary<int, int> columnHeights = new();
        foreach (var node in nodes)
        {
            if (!columnHeights.ContainsKey(node.Position.X))
                columnHeights[node.Position.X] = 0;

            columnHeights[node.Position.X]++;
        }

        Dictionary<int, int> columnOffsets = new();
        foreach (var kvp in columnHeights)
        {
            int x = kvp.Key;
            int height = kvp.Value;
            int offsett = (maxRows - height) / 2;
            columnOffsets[x] = offsett;
        }

        foreach (var node in nodes)
        {
            int yOffset = columnOffsets[node.Position.X];
            Vector2 anchoredPos = GetNodeLocalPosition(node.Position, cellWidth, cellHeight, yOffset) + offset;

            var instance = UnityEngine.Object.Instantiate(reference.NodePrefab, reference.NodeParent);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPos;

            var encounter = Array.Find(encounters, e => e.Id == node.Id);
            if (encounter != null && instance.TryGetComponent(out Image img))
                img.sprite = encounter.Icon;

            foreach (var target in node.Connections)
            {
                int toYOffset = columnOffsets[target.X];
                Vector2 to = GetNodeLocalPosition(target, cellWidth, cellHeight, toYOffset) + offset;

                var line = UnityEngine.Object.Instantiate(reference.ConnectionPrefab, reference.ConnectionParent);
                var uiLine = line.GetComponent<UILineRenderer>();
                uiLine.Points.Add(anchoredPos);
                uiLine.Points.Add(to);
                uiLine.SetAllDirty();
            }

            bool isActive = IsNodeActive(lastNode, node.Position, nodes);

            var buttonAudio = instance.GetComponent<ButtonAudio>();
            buttonAudio.interactable = isActive;

            if (isActive)
                activeButtons[node.Position] = buttonAudio;
        }

        return activeButtons;
    }

    private static bool IsNodeActive(GridPosition? lastNode, GridPosition current, List<NodeRuntimeData> nodes)
    {
        if (!lastNode.HasValue)
            return current.X == 0;

        var map = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Map;

        if (!map.LastNodeCompleted)
            return current.Equals(map.LastNode);

        var previous = nodes.Find(n => n.Position.Equals(map.LastNode));
        return previous != null && previous.Connections.Contains(current);
    }

    private static Vector2 GetNodeLocalPosition(GridPosition pos, float cellWidth, float cellHeight, int verticalOffset)
    {
        float x = (cellWidth * pos.X) + cellWidth / 2f;
        float y = (cellHeight * (pos.Y + verticalOffset)) + cellHeight / 2f;
        return new Vector2(x, y);
    }

    private static int GetMaxRowCount(List<NodeRuntimeData> nodes)
    {
        int max = 0;
        foreach (var node in nodes)
            if (node.Position.Y + 1 > max)
                max = node.Position.Y + 1;
        return max;
    }
}
