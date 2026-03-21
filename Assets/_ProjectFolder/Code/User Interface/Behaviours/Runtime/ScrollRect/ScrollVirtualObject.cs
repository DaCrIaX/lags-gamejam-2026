using UnityEngine.Pool;

namespace UnityEngine.UI
{
    public class ScrollVirtualObject : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        public IObjectPool<ScrollVirtualObject> PoolReference { private get; set; }

        protected virtual void Reset() => RectTransform = transform as RectTransform;

        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);
        public void Destroy() => PoolReference.Release(this);
    }
}