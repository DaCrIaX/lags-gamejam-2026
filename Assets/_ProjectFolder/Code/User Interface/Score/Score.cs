using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class Score : PoolSingleBehaviour<ScoreParticle>
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _score;

    public void AddScore(int value)
    {
        var particle = _pool.Get() as ScoreParticle;
        particle.SetText(value);

        _score += value;
        _text.SetText($"score: {_score}");
    }
}