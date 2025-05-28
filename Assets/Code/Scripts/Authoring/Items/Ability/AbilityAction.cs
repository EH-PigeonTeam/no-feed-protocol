using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    [System.Serializable]
    public class AbilityAction
    {
        [FoldoutGroup("$m_action")]
        [SerializeField]
        private AbilityEffectAction m_action;

        [FoldoutGroup("$m_action")]
        [SerializeField]
        private AbilityTargetType m_target;

        [FoldoutGroup("$m_action")]
        [SerializeField]
        private int m_value;

        public AbilityEffectAction Action => m_action;
        public AbilityTargetType Target => m_target;
        public int Value => m_value;
    }
}
