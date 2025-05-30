using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.Services.Resolvers
{
    public abstract class GenericResolver<TData, TDatabase> : MonoBehaviour where TDatabase : ScriptableObject
    {
        [BoxGroup("References")]
        [SerializeField, InlineEditor]
        protected TDatabase m_database;

        /// <summary>
        /// Resolves a single item by ID.
        /// </summary>
        public abstract TData GetById(string id);

        /// <summary>
        /// Resolves multiple items by ID list.
        /// </summary>
        public virtual List<TData> GetByIds(List<string> ids)
        {
            return ids
                .Select(GetById)
                .Where(item => item != null)
                .ToList();
        }
    }
}
