using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Code.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    public class ImageFadeController : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Duration of the entire transition (entry or exit)"), Unit(Units.Second)]
        [SerializeField, MinValue(0)]
        private float m_transitionDuration = 0.5f;

        [BoxGroup("Settings")]
        [Tooltip("Tween easing")]
        [SerializeField]
        private Ease m_ease = Ease.Linear;

        [BoxGroup("Settings")]
        [Tooltip("Target image to apply the fade effect on")]
        [SerializeField]
        private Image m_blackScreen;

        [BoxGroup("Settings")]
        [Tooltip("Loading screen object")]
        [SerializeField]
        private GameObject m_loadingScreen;

        private Tween m_fadeTween;

        private void Awake()
        {
            if (m_blackScreen == null)
                Debug.LogError("[ImageFadeController] No Image assigned.");
        }

        /// <summary>
        /// Entry transition: fade-in → activate loading screen → fade-out
        /// </summary>
        public void PlayEntryTransition(System.Action onComplete = null)
        {
            m_fadeTween?.Kill();
            gameObject.SetActive(true);
            m_loadingScreen?.SetActive(false);
            SetAlpha(0f);

            float halfDuration = m_transitionDuration / 2f;

            m_fadeTween = DOTween.Sequence()
                .Append(m_blackScreen.DOFade(1f, halfDuration).SetEase(m_ease))
                .AppendCallback(() => m_loadingScreen?.SetActive(true))
                .Append(m_blackScreen.DOFade(0f, halfDuration).SetEase(m_ease))
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// Exit transition: fade-in → deactivate loading screen → fade-out → hide self
        /// </summary>
        public void PlayExitTransition(System.Action onComplete = null)
        {
            m_fadeTween?.Kill();
            gameObject.SetActive(true); // ensure it's active before fading

            float halfDuration = m_transitionDuration / 2f;

            m_fadeTween = DOTween.Sequence()
                .Append(m_blackScreen.DOFade(1f, halfDuration).SetEase(m_ease))
                .AppendCallback(() => m_loadingScreen?.SetActive(false))
                .Append(m_blackScreen.DOFade(0f, halfDuration).SetEase(m_ease))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    gameObject.SetActive(false);
                });
        }

        private void SetAlpha(float value)
        {
            if (m_blackScreen != null)
            {
                Color c = m_blackScreen.color;
                c.a = value;
                m_blackScreen.color = c;
            }
        }

        private void OnDestroy()
        {
            m_fadeTween?.Kill();
        }
    }
}
