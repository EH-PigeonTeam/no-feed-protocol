using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoFeedProtocol.Authoring.Map;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Runtime.Logic.Map;
using NoFeedProtocol.Runtime.UI.Utilities;

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

        foreach (var node in nodes)
        {
            Vector2 anchoredPos = GetNodeLocalPosition(node.Position, cellWidth, cellHeight) + offset;

            var instance = UnityEngine.Object.Instantiate(reference.NodePrefab, reference.NodeParent);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPos;

            var encounter = Array.Find(encounters, e => e.Id == node.Id);
            if (encounter != null && instance.TryGetComponent(out Image img))
                img.sprite = encounter.Icon;

            foreach (var target in node.Connections)
            {
                Vector2 to = GetNodeLocalPosition(target, cellWidth, cellHeight) + offset;

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
        if (lastNode == null)
            return current.X == 0;

        var previous = nodes.Find(n => n.Position.Equals(lastNode.Value));
        return previous != null && previous.Connections.Contains(current);
    }

    private static Vector2 GetNodeLocalPosition(GridPosition pos, float cellWidth, float cellHeight)
    {
        float x = (cellWidth * pos.X) + cellWidth / 2f;
        float y = (cellHeight * pos.Y) + cellHeight / 2f;
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
