using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _round, _maxRounds, _score;

    public int CurrentRound => _round;
    public int MaxRound => _maxRounds;

    public void AddScore(int value)
    {
        _score += value;
        UpdateUI();
    }
    public void NextRound()
    {
        _round++;
        UpdateUI();
    }
    public void SetMaxRound(int maxRound)
    {
        _maxRounds = maxRound;
        UpdateUI();
    }

    private void UpdateUI() => _text.SetText($"score: {_score}\nround: {_round}/{_maxRounds}");
}
