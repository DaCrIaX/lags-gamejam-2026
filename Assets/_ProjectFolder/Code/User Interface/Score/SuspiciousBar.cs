using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SuspiciousBar : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Gradient _gradient;

    [SerializeField] private UnityEvent _onBarCompleted;
    private bool _isPlaying = true;

    public void AddAmount(int value)
    {
        _image.fillAmount += value * 0.01f;
        _image.color = _gradient.Evaluate(_image.fillAmount);

        if (_isPlaying && _image.fillAmount >= 1f)
        {
            _onBarCompleted.Invoke();
            _isPlaying = false;
        }
    }
    public void RemoveValue(int value)
    {
        _image.fillAmount -= value * 0.01f;
        _image.fillAmount = Mathf.Clamp01(_image.fillAmount);
        _image.color = _gradient.Evaluate(_image.fillAmount);
    }
}