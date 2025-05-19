using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Data.Run;
using NoFeedProtocol.Runtime.UI.Map;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Data.Save;
using Code.Systems.LoadingScene;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [HideMonoScript]
    public class MapManager : MonoBehaviour
    {
        #region Inspector References ----------------------------------------

        [BoxGroup("References")]
        [Tooltip("Generatore della mappa dei nodi")]
        [SerializeField, Required]
        private MapGenerator m_generator;

        [BoxGroup("References")]
        [Tooltip("Componente responsabile della visualizzazione della mappa")]
        [SerializeField, Required]
        private MapVisualizer m_visualizer;

        #endregion

        #region Runtime State -----------------------------------------------

        [ShowInInspector, ReadOnly]
        private RunContext m_runContext;

        #endregion

        #region Unity Lifecycle ---------------------------------------------

        private void Start()
        {
            InitializeServices();

            if (!HasValidMap())
            {
                GenerateAndAssignRunData();
            }

            DrawAndBindNodes(m_runContext.RunData.Nodes);
        }

        #endregion

        #region Initialization ----------------------------------------------

        private void InitializeServices()
        {
            m_runContext = ServiceLocator.Get<RunContext>();
        }

        private bool HasValidMap()
        {
            return m_runContext.HasValidRun && m_runContext.RunData.Nodes != null;
        }

        #endregion

        #region Map Generation ----------------------------------------------

        private void GenerateAndAssignRunData()
        {
            RunNodeState[,] jaggedMap = m_generator.GenerateMap();

            RuntimePlayerData playerData = m_runContext.RunData?.Player;
            var newRun = new RunData(playerData, 0, jaggedMap);

            m_runContext.SetRunData(newRun);
        }

        #endregion

        #region Map Setup --------------------------------------------------

        private void DrawAndBindNodes(RunNodeState[,] nodes)
        {
            m_visualizer.DrawConnections(nodes);

            int cols = nodes.GetLength(0);
            int rows = nodes.GetLength(1);

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    RunNodeState node = nodes[x, y];
                    if (node == null)
                        continue;

                    node.OnClicked += () => HandleNodeClick(node);
                }
            }
        }

        #endregion

        #region Node Interaction -------------------------------------------

        private void HandleNodeClick(RunNodeState node)
        {
            ServiceLocator.Get<SceneManager>().LoadScene(node.SceneName);
        }

        #endregion
    }
}
