using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Runtime.UI.Utilities
{
    [HideMonoScript]
    [ExecuteAlways]
    public class GraphicRenderOrder : MonoBehaviour
    {
        [BoxGroup("Rendering")]
        [Tooltip("Graphic component to control (Image, Text, etc.)")]
        [SerializeField, Required]
        private Graphic m_target;

        [BoxGroup("Rendering")]
        [Tooltip("Order relative to siblings. Higher = drawn later (on top)")]
        [SerializeField]
        private int m_siblingOffset = 0;

        [BoxGroup("Rendering")]
        [Button(ButtonSizes.Medium)]
        private void Apply()
        {
            if (m_target == null)
            {
                Debug.LogWarning("[GraphicRenderOrder] No target set.");
                return;
            }

            var rect = m_target.rectTransform;
            var parent = rect.parent;

            if (parent == null)
            {
                Debug.LogWarning("[GraphicRenderOrder] Target has no parent.");
                return;
            }

            int baseIndex = rect.GetSiblingIndex();
            int newIndex = Mathf.Clamp(baseIndex + m_siblingOffset, 0, parent.childCount - 1);
            rect.SetSiblingIndex(newIndex);
        }

        private void OnEnable()
        {
            Apply();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                Apply();
        }
#endif
    }
}
