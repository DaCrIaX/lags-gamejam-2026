using System;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class Score : PoolSingleBehaviour<ScoreParticle>
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _score;
    public int CurrentScore => _score;
    public event Action<int> onScoreChanged;

    public void AddScore(int value)
    {
        var particle = _pool.Get() as ScoreParticle;
        particle.SetText(value);

        _score += value;
        RefreshText();
        onScoreChanged?.Invoke(_score);
    }

    public void ResetScore()
    {
        _score = 0;
        RefreshText();
        onScoreChanged?.Invoke(_score);
    }

    private void RefreshText() => _text.SetText($"score: {_score}");
}
