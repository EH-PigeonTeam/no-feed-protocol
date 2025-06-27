using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEditor.Tilemaps;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class MenuTransition : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Transition")]
        [Tooltip("The ease of the transition")]
        [SerializeField] private Ease m_ease;

        [BoxGroup("Transition")]
        [Tooltip("The duration of the transition in seconds"), Unit(Units.Second)]
        [SerializeField, MinValue(0f)] 
        private float m_duration = 0.10f;

        #endregion

        public void TransitionIn(GameObject go)
        {
            if (go.TryGetComponent(out CanvasGroup canvasGroup))
            {
                go.SetActive(true);
                canvasGroup.DOFade(1f, m_duration)
                    .SetEase(m_ease);
            }
        }

        public void TransitionOut(GameObject go)
        {
            if (go.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.DOFade(0f, m_duration)
                    .SetEase(m_ease)
                    .OnComplete(() => go.SetActive(false));
            }
        }

        public void Disable(GameObject go)
        {
            if(go.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void Enable(GameObject go)
        {
            if (go.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
