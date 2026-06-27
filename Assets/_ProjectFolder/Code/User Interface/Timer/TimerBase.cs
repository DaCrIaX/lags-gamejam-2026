using System.Collections;
using UnityEngine;

public abstract class TimerBase : MonoBehaviour
{
    protected float _currentTime;
    protected bool _isPaused;

    protected abstract void OnTimerUpdate(float value);
    protected abstract void OnCompleteTimer();

    private IEnumerator TimerUpdateRoutine()
    {
        float startValue = 1f / _currentTime;

        while (_currentTime > 0)
        {
            if (_isPaused)
            {
                yield return null;
                continue;
            }
            _currentTime -= Time.deltaTime;
            OnTimerUpdate(_currentTime * startValue);

            yield return null;
        }

        OnCompleteTimer();
    }

    public void Play(float value)
    {
        _isPaused = false;
        _currentTime = value;
        StopAllCoroutines();
        StartCoroutine(TimerUpdateRoutine());
    }
    public void Stop() => StopAllCoroutines();
    public void Continue() => StartCoroutine(TimerUpdateRoutine());
    public void PauseTimer() => _isPaused = true;
}