using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimerUIBar : TimerBase
{
    [SerializeField] private Image _image;
    [SerializeField] private Gradient _gradient;

    [SerializeField] private UnityEvent _onTimeout;

    protected override void OnTimerUpdate(float value)
    {
        _image.fillAmount = value;
        _image.color = _gradient.Evaluate(value);
    }
    protected override void OnCompleteTimer()
    {
        _onTimeout.Invoke();
    }
}