using System.Collections;
using UnityEngine;

public abstract class TimerBase : MonoBehaviour
{
    protected float _currentTime;

    protected abstract void OnTimerUpdate(float value);
    protected abstract void OnCompleteTimer();

    private IEnumerator TimerUpdateRoutine()
    {
        float startValue = 1f / _currentTime;

        while (_currentTime > 0)
        {
            yield return null;
            _currentTime -= Time.deltaTime;
            OnTimerUpdate(_currentTime * startValue);
        }

        OnCompleteTimer();
    }

    public void Play(float value)
    {
        _currentTime = value;
        StopAllCoroutines();
        StartCoroutine(TimerUpdateRoutine());
    }
    public void Stop() => StopAllCoroutines();
    public void Continue() => StartCoroutine(TimerUpdateRoutine());
}