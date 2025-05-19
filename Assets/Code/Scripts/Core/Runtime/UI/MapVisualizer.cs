using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Data.Run;
using NoFeedProtocol.Runtime.UI.Utilities;
using System.Collections.Generic;

namespace NoFeedProtocol.Runtime.UI.Map
{
    [HideMonoScript]
    public class MapVisualizer : MonoBehaviour
    {
        #region Inspector References ----------------------------------------

        [BoxGroup("References")]
        [SerializeField, Required]
        private RectTransform m_lineContainer;

        [BoxGroup("References")]
        [SerializeField, Required, AssetsOnly]
        private GameObject m_linePrefab;

        #endregion

        #region Public Methods ----------------------------------------------

        /// <summary>
        /// Draws UI line renderers between connected nodes.
        /// </summary>
        public void DrawConnections(RunNodeState[,] map)
        {
            ClearLines();

            int cols = map.GetLength(0);
            int rows = map.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    RunNodeState from = map[col, row];
                    if (from == null || from.Nodes == null) continue;

                    foreach (var to in from.Nodes)
                    {
                        if (to == null) continue;

                        var fromRect = from.GetComponent<RectTransform>();
                        var toRect = to.GetComponent<RectTransform>();

                        if (fromRect != null && toRect != null)
                        {
                            DrawUILine(fromRect, toRect);
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Methods --------------------------------------------

        private void DrawUILine(RectTransform from, RectTransform to)
        {
            var lineGO = Instantiate(m_linePrefab, m_lineContainer);
            var line = lineGO.GetComponent<UILineRenderer>();

            if (line == null)
            {
                Debug.LogError("[MapVisualizer] Missing UILineRenderer component on line prefab.");
                return;
            }

            Vector2 start;
            Vector2 end;

            // Convert world position to local space of lineContainer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_lineContainer,
                RectTransformUtility.WorldToScreenPoint(null, from.position),
                null,
                out start
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_lineContainer,
                RectTransformUtility.WorldToScreenPoint(null, to.position),
                null,
                out end
            );

            line.points = new List<Vector2> { start, end };
            line.SetAllDirty();
        }

        private void ClearLines()
        {
            foreach (Transform child in m_lineContainer)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion
    }
}
