namespace UnityEngine.Audio
{
    using AddressableAssets;

    public class RandomAudioEmitter : AudioEmitterBehaviour
    {
        [Header("Audio Reference")]
        [SerializeField] protected AssetReferenceT<AudioClip>[] _audioReference = new AssetReferenceT<AudioClip>[1];
        [SerializeField] private bool _preloadAsset, _playOnAwake, _saveClipOnAwake;

        protected bool[] _hasLoaded;

        protected override void Awake()
        {
            base.Awake();
            _hasLoaded = new bool[_audioReference.Length];
        }

        private async void Start()
        {
            if (_preloadAsset)
            {
                for (int i = 0; i < _audioReference.Length; i++)
                    await LoadAssetIndex(i);
            }

            if (_playOnAwake) Play();
            if (_saveClipOnAwake) _manager.SaveCurrentAudio(_channel);
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _audioReference.Length; i++)
                UnloadAssetIndex(i);
        }

        public override async void Play()
        {
            var audio = await LoadAssetIndex(Random.Range(0, _audioReference.Length));
            _manager.Play(_channel, audio);
        }
        public override async void PlayOneShot()
        {
            var audio = await LoadAssetIndex(Random.Range(0, _audioReference.Length));
            _manager.PlayOneShot(_channel, audio);
        }

        protected async Awaitable<IAudioGenerator> LoadAssetIndex(int index)
        {
            bool hasLoaded = _hasLoaded[index];
            if (!hasLoaded) _hasLoaded[index] = true;
            return await LoadAsset(_audioReference[index], hasLoaded);
        }
        protected void UnloadAssetIndex(int index)
        {
            if (!_hasLoaded[index]) return;
            _manager.UnloadAudioAsset(_audioReference[index]);
        }
    }
}