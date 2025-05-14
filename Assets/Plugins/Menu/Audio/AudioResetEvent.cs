namespace PsychoGarden.Audio
{
    public static class AudioResetEvent
    {
        public static event System.Action OnAudioReset;

        public static void Raise()
        {
            OnAudioReset?.Invoke();
        }
    }
}