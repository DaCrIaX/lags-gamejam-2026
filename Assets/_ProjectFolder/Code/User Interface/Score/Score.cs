using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class Score : PoolSingleBehaviour<ScoreParticle>
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _score;
    public int CurrentScore => _score;

    public void AddScore(int value)
    {
        var particle = _pool.Get() as ScoreParticle;
        particle.SetText(value);

        _score += value;
        RefreshText();
    }

    public void ResetScore()
    {
        _score = 0;
        RefreshText();
    }

    private void RefreshText() => _text.SetText($"score: {_score}");
}
