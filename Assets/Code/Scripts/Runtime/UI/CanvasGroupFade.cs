using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFade : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Speed"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        private float m_speed = 1f;

        [BoxGroup("Settings")]
        [Tooltip("Ease function for transitions")]
        [SerializeField]
        private Ease m_ease = Ease.Linear;

        private CanvasGroup m_canvasGroup;
        private Tween m_currentTween;

        #region Initialization and State Setup --------------------------

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            FadeIn();
        }

        private void OnDisable()
        {
            m_currentTween?.Kill(); // Ensure no tweens persist after disable
        }

        #endregion

        #region Public Methods ------------------------------------------

        /// <summary>
        /// Fades in the canvas group when enabled.
        /// </summary>
        public void FadeIn()
        {
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = false;

            m_currentTween = m_canvasGroup.DOFade(1f, m_speed)
                .SetEase(m_ease)
                .OnComplete(() =>
                {
                    m_canvasGroup.interactable = true;
                    m_canvasGroup.blocksRaycasts = true;
                });
        }

        /// <summary>
        /// Fades out the canvas group and disables the GameObject.
        /// </summary>
        public void FadeOutAndDisable()
        {
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = false;

            m_currentTween = m_canvasGroup.DOFade(0f, m_speed)
                .SetEase(m_ease)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        #endregion
    }
}
