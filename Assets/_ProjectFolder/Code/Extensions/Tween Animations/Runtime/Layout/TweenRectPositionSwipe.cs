namespace UnityEngine.Animations
{
    public class TweenRectPositionSwipe : TweenRectPosition
    {
        protected override void OnStart() => _rectTransform.localPosition = !_tweenCore.IsEnabled ? _to : _from;
        public void SetPosition(Vector2 position) => _rectTransform.position = position - _overrideDistance;

        [ContextMenu("SwipeIn")] public void SwipeIn() => _tweenCore?.Play(true);
        [ContextMenu("SwipeOut")] public void SwipeOut() => _tweenCore?.Play(false);
        [ContextMenu("ForceSwipeIn")] public void ForceSwipeIn() => _tweenCore?.ForcePlay(true);
        [ContextMenu("ForceSwipeOut")] public void ForceSwipeOut() => _tweenCore?.ForcePlay(false);
    }
}