using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Authoring.Items;
using Code.Systems.Locator;

namespace NoFeedProtocol.Runtime.Services.Items
{
    [HideMonoScript]
    public class ItemResolver : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField]
        private ItemsData m_database;

        /// <summary>
        /// Resolves a single item by its unique ID.
        /// </summary>
        public Item GetById(string id)
        {
            return m_database.Items.FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Resolves multiple items by their unique IDs.
        /// </summary>
        public List<Item> GetByIds(List<string> ids)
        {
            return ids
                .Select(GetById)
                .Where(item => item != null)
                .ToList();
        }

        private void OnEnable()
        {
            ServiceLocator.Register<ItemResolver>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<ItemResolver>();
        }
    }
}
