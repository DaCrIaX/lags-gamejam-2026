using PrimeTween;

namespace UnityEngine.Animations
{
    public abstract class TweenPosition : TweenTransform
    {
        [SerializeField] protected Direction _direction;
        [SerializeField] protected float _distance = 1f;

        protected override void Awake()
        {
            base.Awake();
            var direction = _direction.Get();

            _from = _to = _transform.localPosition;
            _to += direction * _distance;
        }

        protected override void OnPlay(bool value)
        {
            base.OnPlay(value);

            _tweenSettings = new(_transform.localPosition, value ? _from : _to, _settings);
            _tween = Tween.LocalPosition(_transform, _tweenSettings);
            _tween.OnComplete(OnComplete);
        }
    }
}