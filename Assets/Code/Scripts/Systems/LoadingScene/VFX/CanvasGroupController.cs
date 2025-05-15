using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

namespace Code.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("The transition duration in seconds"), Unit(Units.Second)]
        [SerializeField, MinValue(0)]
        private float m_transitionDuration = 0.5f;

        [BoxGroup("Settings")]
        [Tooltip("The transition effect")]
        [SerializeField]
        private Ease m_ease = Ease.Linear;

        [BoxGroup("Settings")]
        [Tooltip("Enable fade-in effect on enable")]
        [SerializeField]
        private bool m_useFadeIn = true;

        [BoxGroup("Settings")]
        [Tooltip("Enable fade-out effect on hide")]
        [SerializeField]
        private bool m_useFadeOut = true;

        private CanvasGroup m_canvasGroup;
        private Tween m_fadeTween;

        public bool UseFadeIn
        {
            get => this.m_useFadeIn;
            set => this.m_useFadeIn = value;
        }

        public bool UseFadeOut
        {
            get => this.m_useFadeOut;
            set => this.m_useFadeOut = value;
        }

        public float TransitionDuration => this.m_transitionDuration;

        private void Awake()
        {
            this.m_canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            this.gameObject.SetActive(true);

            this.m_fadeTween?.Kill();

            if (this.m_useFadeIn)
            {
                this.m_canvasGroup.alpha = 0;
                this.m_fadeTween = this.m_canvasGroup
                    .DOFade(1, this.m_transitionDuration)
                    .SetEase(this.m_ease)
                    .SetUpdate(true);
            }
            else
            {
                this.m_canvasGroup.alpha = 1;
            }
        }

        public void Hide()
        {
            this.m_fadeTween?.Kill();

            if (this.m_useFadeOut)
            {
                this.m_fadeTween = this.m_canvasGroup
                    .DOFade(0, this.m_transitionDuration)
                    .SetEase(this.m_ease)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        this.gameObject.SetActive(false);
                    });
            }
            else
            {
                this.m_canvasGroup.alpha = 0;

                this.gameObject.SetActive(false);
            }
        }

        public void SetUseFadeIn(bool value) => this.m_useFadeIn = value;

        public void SetUseFadeOut(bool value) => this.m_useFadeOut = value;

        private void OnDestroy()
        {
            this.m_fadeTween?.Kill();
        }
    }
}
