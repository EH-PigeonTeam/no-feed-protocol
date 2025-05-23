using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;
using UnityEngine.UI;

namespace Core.Gameplay.SlotMachine
{
    [HideMonoScript]
    public class Indicator : MonoBehaviour, IActivatable, IResettable
    {
        [FoldoutGroup("Settings")]
        [Tooltip("The indicator image")]
        [SerializeField, Required]
        private Image m_image;

        [FoldoutGroup("Settings")]
        [Tooltip("The color to use when the indicator is not active")]
        [SerializeField]
        private Color m_colorDefault = Color.white;

        [FoldoutGroup("Settings")]
        [Tooltip("The color to use when the indicator is active")]
        [SerializeField]
        private Color m_colorActive = Color.white;

        public void Activate()
        {
            ChangeColor(this.m_colorActive);
        }

        public void Deactivate()
        {
            ChangeColor(this.m_colorDefault);
        }

        public void Restore()
        {
            ChangeColor(this.m_colorDefault);
        }

        private void ChangeColor(Color color)
        {
            this.m_image.color = color;
        }
    }

    public interface IActivatable
    {
        void Activate();
        void Deactivate();
    }
}
