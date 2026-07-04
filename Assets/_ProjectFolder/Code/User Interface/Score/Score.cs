using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Min(0f)] private float _deltaDisplayDuration = 0.75f;
    [SerializeField, Min(1)] private int _scoreDigits = 7;

    private int _score;
    private Coroutine _showTotalCoroutine;

    public int CurrentScore => _score;
    public event Action<int> onScoreChanged;

    private void Awake()
    {
        CacheTextReference();
        RefreshText();
    }

    private void OnValidate()
    {
        _scoreDigits = Mathf.Max(1, _scoreDigits);
        CacheTextReference();
        RefreshText();
    }

    public void AddScore(int value)
    {
        _score += value;
        ShowDelta(value);
        onScoreChanged?.Invoke(_score);
    }

    public void ResetScore()
    {
        _score = 0;
        StopShowTotalCoroutine();
        RefreshText();
        onScoreChanged?.Invoke(_score);
    }

    private void RefreshText()
    {
        CacheTextReference();

        if (_text != null)
        {
            _text.SetText(FormatTotalScore());
        }
    }

    private string FormatTotalScore()
    {
        int digits = Mathf.Max(7, _scoreDigits);

        if (_score < 0)
        {
            return $"-{Mathf.Abs(_score).ToString().PadLeft(digits, '0')}";
        }

        return _score.ToString().PadLeft(digits, '0');
    }

    private void ShowDelta(int value)
    {
        CacheTextReference();
        StopShowTotalCoroutine();

        if (_text != null)
        {
            _text.SetText(Mathf.Abs(value).ToString());
        }

        _showTotalCoroutine = StartCoroutine(ShowTotalAfterDelay());
    }

    private IEnumerator ShowTotalAfterDelay()
    {
        yield return new WaitForSeconds(_deltaDisplayDuration);
        _showTotalCoroutine = null;
        RefreshText();
    }

    private void CacheTextReference()
    {
        if (_text == null)
        {
            TryGetComponent(out _text);
        }
    }

    private void StopShowTotalCoroutine()
    {
        if (_showTotalCoroutine == null)
        {
            return;
        }

        StopCoroutine(_showTotalCoroutine);
        _showTotalCoroutine = null;
    }
}
