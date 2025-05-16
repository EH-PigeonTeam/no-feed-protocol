using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

[HideMonoScript]
public class LoadingBar : MonoBehaviour
{
    [BoxGroup("Settings")]
    [Tooltip("List of images")]
    [SerializeField]
    private Image[] m_images = null;

    [BoxGroup("Settings")]
    [Tooltip("Fade duration per image (in seconds)")]
    [SerializeField]
    private float m_speed = 0.2f;

    [BoxGroup("Settings")]
    [Tooltip("Ease effect for the fade")]
    [SerializeField]
    private Ease m_ease = Ease.Linear;

    [BoxGroup("Settings")]
    [Tooltip("Pause duration between full cycles")]
    [SerializeField]
    private float m_pauseDuration = 1f;

    private Sequence m_sequence;

    private void OnEnable()
    {
        ResetImageAlphas();
        BuildSequence();
    }

    private void OnDisable()
    {
        KillSequence();
        ResetImageAlphas();
    }

    /// <summary>
    /// Resets all image alphas to 0 and kills any active tweens on them
    /// </summary>
    private void ResetImageAlphas()
    {
        if (m_images == null) return;

        foreach (var img in m_images)
        {
            if (img != null)
            {
                img.DOKill();
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }
    }

    /// <summary>
    /// Kills the current DOTween sequence if active
    /// </summary>
    private void KillSequence()
    {
        if (m_sequence != null && m_sequence.IsActive())
        {
            m_sequence.Kill();
            m_sequence = null;
        }
    }

    /// <summary>
    /// Builds the DOTween sequence for sequential fade-in and fade-out
    /// </summary>
    private void BuildSequence()
    {
        if (m_images == null || m_images.Length == 0)
            return;

        m_sequence = DOTween.Sequence();

        // Phase 1: Fade-in each image one after the other
        foreach (var img in m_images)
        {
            if (img == null) continue;

            m_sequence.Append(img.DOFade(1f, m_speed).SetEase(m_ease));
        }

        // Wait after all images are fully visible
        m_sequence.AppendInterval(m_pauseDuration);

        // Phase 2: Fade-out each image one after the other
        foreach (var img in m_images)
        {
            if (img == null) continue;

            m_sequence.Append(img.DOFade(0f, m_speed).SetEase(m_ease));
        }

        // Wait before restarting the full cycle
        m_sequence.AppendInterval(m_pauseDuration);

        // Loop the whole sequence forever
        m_sequence.SetLoops(-1, LoopType.Restart);
    }
}
