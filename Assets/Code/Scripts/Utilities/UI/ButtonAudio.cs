using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Code.Systems.Locator;
using Code.Systems.Audio;

[DisallowMultipleComponent]
public class ButtonAudio : Button
{
    [Header("Audio")]
    [SerializeField] private string m_onHoverSound;
    [SerializeField] private string m_onClickSound;

    [Header("Graphics")]
    [SerializeField] private Graphic m_graphic;
    [SerializeField] private Color m_normalColor = Color.white;
    [SerializeField] private Color m_highlightedColor = Color.white;
    [SerializeField] private Color m_pressedColor = Color.white;
    [SerializeField] private Color m_selectedColor = Color.white;
    [SerializeField] private Color m_disabledColor = Color.gray;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        // Play audio clips
        switch (state)
        {
            case SelectionState.Highlighted:
                ServiceLocator.Get<AudioManager>().PlayAudioClip(this.m_onHoverSound);
                break;
            case SelectionState.Pressed:
                ServiceLocator.Get<AudioManager>().PlayAudioClip(this.m_onClickSound);
                break;
            default:
                break;
        }

        // Update graphic color if it's set
        if (this.m_graphic != null)
        {
            switch (state)
            {
                case SelectionState.Normal:
                    this.m_graphic.color = this.m_normalColor;
                    break;
                case SelectionState.Highlighted:
                    this.m_graphic.color = this.m_highlightedColor;
                    break;
                case SelectionState.Pressed:
                    this.m_graphic.color = this.m_pressedColor;
                    break;
                case SelectionState.Selected:
                    this.m_graphic.color = this.m_selectedColor;
                    break;
                case SelectionState.Disabled:
                    this.m_graphic.color = this.m_disabledColor;
                    break;
                default:
                    break;
            }
        }
    }
}
