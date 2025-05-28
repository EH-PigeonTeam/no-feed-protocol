using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    [Serializable]
    public class AbilityCondition
    {
        [FoldoutGroup("AbilityCondition")]
        [Tooltip("The type of condition")]
        [SerializeField]
        private AbilityConditionType m_type;

        public AbilityConditionType Type => m_type;
    }
}
