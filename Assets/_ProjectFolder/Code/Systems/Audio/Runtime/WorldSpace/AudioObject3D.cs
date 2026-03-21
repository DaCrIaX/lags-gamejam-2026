using System.Collections;

namespace UnityEngine.Audio
{
    using Pool;

    public class AudioObject3D : MonoBehaviour
    {
        private AudioSource _source;
        private Transform _transform;
        private WaitWhile _whileIsPlaying;

        public IObjectPool<AudioObject3D> PoolReference { private get; set; }

        private void Awake()
        {
            _transform = transform;
            _source = GetComponent<AudioSource>();
            _whileIsPlaying = new(() => _source.isPlaying);
        }
        
        public void Play(IAudioGenerator audio, Vector3 position)
        {
            _transform.position = position;
            _source.generator = audio;
            _source.Play();

            StartCoroutine(SoundLifetime());
        }
        public void PlayOneShot(IAudioGenerator audio, Vector3 position)
        {
            if (audio is not AudioClip clip) return;
            _transform.position = position;
            _source.PlayOneShot(clip);

            StartCoroutine(SoundLifetime());
        }

        private IEnumerator SoundLifetime()
        {
            yield return _whileIsPlaying;
            _source.generator = null;

            PoolReference.Release(this);
        }
    }
}