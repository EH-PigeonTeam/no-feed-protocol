using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Items
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "ItemsData", menuName = "No Feed Protocol/ItemsData")]
    public class ItemsData : ScriptableObject
    {
        [BoxGroup("Items")]
        [Tooltip("Items in the game")]
        [SerializeField]
        private Item[] m_items;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var item in m_items)
                item.OnValidate();
        }
#endif

        public Item[] Items => m_items;

        public Item GetItem(int index) => m_items[index];

        /// <summary>
        /// Finds an item by its unique ID.
        /// </summary>
        public Item GetById(string id)
        {
            foreach (var item in m_items)
            {
                if (item.Id == id)
                    return item;
            }

            Debug.LogWarning($"Item ID '{id}' not found.");
            return null;
        }
    }
}
