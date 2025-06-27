using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NoFeedProtocol.Runtime.UI
{
    [ExecuteAlways]
    [AddComponentMenu("Layout/Indented Vertical Layout Group")]
    [HideMonoScript]
    public class IndentedVerticalLayoutGroup : LayoutGroup
    {
        [SerializeField] private float m_verticalSpacing = 32f;
        [SerializeField] private float m_baseHorizontalPadding = 0f;
        [SerializeField] private float m_paddingStep = 8f;
        [SerializeField] private bool m_reverseOrder = false;

        [InlineProperty, SerializeField] private BoolPair m_childControl = false;
        [InlineProperty, SerializeField] private BoolPair m_childScale = false;
        [InlineProperty, SerializeField] private BoolPair m_childForceExpand = true;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
        }

        public override void CalculateLayoutInputVertical()
        {
            float totalHeight = padding.vertical;
            int count = rectChildren.Count;

            for (int i = 0; i < count; i++)
            {
                RectTransform child = rectChildren[i];
                float height = LayoutUtility.GetPreferredHeight(child);
                float scaleY = m_childScale.Height ? child.localScale.y : 1f;
                totalHeight += height * scaleY;

                if (i < count - 1)
                    totalHeight += m_verticalSpacing;
            }

            SetLayoutInputForAxis(totalHeight, totalHeight, 0, 1);
        }

        public override void SetLayoutHorizontal()
        {
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float scaleX = m_childScale.Width ? child.localScale.x : 1f;

                float indent = m_baseHorizontalPadding + (m_paddingStep * i);
                float width = m_childControl.Width
                    ? rectTransform.rect.width - padding.horizontal - indent * 2
                    : LayoutUtility.GetPreferredWidth(child);

                if (m_childForceExpand.Width)
                    width = Mathf.Max(width, rectTransform.rect.width - indent * 2 - padding.horizontal);

                float x = GetAlignedX(child, width, indent);

                if (m_childControl.Width || m_childForceExpand.Width)
                    SetChildAlongAxis(child, 0, x, width * scaleX);
                else
                    SetChildAlongAxis(child, 0, x);
            }
        }

        public override void SetLayoutVertical()
        {
            int count = rectChildren.Count;
            if (count == 0) return;

            float totalHeight = GetTotalContentHeight();
            float y = GetStartOffset(1, totalHeight);

            for (int i = 0; i < count; i++)
            {
                int index = m_reverseOrder ? count - 1 - i : i;
                RectTransform child = rectChildren[index];

                float height = m_childControl.Height
                    ? LayoutUtility.GetPreferredHeight(child)
                    : child.rect.height;

                float scaleY = m_childScale.Height ? child.localScale.y : 1f;

                if (m_childForceExpand.Height)
                    height = Mathf.Max(height, LayoutUtility.GetFlexibleHeight(child));

                if (m_childControl.Height || m_childForceExpand.Height)
                    SetChildAlongAxis(child, 1, y, height * scaleY);
                else
                    SetChildAlongAxis(child, 1, y);

                y += height * scaleY + m_verticalSpacing;
            }
        }

        private float GetAlignedX(RectTransform child, float width, float indent)
        {
            float containerWidth = rectTransform.rect.width;

            return childAlignment switch
            {
                TextAnchor.UpperLeft or TextAnchor.MiddleLeft or TextAnchor.LowerLeft => padding.left + indent,
                TextAnchor.UpperCenter or TextAnchor.MiddleCenter or TextAnchor.LowerCenter => (containerWidth - width) * 0.5f,
                TextAnchor.UpperRight or TextAnchor.MiddleRight or TextAnchor.LowerRight => containerWidth - padding.right - indent - width,
                _ => padding.left + indent,
            };
        }

        private float GetTotalContentHeight()
        {
            float total = padding.vertical;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                float h = LayoutUtility.GetPreferredHeight(rectChildren[i]);
                float scaleY = m_childScale.Height ? rectChildren[i].localScale.y : 1f;
                total += h * scaleY;

                if (i < rectChildren.Count - 1)
                    total += m_verticalSpacing;
            }

            return total;
        }
    }

    [Serializable]
    public struct BoolPair
    {
        [HorizontalGroup("WidthHeight"), ToggleLeft] public bool Width;
        [HorizontalGroup("WidthHeight"), ToggleLeft] public bool Height;

        public BoolPair(bool width, bool height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator BoolPair(bool value)
        {
            return new BoolPair(value, value);
        }
    }
}
