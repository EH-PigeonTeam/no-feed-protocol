using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Data.Save;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using NoFeedProtocol.Runtime.Logic.Conversion;
using NoFeedProtocol.Runtime.Data.Characters;
using NoFeedProtocol.Runtime.Data.Items;
using UnityEngine.UI;

namespace NoFeedProtocol.Runtime.Data.Run
{
    [HideMonoScript]
    public class RunNodeState : ButtonAudio
    {
        #region Fields -----------------------------------------------------

        private string m_sceneName;
        private List<RunNodeState> m_nodes;
        private Vector3 m_position;
        private Sprite m_icon;

        #endregion

        #region Events -----------------------------------------------------

        public event Action OnClicked;

        #endregion

        #region Initialization ---------------------------------------------

        public void Initialize(string sceneName, Sprite icon, Vector3 position)
        {
            this.m_sceneName = sceneName;
            this.m_position = position;
            this.m_icon = icon;

            this.GetComponent<Image>().sprite = this.m_icon;
        }

        #endregion

        #region Unity Events -----------------------------------------------

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnClicked?.Invoke();
        }

        #endregion

        #region Public API -------------------------------------------------

        public string SceneName => this.m_sceneName;
        public Vector3 Position => this.m_position;
        public Sprite Icon => this.m_icon;
        public List<RunNodeState> Nodes => this.m_nodes;

        public void SetConnections(List<RunNodeState> map)
        {
            this.m_nodes = map;
        }

        #endregion
    }

    [Serializable]
    public class RunData
    {
        #region Field --------------------------------------------

        [BoxGroup("Run")]
        [Tooltip("Player")]
        [ShowInInspector, ReadOnly]
        private RuntimePlayerData m_player;

        [BoxGroup("Run")]
        [Tooltip("The current column index")]
        [ShowInInspector, ReadOnly]
        private int m_currentColumnIndex;

        [BoxGroup("Run")]
        [Tooltip("Map of nodes")]
        [ShowInInspector, ReadOnly]
        private RunNodeState[,] m_nodes;

        #endregion

        #region Constructor --------------------------------------

        public RunData() { }

        public RunData(RuntimePlayerData player, int currentColumnIndex, RunNodeState[,] nodes)
        {
            this.m_player = player;
            this.m_currentColumnIndex = currentColumnIndex;
            this.m_nodes = nodes;
        }

        #endregion

        #region Public Methods -----------------------------------

        public RuntimePlayerData Player => this.m_player;
        public int CurrentColumnIndex => this.m_currentColumnIndex;
        public RunNodeState[,] Nodes => this.m_nodes;

        public void SetRunNode(RunNodeState[,] map)
        {
            this.m_nodes = map;
        }

        public void AdvanceColumn()
        {
            m_currentColumnIndex++;
        }

        #endregion
    }

    [Serializable]
    public class RunSaveData
    {
        #region Fields -----------------------------------------------------

        [SerializeField]
        private PlayerSaveData m_player;

        [SerializeField]
        private int m_currentColumnIndex;

        [SerializeField]
        private List<NodeSaveData> m_nodes;

        #endregion

        #region Properties -------------------------------------------------

        public PlayerSaveData Player => m_player;
        public int CurrentColumnIndex => m_currentColumnIndex;
        public List<NodeSaveData> Nodes => m_nodes;

        #endregion

        #region Constructors -----------------------------------------------

        public RunSaveData() { }

        public RunSaveData(PlayerSaveData player, int columnIndex, List<NodeSaveData> nodes)
        {
            m_player = player;
            m_currentColumnIndex = columnIndex;
            m_nodes = nodes;
        }

        #endregion

        #region Static Converters ------------------------------------------

        public static RunSaveData FromRuntime(RunData runData)
        {
            var playerSave = PlayerDataConverter.ToSaveData(runData.Player);

            var nodeList = new List<NodeSaveData>();
            if (runData.Nodes != null)
            {
                int cols = runData.Nodes.GetLength(0);
                int rows = runData.Nodes.GetLength(1);

                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        var node = runData.Nodes[x, y];
                        if (node == null) continue;

                        var saveNode = new NodeSaveData(
                            node.SceneName,
                            node.Position,
                            x, y // for reconstruction
                        );
                        nodeList.Add(saveNode);
                    }
                }
            }

            return new RunSaveData(playerSave, runData.CurrentColumnIndex, nodeList);
        }

        public static RunData ToRuntime(RunSaveData saveData, CharactersData charactersData, ItemsData itemsData)
        {
            var player = PlayerDataConverter.ToRuntimeData(saveData.Player, charactersData, itemsData);

            RunNodeState[,] map = null;

            if (saveData.Nodes != null && saveData.Nodes.Count > 0)
            {
                int maxX = 0, maxY = 0;
                foreach (var n in saveData.Nodes)
                {
                    if (n.Column > maxX) maxX = n.Column;
                    if (n.Row > maxY) maxY = n.Row;
                }

                map = new RunNodeState[maxX + 1, maxY + 1];

                foreach (var n in saveData.Nodes)
                {
                    var node = new RunNodeState();
                    node.Initialize(n.EncounterId, null, n.Position); // to be fixed
                    map[n.Column, n.Row] = node;
                }
            }

            return new RunData(player, saveData.CurrentColumnIndex, map);
        }

        #endregion
    }

    [Serializable]
    public class NodeSaveData
    {
        [SerializeField]
        private string m_encounterId;

        [SerializeField]
        private Vector3 m_position;

        [SerializeField]
        private int m_column;

        [SerializeField]
        private int m_row;

        public string EncounterId => m_encounterId;
        public Vector3 Position => m_position;
        public int Column => m_column;
        public int Row => m_row;

        public NodeSaveData(string id, Vector3 pos, int col, int row)
        {
            m_encounterId = id;
            m_position = pos;
            m_column = col;
            m_row = row;
        }
    }
}
