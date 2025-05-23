using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [Serializable]
    public class MapStrutcture
    {
        [SerializeField, Range(2, 20)]
        private int m_columns = 5;

        [SerializeField, MinMaxSlider(1, 10)]
        private Vector2Int m_rows = new(2,5);

        [SerializeField]
        private Bound m_start;

        [SerializeField]
        private Bound m_end;

        [SerializeField]
        private Vector2 m_cellSize = new(1f, 1f);

        public int Columns => m_columns;
        public Vector2Int Rows => m_rows;
        public Bound Start => m_start;
        public Bound End => m_end;
        public Vector2 CellSize => m_cellSize;
    }
}
