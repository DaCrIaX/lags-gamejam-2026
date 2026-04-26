using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private InputActionReference _pauseButton;
    [SerializeField] private TweenCanvasGroup _fadeAnimation;

    private bool _isPaused;

    private void OnEnable() => _pauseButton.action.performed += PerformeKey;
    private void OnDisable() => _pauseButton.action.performed -= PerformeKey;
    private void PerformeKey(InputAction.CallbackContext ctx)
    {
        if (_isPaused) ButtonUnPause();
        else ButtonPause();
    }

    public void ButtonPause()
    {
        Time.timeScale = 0f;
        
        _isPaused = true;
        _fadeAnimation.FadeIn();
    }
    public void ButtonUnPause()
    {
        Time.timeScale = 1f;

        _isPaused = false;
        _fadeAnimation.FadeOut();
    }
}