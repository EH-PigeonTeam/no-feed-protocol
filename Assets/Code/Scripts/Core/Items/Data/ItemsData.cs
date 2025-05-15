using UnityEngine;
using Sirenix.OdinInspector;

namespace Core.Gameplay.Items.Data
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "ItemsData", menuName = "No Feed Protocol/ItemsData")]
    public class ItemsData : ScriptableObject
    {
        [BoxGroup("Items")]
        [Tooltip("Items in the game")]
        [SerializeField]
        private Item[] m_items;

        public Item[] Items => m_items;
        public Item GetItem(int index) => m_items[index];
    }

    [System.Serializable]
    public class Item
    {
        [FoldoutGroup("@m_name")]
        [Tooltip("The name of the item")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The icon of the item")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("The description of the item")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, Range(0, 1)]
        private float m_Percent = 1f;

        // Logic

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public float Percent => m_Percent;
    }
}
