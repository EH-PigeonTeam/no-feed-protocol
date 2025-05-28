using System;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Characters
{
    [Serializable]
    public class AimingBehavior
    {
        [BoxGroup("Targeting")]
        [Tooltip("The strategy to use for target selection.")]
        [SerializeField]
        private TargetStrategy m_targetStrategy;

        [BoxGroup("Targeting")]
        [Tooltip("The value to use for target selection.")]
        [SerializeField, Range(0f, 1f), ShowIf("m_targetStrategy", TargetStrategy.RandomSplit)]
        private float m_value = 0.5f;

        public TargetStrategy TargetStrategy => this.m_targetStrategy;
        public float Value => this.m_value;
    }
}