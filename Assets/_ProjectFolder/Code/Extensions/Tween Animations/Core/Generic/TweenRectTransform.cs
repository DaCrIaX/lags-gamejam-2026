namespace UnityEngine.Animations
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class TweenRectTransform : TweenBehaviour<Vector3>
    {
        protected RectTransform _rectTransform;
        protected Vector3 _from, _to;

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = transform as RectTransform;
        }
    }
}