using System;
using UnityEngine;

namespace NoFeedProtocol.Runtime.Logic.Battle.Players
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 Position;
        public Vector3 Rotation; // Euler angles
        public Vector3 Scale;

        public static TransformData Default()
        {
            return new TransformData
            {
                Position = Vector3.zero,
                Rotation = Vector3.zero,
                Scale = Vector3.one
            };
        }

        public static TransformData FromTransform(Transform transform)
        {
            return new TransformData
            {
                Position = transform.localPosition,
                Rotation = transform.localEulerAngles,
                Scale = transform.localScale
            };
        }

        public void ApplyTo(Transform transform)
        {
            transform.localPosition = Position;
            transform.localEulerAngles = Rotation;
            transform.localScale = Scale;
        }
    }
}
