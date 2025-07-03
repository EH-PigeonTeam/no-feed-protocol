using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using PsychoGarden.TriggerEvents;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class CreditsAnimation : MonoBehaviour
    {
        #region Fields --------------------------------------------

        [BoxGroup("Events")]
        [Tooltip("Event to trigger when the animation is complete")]
        [SerializeField]
        private TriggerEvent OnComplete;

        [BoxGroup("References")]
        [Tooltip("The RectTransform that contains the full credits text")]
        [SerializeField]
        private RectTransform m_content;

        [BoxGroup("References")]
        [Tooltip("The skip button's CanvasGroup (used for fade in)")]
        [SerializeField]
        private CanvasGroup m_skipButtonGroup;

        [BoxGroup("Settings")]
        [BoxGroup("Settings/Scroll")]
        [Tooltip("Duration of the scrolling animation in seconds")]
        [SerializeField]
        private float m_scrollDuration = 30f;

        [BoxGroup("Settings/Scroll")]
        [Tooltip("Ease type for the scroll animation")]
        [SerializeField]
        private Ease m_scrollEase = Ease.Linear;

        [BoxGroup("Settings/Skip Button")]
        [Tooltip("Delay before showing the skip button")]
        [SerializeField]
        private float m_skipFadeDelay = 2f;

        [BoxGroup("Settings/Skip Button")]
        [Tooltip("Fade-in duration for the skip button")]
        [SerializeField]
        private float m_skipFadeDuration = 1.5f;

        [BoxGroup("Settings/Skip Button")]
        [Tooltip("Ease for the skip button fade-in")]
        [SerializeField]
        private Ease m_skipFadeEase = Ease.InOutSine;

        private Tween m_scrollTween;
        private Tween m_skipFadeTween;

        private Vector2 m_startPos;
        private Vector2 m_endPos;

        #endregion

        #region Unity Events ---------------------------------------

        private void OnEnable()
        {
            SetupScroll();
            StartScroll();
            ShowSkipButton();
        }

        private void OnDisable()
        {
            KillScroll();
            KillFade();
        }

        #endregion

        #region Animation Logic ------------------------------------

        private void SetupScroll()
        {
            RectTransform viewport = GetComponent<RectTransform>();

            float viewportHeight = viewport.rect.height;
            float contentHeight = m_content.rect.height;

            float startY = -viewportHeight / 2f - contentHeight / 2f;
            float endY = viewportHeight / 2f + contentHeight / 2f;

            m_startPos = new Vector2(0, startY);
            m_endPos = new Vector2(0, endY);

            m_content.anchoredPosition = m_startPos;
        }

        private void StartScroll()
        {
            m_scrollTween = m_content
                .DOAnchorPos(m_endPos, m_scrollDuration)
                .SetEase(m_scrollEase)
                .OnComplete(() =>
                {
                    OnComplete?.Invoke(this.transform);
                });
        }

        private void ShowSkipButton()
        {
            if (m_skipButtonGroup == null)
                return;

            m_skipButtonGroup.alpha = 0f;
            m_skipButtonGroup.gameObject.SetActive(true);

            m_skipFadeTween = m_skipButtonGroup
                .DOFade(1f, m_skipFadeDuration)
                .SetDelay(m_skipFadeDelay)
                .SetEase(m_skipFadeEase);
        }

        private void KillScroll()
        {
            if (m_scrollTween != null && m_scrollTween.IsActive())
            {
                m_scrollTween.Kill();
                m_scrollTween = null;
            }
        }

        private void KillFade()
        {
            if (m_skipFadeTween != null && m_skipFadeTween.IsActive())
            {
                m_skipFadeTween.Kill();
                m_skipFadeTween = null;
            }

            if (m_skipButtonGroup != null)
            {
                m_skipButtonGroup.alpha = 0f;
                m_skipButtonGroup.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
