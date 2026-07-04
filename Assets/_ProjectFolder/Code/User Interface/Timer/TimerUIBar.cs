using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TimerUIBar : TimerBase
{
    [SerializeField] private Image _image;
    [SerializeField] private Gradient _gradient;

    [Header("Z Rotation")]
    [FormerlySerializedAs("_zMovementTarget")]
    [SerializeField] private Transform _zRotationTarget;
    [FormerlySerializedAs("_zAtFull")]
    [SerializeField] private float _zRotationAtFull = 0f;
    [FormerlySerializedAs("_zAtEmpty")]
    [SerializeField] private float _zRotationAtEmpty = 360f;
    [FormerlySerializedAs("_useLocalPosition")]
    [SerializeField] private bool _useLocalRotation = true;

    [SerializeField] private UnityEvent _onTimeout;

    protected override void OnTimerUpdate(float value)
    {
        _image.fillAmount = value;
        _image.color = _gradient.Evaluate(value);
        UpdateZRotation(value);
    }
    protected override void OnCompleteTimer()
    {
        OnTimerUpdate(0f);
        _onTimeout.Invoke();
    }

    private void UpdateZRotation(float fillAmount)
    {
        if (_zRotationTarget == null)
        {
            return;
        }

        float zRotation = Mathf.Lerp(_zRotationAtEmpty, _zRotationAtFull, fillAmount);

        if (_useLocalRotation)
        {
            Vector3 localEulerAngles = _zRotationTarget.localEulerAngles;
            localEulerAngles.z = zRotation;
            _zRotationTarget.localEulerAngles = localEulerAngles;
            return;
        }

        Vector3 eulerAngles = _zRotationTarget.eulerAngles;
        eulerAngles.z = zRotation;
        _zRotationTarget.eulerAngles = eulerAngles;
    }
}
