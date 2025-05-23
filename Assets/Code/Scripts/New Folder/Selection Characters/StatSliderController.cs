using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class StatSliderController : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [FoldoutGroup("Settings/References")]
        [Tooltip("The slider component")]
        [SerializeField, Required]
        private Slider m_slider;

#if UNITY_EDITOR
        [FoldoutGroup("Settings/Debug")]
        [Tooltip("The value of the slider")]
        [SerializeField, Range(0f, 1f), OnValueChanged("SetFloat")]
        private float m_value = 0f;
#endif
        private void Awake()
        {
            this.m_slider.value = 0f;
        }

        public void SetFloat(float value)
        {
            this.m_slider.value = value;
        }

        public void SetValue(int i, int max)
        {
            this.m_slider.value = i / (float)max;
        }
    }
}
