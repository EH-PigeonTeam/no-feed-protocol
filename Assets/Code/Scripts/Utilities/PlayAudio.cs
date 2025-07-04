using Code.Systems.Audio;
using Code.Systems.Locator;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    private string m_audioName;

    [SerializeField]
    private bool m_playOnEnable = true;

    private void OnEnable()
    {
        if (!m_playOnEnable) return;

        ServiceLocator.Get<AudioManager>().PlayAudioClip(m_audioName);
    }

    public void PlayAudioClip(string audioName)
    {
        ServiceLocator.Get<AudioManager>().PlayAudioClip(audioName);
    }
}
