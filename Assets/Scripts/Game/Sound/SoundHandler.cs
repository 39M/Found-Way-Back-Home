namespace WhaleFall
{
    public static class SoundHandler
    {
        public static void PlayMusic(string clipName)
        {
            SoundManager.ins.PlayMusic(clipName);
        }

        public static void PlayShortMusic(string clipName)
        {
            SoundManager.ins.PlayShortMusic(clipName);
        }

        public static void PlayEffect(string clipName, float pitch = 1f, float volume = 1f)
        {
            SoundManager.ins.PlayEffect(clipName, pitch, volume);
        }
    }
}