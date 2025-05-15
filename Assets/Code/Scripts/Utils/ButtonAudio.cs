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

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        ServiceLocator.Get<AudioManager>().PlayAudioClip(this.m_onHoverSound);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        ServiceLocator.Get<AudioManager>().PlayAudioClip(this.m_onClickSound);
    }
}
