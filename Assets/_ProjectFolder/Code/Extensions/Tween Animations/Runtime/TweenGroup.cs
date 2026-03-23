using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Animations
{
    using Events;

    public class TweenGroup : MonoBehaviour
    {
        [SerializeField] private float _timeToStart, _timeInBetween;
        [SerializeField] private bool _playOnAwake;

        [SerializeField] private bool _inverseCallback;
        [SerializeField] private UnityEvent<bool> _onValueChanged;

        private List<ITween> _tweens = new();
        private WaitForSeconds _waitToStart, _waitTimeInBetween;

        private void Awake()
        {
            _waitToStart = new(_timeToStart);
            _waitTimeInBetween = new(_timeInBetween);
        }
        private void OnEnable()
        {
            if (_playOnAwake)
                EnableGroup();
        }

        public void AddListener(ITween tween) => _tweens.Add(tween);
        public void RemoveListener(ITween tween) => _tweens.Remove(tween);

        [ContextMenu("Enable Group")] public void EnableGroup() => SetGroupStatus(true);
        [ContextMenu("Disable Group")] public void DisableGroup() => SetGroupStatus(false);
        public void SetGroupStatus(bool value)
        {
            StopAllCoroutines();
            StartCoroutine(TweenDelay(value));
        }

        private IEnumerator TweenDelay(bool value)
        {
            yield return _waitToStart;
            _onValueChanged?.Invoke(_inverseCallback ? !value : value);

            foreach (var tween in _tweens)
            {
                tween.ForcePlay(value);

                if (_timeInBetween != 0)
                    yield return _waitTimeInBetween;
            }
        }
    }
}