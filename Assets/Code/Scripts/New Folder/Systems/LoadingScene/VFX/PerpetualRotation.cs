using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Code.Scripts.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    public class PerpetualRotation : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Rotation axis")]
        [SerializeField]
        private Axis m_axis = Axis.Y;

        [BoxGroup("Settings")]
        [Tooltip("Velocity")]
        [SerializeField, Range(0f, 360f)]
        private float m_degreePerSecond = 90f;

        [BoxGroup("Settings")]
        [Tooltip("Velocity")]
        [SerializeField]
        private Orientation m_orientation = Orientation.Clockwise;

        private Tween m_rotationTween;

        private void OnEnable()
        {
            Vector3 axisVector = m_axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.up
            };

            if (m_orientation == Orientation.CounterClockwise)
            {
                axisVector = -axisVector;
            }

            float duration = 360f / m_degreePerSecond;

            m_rotationTween = transform
                .DORotate(axisVector * 360f, duration, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            if (m_rotationTween != null && m_rotationTween.IsActive())
            {
                m_rotationTween.Kill();
                m_rotationTween = null;
            }
        }
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum Orientation
    {
        Clockwise,
        CounterClockwise
    }
}
