using System.Collections.Generic;

namespace UnityEngine.Pool
{
    public abstract class PoolBehaviour : MonoBehaviour
    {
        [SerializeField] protected Transform _parent;
        protected readonly List<PoolObjectBehaviour> _itemsSpawned = new();

        public IList<PoolObjectBehaviour> ItemsSpawned => _itemsSpawned;
        public int LastIndex => _itemsSpawned.Count - 1;

        protected virtual void OnGet(PoolObjectBehaviour @object)
        {
            @object.Enable();
            _itemsSpawned?.Add(@object);
        }
        protected virtual void OnRelease(PoolObjectBehaviour @object)
        {
            @object.Disable();
            _itemsSpawned?.Remove(@object);
        }
        protected virtual void OnDestroyObject(PoolObjectBehaviour @object)
        {
            Destroy(@object.gameObject);
        }
        protected void Clear()
        {
            for (int i = LastIndex; i >= 0; i--)
                _itemsSpawned[i].Destroy();
        }
    }

    public abstract class PoolSingleBehaviour<T> : PoolBehaviour where T : PoolObjectBehaviour
    {
        [SerializeField] protected T _prefab;
        protected ObjectPool<PoolObjectBehaviour> _pool;

        protected virtual void Awake() => _pool = new(OnCreate, OnGet, OnRelease, OnDestroyObject);
        protected virtual T OnCreate()
        {
            var obj = Instantiate(_prefab, _parent);
            obj.PoolReference = _pool;
            return obj;
        }
    }

    public abstract class PoolMultipleBehaviuour<T> : PoolBehaviour where T : PoolObjectBehaviour
    {
        [SerializeField] protected T[] _prefabs;
        protected Dictionary<T, ObjectPool<PoolObjectBehaviour>> _pools = new();

        protected virtual void Awake()
        {
            foreach (var prefab in _prefabs)
                _pools.Add(prefab, new(() => OnCreate(prefab), OnGet, OnRelease, OnDestroyObject));
        }
        protected virtual T OnCreate(T prefab)
        {
            var obj = Instantiate(prefab, _parent);
            obj.PoolReference = _pools[prefab];
            return obj;
        }
        protected PoolObjectBehaviour GetPrefabRandom()
        {
            var prefab = _prefabs[Random.Range(0, _prefabs.Length)];
            return _pools[prefab].Get();
        }
    }
}