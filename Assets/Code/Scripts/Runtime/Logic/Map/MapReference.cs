using UnityEngine;
using Sirenix.OdinInspector;
using System;
using NoFeedProtocol.Runtime.UI.Utilities;

namespace NoFeedProtocol.Runtime.Logic.Map
{
    [Serializable]
    public class MapReference
    {
        [SerializeField, AssetsOnly]
        private GameObject m_nodePrefab;

        [SerializeField, SceneObjectsOnly]
        private Transform m_nodeParent;

        [SerializeField, AssetsOnly]
        private GameObject m_connectionPrefab;

        [SerializeField, SceneObjectsOnly]
        private Transform m_connectionParent;

        public GameObject NodePrefab => m_nodePrefab;
        public GameObject ConnectionPrefab => m_connectionPrefab;

        public Transform NodeParent => m_nodeParent;
        public Transform ConnectionParent => m_connectionParent;
    }
}
