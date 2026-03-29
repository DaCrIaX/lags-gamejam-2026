namespace UnityEngine.Audio
{
    public class RandomAudioEmitterAction : RandomAudioEmitter
    {
        [SerializeField] private Vector2 _pitch = Vector2.one;

        public override async void PlayOneShot()
        {
            var clip = await LoadAssetIndex(Random.Range(0, _audioReference.Length));
            float pitch = _pitch.x == _pitch.y ? _pitch.x : Random.Range(_pitch.x, _pitch.y);

            _manager.PlayOneShot(_channel, clip, pitch);
        }
    }
}