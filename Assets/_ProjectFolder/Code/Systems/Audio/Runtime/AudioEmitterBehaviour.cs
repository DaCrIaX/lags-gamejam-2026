namespace UnityEngine.Audio
{
    using AddressableAssets;

    public abstract class AudioEmitterBehaviour : MonoBehaviour
    {
        [SerializeField] protected Channel _channel = Channel.SoundFx;

        protected IAudioManager _manager;

        protected virtual void Awake() => _manager = AudioManager.Instance;

        protected async Awaitable<IAudioGenerator> LoadAsset(AssetReferenceT<AudioClip> reference, bool hasLoaded = false)
        {
            if (reference == null) return null;
            return await _manager.LoadAudioAsset(reference, hasLoaded);
        }

        public abstract void Play();
        public abstract void PlayOneShot();
        public void ResetAudio() => _manager?.ResetAudio(_channel);
    }
}