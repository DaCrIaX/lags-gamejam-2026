namespace UnityEngine.Audio
{
    public class AudioEmitter3D : AudioEmitter
    {
        private Transform _transform;

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }

        [ContextMenu("Play")]
        public override async void Play()
        {
            var audio = await _manager.LoadAudioAsset(_audioReference, _hasLoaded);
            _hasLoaded = true;

            _manager.PlayAtPoint(_channel, audio, _transform.position);
        }

        [ContextMenu("PlayOneShot")]
        public override async void PlayOneShot()
        {
            var audio = await _manager.LoadAudioAsset(_audioReference, _hasLoaded);
            _hasLoaded = true;

            _manager.PlayOneShotAtPoint(_channel, audio, _transform.position);
        }
    }
}