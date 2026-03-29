using System.Collections;

namespace UnityEngine.Audio
{
    public class AudioSequence : RandomAudioEmitter
    {
        [SerializeField] private float _audioDuration = 1f;
        [SerializeField] private bool _isLoop;
        private int _currentIndex = 0;

        public override void PlayOneShot() { }
        public override async void Play()
        {
            if (_isLoop) _currentIndex %= _audioReference.Length;
            if (_currentIndex >= _audioReference.Length) return;

            var sound = await LoadAssetIndex(_currentIndex);
            _manager.Play(_channel, sound);
            _currentIndex++;

            StartCoroutine(SwapDelay());
        }

        private IEnumerator SwapDelay()
        {
            yield return new WaitForSeconds(_audioDuration);
            Play();
        }
    }
}