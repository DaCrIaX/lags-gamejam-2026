namespace UnityEngine.Animations
{
    public class TweenRectPositionSwipe : TweenRectPosition
    {
        protected override void OnStart() => _rectTransform.localPosition = !_tweenCore.IsEnabled ? _to : _from;

        [ContextMenu("SwipeIn")] public void SwipeIn() => _tweenCore?.Play(true);
        [ContextMenu("SwipeOut")] public void SwipeOut() => _tweenCore?.Play(false);
    }
}