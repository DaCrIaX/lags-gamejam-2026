namespace UnityEngine.Pool
{
    public abstract class PoolObjectBehaviour : MonoBehaviour
    {
        [field: SerializeField] public Transform Transform { get; private set; }

        public IObjectPool<PoolObjectBehaviour> PoolReference { protected get; set; }
        public uint Index { protected get; set; }

        private bool _isSpawned;

        protected virtual void Reset() => Transform = transform;
        protected virtual void OnEnable() => _isSpawned = true;
        protected virtual void OnDisable()
        {
            if (_isSpawned)
                Destroy();
        }

        public virtual void Enable() => gameObject.SetActive(true);
        public virtual void Disable() => gameObject.SetActive(false);
        public virtual void Destroy()
        {
            _isSpawned = false;
            PoolReference.Release(this);
        }
    }
}