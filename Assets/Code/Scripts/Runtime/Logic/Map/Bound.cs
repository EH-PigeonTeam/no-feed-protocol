using UnityEngine;
using NoFeedProtocol.Authoring.Map;
using System;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [Serializable]
    public class Bound
    {
        [SerializeField, Range(1, 10)]
        private int m_rows = 5;

        [SerializeField]
        private EncounterType m_type;

        public int Rows => m_rows;
        public EncounterType Type => m_type;
    }
}
