namespace UnityEngine.Audio
{
    public class AudioFadeout : AudioEmitterBehaviour
    {
        public override void Play() => _manager.Play(_channel, null);
        public override void PlayOneShot() { }
    }
}