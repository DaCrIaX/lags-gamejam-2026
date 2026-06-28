using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CycleEvaluationResult
{
    [SerializeField] private int _cycle;
    [SerializeField] private int _cycleScore;
    [SerializeField] private int _minimumQuota;
    [SerializeField] private int _fundsBefore;
    [SerializeField] private int _fundsAfter;
    [SerializeField] private bool _survived;

    public int Cycle => _cycle;
    public int CycleScore => _cycleScore;
    public int MinimumQuota => _minimumQuota;
    public int FundsBefore => _fundsBefore;
    public int FundsAfter => _fundsAfter;
    public bool Survived => _survived;

    public CycleEvaluationResult(
        int cycle,
        int cycleScore,
        int minimumQuota,
        int fundsBefore,
        int fundsAfter,
        bool survived)
    {
        _cycle = cycle;
        _cycleScore = cycleScore;
        _minimumQuota = minimumQuota;
        _fundsBefore = fundsBefore;
        _fundsAfter = fundsAfter;
        _survived = survived;
    }
}

public class CycleQuotaManager : MonoBehaviour
{
    [SerializeField] private DifficultyManager _difficultyManager;
    [SerializeField] private Score _score;

    [Header("Quota")]
    [SerializeField, Min(0)] private int _baseMinimumQuota = 1000;
    [SerializeField, Min(0f)] private float _quotaGrowthPercentPerCycle = 10f;
    [SerializeField, Min(0)] private int _startingFunds;

    [Header("Events")]
    [SerializeField] private UnityEvent _onCycleSurvived;
    [SerializeField] private UnityEvent _onGameOver;

    public event Action<CycleEvaluationResult> onCycleEvaluated;
    public event Action<CycleEvaluationResult> onCycleSurvived;
    public event Action<CycleEvaluationResult> onGameOver;

    private int _funds;

    public int Funds => _funds;
    public int BaseMinimumQuota => _baseMinimumQuota;
    public float QuotaGrowthPercentPerCycle => _quotaGrowthPercentPerCycle;
    public int CurrentMinimumQuota => GetMinimumQuotaForCycle(_difficultyManager ? _difficultyManager.CurrentCycle : 1);
    public CycleEvaluationResult LastResult { get; private set; }

    private void Awake()
    {
        _funds = _startingFunds;
    }

    private void OnEnable()
    {
        if (_difficultyManager)
        {
            _difficultyManager.onCycleStarted += OnCycleStarted;
            _difficultyManager.onCycleCompleted += EvaluateCycle;
        }
    }

    private void OnDisable()
    {
        if (_difficultyManager)
        {
            _difficultyManager.onCycleStarted -= OnCycleStarted;
            _difficultyManager.onCycleCompleted -= EvaluateCycle;
        }
    }

    private void OnCycleStarted(int cycle)
    {
        _score?.ResetScore();
    }

    public void EvaluateCycle()
    {
        int currentCycle = _difficultyManager ? _difficultyManager.CurrentCycle : 1;
        int minimumQuota = GetMinimumQuotaForCycle(currentCycle);
        int cycleScore = _score ? _score.CurrentScore : 0;
        int fundsBefore = _funds;
        int availableScore = cycleScore + _funds;
        bool survived = availableScore >= minimumQuota;

        if (survived)
        {
            if (cycleScore >= minimumQuota)
                _funds += cycleScore - minimumQuota;
            else
                _funds -= minimumQuota - cycleScore;
        }

        LastResult = new CycleEvaluationResult(
            currentCycle,
            cycleScore,
            minimumQuota,
            fundsBefore,
            _funds,
            survived);

        onCycleEvaluated?.Invoke(LastResult); // por ahora esto no hace nada, quiero ver si luego hay algo al final de la ronda que mostrar en UI para que este evento se ejecute

        if (survived)
        {
            _difficultyManager?.AdvanceCycle();
            _onCycleSurvived?.Invoke();
            onCycleSurvived?.Invoke(LastResult);
            return;
        }

        _onGameOver?.Invoke();
        onGameOver?.Invoke(LastResult);
    }

    public int GetMinimumQuotaForCycle(int cycle)
    {
        int safeCycle = Mathf.Max(1, cycle);
        float growthFactor = 1f + _quotaGrowthPercentPerCycle * 0.01f;
        return Mathf.RoundToInt(_baseMinimumQuota * Mathf.Pow(growthFactor, safeCycle - 1));
    }
}
