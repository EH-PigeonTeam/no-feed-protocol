using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;

namespace Project.Runtime.Logic
{
    [HideMonoScript]
    public class InteractiveColorFeedback : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerClickHandler
    {
        [FoldoutGroup("Target"), Required]
        [LabelText("Target Object")]
        [SerializeField]
        private GameObject m_targetObject;

        private Graphic m_graphic;
        private TextMeshProUGUI m_tmpText;
        private Renderer m_renderer;

        [FoldoutGroup("Colors"), LabelText("Normal Color"), PropertySpace]
        [SerializeField]
        private Color m_normalColor = Color.white;

        [FoldoutGroup("Colors"), LabelText("Hover Color"), PropertySpace]
        [SerializeField]
        private Color m_hoverColor = Color.yellow;

        [FoldoutGroup("Colors"), LabelText("Pressed Color"), PropertySpace]
        [SerializeField]
        private Color m_pressedColor = Color.gray;

        [FoldoutGroup("Tween"), LabelText("Duration"), PropertySpace]
        [SerializeField]
        private float m_duration = 0.25f;

        [FoldoutGroup("Tween"), LabelText("Ease")]
        [SerializeField]
        private Ease m_ease = Ease.OutQuad;

        [FoldoutGroup("Events"), LabelText("On Click"), PropertySpace]
        [SerializeField]
        private TriggerEvent m_onClick;

        [FoldoutGroup("Events"), LabelText("On Release")]
        [SerializeField]
        private TriggerEvent m_onRelease;

        private Tweener m_tweener;
        private bool m_isHovering;

        #region Initialization and State Setup --------------------------

        private void Reset()
        {
            if (m_targetObject == null)
                m_targetObject = gameObject;
        }

        private void Awake()
        {
            InitializeTargetComponents();
            SetColorInstant(m_normalColor);
        }

        private void InitializeTargetComponents()
        {
            m_graphic = m_targetObject.GetComponent<Graphic>();
            m_tmpText = m_targetObject.GetComponent<TextMeshProUGUI>();
            m_renderer = m_targetObject.GetComponent<Renderer>();
        }

        #endregion

        #region Event Handlers ------------------------------------------

        /// <summary>
        /// Triggered when the pointer enters the object.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isHovering = true;
            AnimateColor(m_hoverColor);
        }

        /// <summary>
        /// Triggered when the pointer exits the object.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            m_isHovering = false;
            AnimateColor(m_normalColor);
        }

        /// <summary>
        /// Triggered on pointer down (click press).
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateColor(m_pressedColor);
        }

        /// <summary>
        /// Triggered on pointer up (release).
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Color target = m_isHovering ? m_hoverColor : m_normalColor;
            AnimateColor(target);
            m_onRelease?.Invoke(this.transform);
        }

        /// <summary>
        /// Triggered on pointer click (after down + up inside target).
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            m_onClick?.Invoke(this.transform);
        }

        #endregion

        #region Color Logic ---------------------------------------------

        private void AnimateColor(Color targetColor)
        {
            m_tweener?.Kill();

            if (m_graphic != null)
            {
                m_tweener = m_graphic.DOColor(targetColor, m_duration).SetEase(m_ease);
            }
            else if (m_tmpText != null)
            {
                m_tweener = m_tmpText.DOColor(targetColor, m_duration).SetEase(m_ease);
            }
            else if (m_renderer != null)
            {
                Material mat = m_renderer.material;
                m_tweener = mat.DOColor(targetColor, "_Color", m_duration).SetEase(m_ease);
            }
        }

        private void SetColorInstant(Color color)
        {
            if (m_graphic != null)
                m_graphic.color = color;
            else if (m_tmpText != null)
                m_tmpText.color = color;
            else if (m_renderer != null)
                m_renderer.material.color = color;
        }

        #endregion
    }
}
