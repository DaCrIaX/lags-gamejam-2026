namespace UnityEngine.Animations
{
    using Events;

    [RequireComponent(typeof(ITweenCallback))]
    public class TweenEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> _onTweenStatusChanged, _onTweenCompleted;
        [SerializeField] private UnityEvent _onAnimationCompleted;

        private void Awake()
        {
            var tween = GetComponent<ITweenCallback>();
            tween.onTweenStatusChanged += _onTweenStatusChanged.Invoke;
            tween.onTweenCompleted += _onTweenCompleted.Invoke;
            tween.onCompleted += _onAnimationCompleted.Invoke;
        }
        private void OnDestroy()
        {
            var tween = GetComponent<ITweenCallback>();
            tween.onTweenCompleted -= _onTweenCompleted.Invoke;
            tween.onTweenStatusChanged -= _onTweenStatusChanged.Invoke;
            tween.onCompleted -= _onAnimationCompleted.Invoke;
        }
    }
}