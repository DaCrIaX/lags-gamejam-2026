using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Start State")]
    [SerializeField, Min(1)] private int _startingRound = 1;
    [SerializeField, Min(0)] private int _startingScore;

    [Header("Quota")]
    [SerializeField, Min(0)] private int _baseMinimumQuota = 1000;
    [SerializeField, Min(0f)] private float _quotaGrowthPercentPerCycle = 10f;
    [SerializeField, Min(0)] private int _startingFunds;

    [Header("UI")]
    [SerializeField] private Image _quotaProgressImage;
    [SerializeField] private TextMeshProUGUI _quotaStateText;
    [SerializeField] private string _quotaText = "Cuota";
    [SerializeField] private string _cycleRestartText = "Siguiente semana...";
    [SerializeField, Min(0f)] private float _cycleRestartMessageDuration = 3f;

    [Header("Events")]
    [SerializeField] private SceneLoader _gameOverSceneLoader;
    [SerializeField] private UnityEvent _onCycleSurvived;
    [SerializeField] private UnityEvent _onGameOver;

    public event Action<CycleEvaluationResult> onCycleEvaluated;
    public event Action<CycleEvaluationResult> onCycleSurvived;
    public event Action<CycleEvaluationResult> onGameOver;

    private int _funds;
    private bool _isRestartingCycle;

    public int StartingRound => _startingRound;
    public int StartingScore => _startingScore;
    public int Funds => _funds;
    public int BaseMinimumQuota => _baseMinimumQuota;
    public float QuotaGrowthPercentPerCycle => _quotaGrowthPercentPerCycle;
    public int CurrentMinimumQuota => GetMinimumQuotaForCycle(_difficultyManager ? _difficultyManager.CurrentCycle : 1);
    public float CurrentQuotaProgress => GetQuotaProgress();
    public CycleEvaluationResult LastResult { get; private set; }

    private void Awake()
    {
        _funds = _startingFunds;
        ApplyStartingRound();
    }

    private void OnEnable()
    {
        if (_difficultyManager)
        {
            _difficultyManager.onCycleStarted += OnCycleStarted;
            _difficultyManager.onCycleCompleted += EvaluateCycle;
        }

        if (_score)
        {
            _score.onScoreChanged += OnScoreChanged;
        }
    }

    private void OnDisable()
    {
        if (_difficultyManager)
        {
            _difficultyManager.onCycleStarted -= OnCycleStarted;
            _difficultyManager.onCycleCompleted -= EvaluateCycle;
        }

        if (_score)
        {
            _score.onScoreChanged -= OnScoreChanged;
        }
    }

    private void OnCycleStarted(int cycle)
    {
        _isRestartingCycle = false;
        RefreshQuotaStateText(_quotaText);
        _score?.ResetScore(_startingScore);
        RefreshQuotaProgress();
    }

    private void Start()
    {
        RefreshQuotaStateText(_quotaText);
        RefreshQuotaProgress();
    }

    private void OnValidate()
    {
        _startingRound = Mathf.Max(1, _startingRound);
        ApplyStartingRound();
        RefreshQuotaProgress();
    }

    private void OnScoreChanged(int score)
    {
        RefreshQuotaProgress();
    }

    private void ApplyStartingRound()
    {
        if (_difficultyManager == null)
        {
            return;
        }

        _difficultyManager.SetStartingRound(_startingRound);
    }

    public void EvaluateCycle()
    {
        if (_isRestartingCycle)
        {
            return;
        }

        int currentCycle = _difficultyManager ? _difficultyManager.CurrentCycle : 1;
        int minimumQuota = GetMinimumQuotaForCycle(currentCycle);
        int cycleScore = _score ? _score.CurrentScore : 0;
        int fundsBefore = _funds;
        int remainingFunds = fundsBefore + cycleScore - minimumQuota;
        bool survived = remainingFunds >= 0;
        _funds = survived ? remainingFunds : fundsBefore;

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
            StartCoroutine(RestartCycleRoutine(LastResult));
            return;
        }

        _onGameOver?.Invoke();
        onGameOver?.Invoke(LastResult);

        if (_gameOverSceneLoader)
        {
            _gameOverSceneLoader.SwipeScene();
            return;
        }

        Debug.LogWarning($"{nameof(CycleQuotaManager)} detected game over, but no game over scene loader is assigned.", this);
    }

    private IEnumerator RestartCycleRoutine(CycleEvaluationResult result)
    {
        _isRestartingCycle = true;
        RefreshQuotaStateText(_cycleRestartText);

        if (_cycleRestartMessageDuration > 0f)
        {
            yield return new WaitForSeconds(_cycleRestartMessageDuration);
        }

        _difficultyManager?.AdvanceCycle();
        RefreshQuotaProgress();
        _onCycleSurvived?.Invoke();
        onCycleSurvived?.Invoke(result);
    }

    public int GetMinimumQuotaForCycle(int cycle)
    {
        int safeCycle = Mathf.Max(1, cycle);
        float growthFactor = 1f + _quotaGrowthPercentPerCycle * 0.01f;
        return Mathf.RoundToInt(_baseMinimumQuota * Mathf.Pow(growthFactor, safeCycle - 1));
    }

    private void RefreshQuotaProgress()
    {
        if (_quotaProgressImage == null)
        {
            return;
        }

        _quotaProgressImage.fillAmount = CurrentQuotaProgress;
    }

    private void RefreshQuotaStateText(string text)
    {
        if (_quotaStateText == null)
        {
            return;
        }

        _quotaStateText.SetText(text);
    }

    private float GetQuotaProgress()
    {
        int minimumQuota = Mathf.Max(1, CurrentMinimumQuota);
        int cycleScore = _score ? _score.CurrentScore : 0;
        int availableScore = Mathf.Max(0, cycleScore + _funds);

        return Mathf.Clamp01((float)availableScore / minimumQuota);
    }
}
