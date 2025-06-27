using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class StatSliderController : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [FoldoutGroup("Settings/References")]
        [Tooltip("Default text displayed on the stat label.")]
        [SerializeField, Required]
        private string m_baseText = "00";

        [BoxGroup("Settings")]
        [FoldoutGroup("Settings/References")]
        [Tooltip("Text component displaying the stat value.")]
        [SerializeField, Required]
        private TMP_Text m_statLabel;

        private void Awake()
        {
            if (this.m_statLabel != null)
            {
                this.m_statLabel.text = this.m_baseText;
            }
        }

        /// <summary>
        /// Updates the stat label with the given value.
        /// </summary>
        /// <param name="value">The value to display.</param>
        public void SetValue(float value)
        {
            if (this.m_statLabel != null)
            {
                this.m_statLabel.text = value.ToString("0");
            }
        }
    }
}
