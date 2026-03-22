namespace UnityEngine.UI
{
    public abstract class HOVLayoutGroupSmooth : HorizontalOrVerticalLayoutGroup
    {
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _duration = 1f;

        protected Vector2[] _origin, _target;
        private bool _initialized, _animating;
        private float _inverseDuration, _time;

        protected void Cache()
        {
            int count = rectChildren.Count;
            if (_target != null && _target.Length == count) return;
            _target = new Vector2[count];
            _origin = new Vector2[count];
        }
        protected void SaveOriginPositions()
        {
            int count = rectChildren.Count;
            for (int i = 0; i < count; i++)
                _origin[i] = rectChildren[i].anchoredPosition;
        }
        protected void CaptureTargetPositions()
        {
            int count = rectChildren.Count;
            for(int i = 0; i < count; i++)
            {
                var rect = rectChildren[i];
                _target[i] = rect.anchoredPosition;

                if (!_initialized) _origin[i] = _target[i];
                rect.anchoredPosition = _origin[i];
            }

            _time = 0f;
            _inverseDuration = 1f / _duration;
            _initialized = _animating = true;
        }

        protected void LateUpdate()
        {
            if (!Application.isPlaying || !_animating) return;
            _time += Time.deltaTime * _inverseDuration;
            _time = Mathf.Clamp01(_time);

            float curveT = _curve.Evaluate(_time);

            int count = rectChildren.Count;
            for (int i = 0; i < count; i++)
                rectChildren[i].anchoredPosition = Vector2.LerpUnclamped(_origin[i], _target[i], curveT);

            if (_time >= 1f) _animating = false;
        }
    }
}