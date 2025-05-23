using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Items.Abilities
{
    [System.Serializable]
    public class AbilityEffect
    {
        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityEffectType Type;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityEffectAction Action;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public int Value;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityTargetType Target;
    }
}
