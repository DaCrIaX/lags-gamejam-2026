namespace UnityEngine.Audio
{
    using Pool;

    public class WorldPointChannel : AudioChannel
    {
        [SerializeField] private AudioObject3D _prefab;

        private ObjectPool<AudioObject3D> _pool;
        private Transform _parent;

        public override AudioSource Source => null;

        private void Awake()
        {
            _parent = transform;
            _pool = new(ObjCreate, ObjEnable, ObjDisable, ObjDestroy);
        }
        private AudioObject3D ObjCreate()
        {
            var instance = Instantiate(_prefab, _parent);
            instance.PoolReference = _pool;
            return instance;
        }

        private void ObjEnable(AudioObject3D @object) => @object.gameObject.SetActive(true);
        private void ObjDisable(AudioObject3D @object) => @object.gameObject.SetActive(false);
        private void ObjDestroy(AudioObject3D @object) => Destroy(@object.gameObject);

        public override void PlayAtPoint(IAudioGenerator audio, Vector3 position)
        {
            _pool.Get().Play(audio, position);
        }
        public override void PlayOneShotAtPoint(IAudioGenerator audio, Vector3 position, float pitch = 1f)
        {
            _pool.Get().PlayOneShot(audio, position);
        }
    }
}