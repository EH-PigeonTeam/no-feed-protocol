using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Map;
using System.Collections.Generic;
using Code.Systems.LoadingScene;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [HideMonoScript]
    public class MapManager : MonoBehaviour
    {
        [SerializeField, InlineEditor]
        private EncountersData m_encounters;

        [SerializeField]
        private MapStrutcture m_structure;

        [SerializeField]
        private MapReference m_references;

        [SerializeField]
        private NodeRuntimeData[,] m_nodes;

        private RuntimeDataStore m_dataStore;

        private void Start()
        {
            m_dataStore = ServiceLocator.Get<RuntimeDataStore>();

            if (m_dataStore.GameData.Run.Map?.Nodes?.Count > 0)
            {
                Debug.Log("Loading existing map...");

                //m_nodes = m_dataStore.GameData.Run.Map.Nodes // convert back
            }
            else
            {
                m_nodes = MapGenerator.Generate(m_structure, m_references, m_encounters);

                m_dataStore.GameData.Run.Map.Nodes = Flatten(m_nodes);
            }

            DrawAndBindNodes();
        }

        public static List<NodeRuntimeData> Flatten(NodeRuntimeData[,] nodes)
        {
            List<NodeRuntimeData> result = new();

            int width = nodes.GetLength(0);
            int height = nodes.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var node = nodes[x, y];
                    if (node != null)
                        result.Add(node);
                }
            }

            return result;
        }

        #region Map Setup --------------------------------------------------

        private void DrawAndBindNodes()
        {
            var runtimeMap = m_dataStore.GameData.Run.Map;

            Dictionary<GridPosition, ButtonAudio> activeButtons = MapVisualizer.Initialize(
                runtimeMap.Nodes,
                m_structure,
                m_references,
                m_encounters.Encounters,
                runtimeMap.LastNode
            );

            foreach (var kvp in activeButtons)
            {
                GridPosition position = kvp.Key;
                ButtonAudio button = kvp.Value;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleNodeClick(position));
            }
        }

        #endregion

        #region Node Interaction -------------------------------------------

        private void HandleNodeClick(GridPosition position)
        {
            Debug.Log($"Clicked node at X: {position.X}, Y: {position.Y}");

            m_dataStore.GameData.Run.Map.LastNode = position;

            // load scene
            ServiceLocator.Get<ScenesManager>()
                .LoadScene(this.m_encounters.GetEncounter(this.m_nodes[position.X, position.Y].Id).SceneName);
        }

        #endregion
    }
}
