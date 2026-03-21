using System;
using PrimeTween;

namespace UnityEngine.Animations
{
    [DefaultExecutionOrder(100)]
    public class TweenCore : MonoBehaviour, ITween, ITweenCallback
    {
        [SerializeField] private TweenGroup _group;
        [SerializeField] private bool _startDisable, _playOnAwake;
        [SerializeField] private TweenSettings _settings;

        public TweenSettings Settings => _settings;
        public bool IsEnabled { get; set; }

        public event Action<bool> onTweenStatusChanged, onTweenCompleted;
        public event Action onEnabled, onDisabled;
        public event Action onCompleted;

        private void Awake() => _group?.AddListener(this);
        private void OnDestroy() => _group?.RemoveListener(this);

        private async void OnEnable()
        {
            IsEnabled = !_startDisable;
            onEnabled?.Invoke();

            await Awaitable.NextFrameAsync();
            if (_playOnAwake) Play(!IsEnabled);
        }
        private void OnDisable() => onDisabled?.Invoke();

        public void Play(bool value)
        {
            if (value == IsEnabled) return;

            onTweenStatusChanged?.Invoke(value);
            IsEnabled = value;
        }
        public void ForcePlay(bool value)
        {
            IsEnabled = !value;
            Play(value);
        }
        public void OnComplete()
        {
            onTweenCompleted?.Invoke(IsEnabled);
            onCompleted?.Invoke();
        }
    }
}