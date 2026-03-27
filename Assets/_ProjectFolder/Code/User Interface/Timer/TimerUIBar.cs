using UnityEngine;
using UnityEngine.UI;

public class TimerUIBar : TimerBase
{
    [SerializeField] private Image _image;

    protected override void OnTimerUpdate(float value)
    {
        _image.fillAmount = value;
    }
}