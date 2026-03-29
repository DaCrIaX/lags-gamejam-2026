using UnityEngine;
using TMPro;
using UnityEngine.Animations;
using System.Collections;

public class History : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TweenCanvasGroup _fadeAnimation;
    [SerializeField, TextArea(1, 10)] private string[] _pages;

    private int _index = 0;

    private void Start() => UpdateUI();
    private void UpdateUI() => _text.SetText(_pages[_index]);

    private IEnumerator UpdateUIRoutine()
    {
        _fadeAnimation.FadeOut();
        yield return new WaitForSeconds(_fadeAnimation.Duration);
        UpdateUI();
        _fadeAnimation.FadeIn();
    }

    public void Next()
    {
        if (_index >= _pages.Length - 1) return;
        _index++;
        StopAllCoroutines();
        StartCoroutine(UpdateUIRoutine());
    }
    public void Previus()
    {
        if (_index <= 0) return;
        _index--;
        StopAllCoroutines();
        StartCoroutine(UpdateUIRoutine());
    }
}