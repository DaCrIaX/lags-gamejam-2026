using System.Collections;
using UnityEngine;

public abstract class TimerBase : MonoBehaviour
{
    protected float _currentTime;

    protected abstract void OnTimerUpdate(float value);
    private IEnumerator TimerUpdateRoutine()
    {
        float startValue = 1f / _currentTime;

        while (_currentTime > 0)
        {
            yield return null;
            _currentTime -= Time.deltaTime;
            OnTimerUpdate(_currentTime * startValue);
        }
    }

    public void SetTimer(float value)
    {
        _currentTime = value;
        StopAllCoroutines();
        StartCoroutine(TimerUpdateRoutine());
    }
}