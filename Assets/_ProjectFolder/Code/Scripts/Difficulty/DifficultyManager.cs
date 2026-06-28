using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ComplexityLevel
{
    Bajo,
    Medio,
    Alto,
    Rush
}

[Serializable]
public class ClientAmountRange
{
    [SerializeField, Min(0)] private int _min;
    [SerializeField, Min(0)] private int _max;

    public int Min => _min;
    public int Max => Mathf.Max(_min, _max);

    public ClientAmountRange(int min, int max)
    {
        _min = min;
        _max = max;
    }

    public int GetRandomAmount() => UnityEngine.Random.Range(Min, Max + 1);
}

[Serializable]
public class ClientTimeRange
{
    [SerializeField, Min(0.1f)] private float _min;
    [SerializeField, Min(0.1f)] private float _max;

    public float Min => _min;
    public float Max => Mathf.Max(_min, _max);

    public ClientTimeRange(float min, float max)
    {
        _min = min;
        _max = max;
    }

    public float GetRandomTime() => UnityEngine.Random.Range(Min, Max);
}

[Serializable]
public class DifficultyRoundData
{
    [SerializeField] private int _cycle;
    [SerializeField] private int _roundNumber;
    [SerializeField] private float _intensity;
    [SerializeField] private ComplexityLevel _complexity;
    [SerializeField] private int _clientAmount;
    [SerializeField] private float _clientTimeLimit;

    public int Cycle => _cycle;
    public int RoundNumber => _roundNumber;
    public float Intensity => _intensity;
    public ComplexityLevel Complexity => _complexity;
    public int ClientAmount => _clientAmount;
    public float ClientTimeLimit => _clientTimeLimit;

    public DifficultyRoundData(int cycle, int roundNumber, float intensity)
    {
        _cycle = cycle;
        _roundNumber = roundNumber;
        _intensity = intensity;
    }

    public void SetComplexity(ComplexityLevel complexity) => _complexity = complexity;
    public void SetClientAmount(int clientAmount) => _clientAmount = clientAmount;
    public void SetClientTimeLimit(float clientTimeLimit) => _clientTimeLimit = clientTimeLimit;
}

public class DifficultyManager : SingletonBasic<DifficultyManager>
{
    [Header("Cycle")]
    [SerializeField, Min(1)] private int _startingCycle = 1;
    [SerializeField, Range(4, 7)] private int _roundsPerCycle = 6;
    [SerializeField, Range(0f, 1f)] private float _maxCyclePhaseShift;
    [SerializeField] private AnimationCurve _intensityCurve = new(
        new Keyframe(0f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f));

    [Header("Client Amount Per Complexity")]
    [SerializeField] private ClientAmountRange _lowClientRange = new(3, 5);
    [SerializeField] private ClientAmountRange _mediumClientRange = new(6, 10);
    [SerializeField] private ClientAmountRange _highClientRange = new(11, 15);
    [SerializeField] private ClientAmountRange _rushClientRange = new(16, 25);

    [Header("Client Time")]
    [SerializeField] private bool _useVariableClientTime = true;
    [SerializeField, Min(0.1f)] private float _fixedClientTimeLimit = 10f;

    [Header("Variable Client Time Per Complexity")]
    [SerializeField] private ClientTimeRange _lowClientTimeRange = new(17f, 20f);
    [SerializeField] private ClientTimeRange _mediumClientTimeRange = new(12f, 15f);
    [SerializeField] private ClientTimeRange _highClientTimeRange = new(8f, 11f);
    [SerializeField] private ClientTimeRange _rushClientTimeRange = new(5f, 7f);

    public event Action<int> onCycleStarted;
    public event Action<DifficultyRoundData> onRoundStarted;
    public event Action onCycleCompleted;

    private readonly List<DifficultyRoundData> _rounds = new();
    private int _currentCycle;
    private int _currentRoundIndex = -1;
    private float _currentCyclePhaseShift;

    public int CurrentCycle => _currentCycle;
    public int StartingCycle => _startingCycle;
    public int RoundsPerCycle => _roundsPerCycle;
    public bool UseVariableClientTime => _useVariableClientTime;
    public float FixedClientTimeLimit => _fixedClientTimeLimit;
    public float CurrentClientTimeLimit => GetClientTimeLimit(CurrentRound);
    public float CurrentCyclePhaseShift => _currentCyclePhaseShift;
    public DifficultyRoundData CurrentRound { get; private set; }
    public IReadOnlyList<DifficultyRoundData> Rounds => _rounds;

    public IReadOnlyList<DifficultyRoundData> BuildPreviewRounds(float phaseShift)
    {
        int cycle = Application.isPlaying && _currentCycle > 0 ? _currentCycle : _startingCycle;
        return CreateCycleRounds(cycle, Mathf.Repeat(phaseShift, 1f), false);
    }

    public ClientAmountRange GetClientRange(ComplexityLevel complexity)
    {
        return complexity switch
        {
            ComplexityLevel.Bajo => _lowClientRange,
            ComplexityLevel.Medio => _mediumClientRange,
            ComplexityLevel.Alto => _highClientRange,
            ComplexityLevel.Rush => _rushClientRange,
            _ => _lowClientRange
        };
    }

    public ClientTimeRange GetClientTimeRange(ComplexityLevel complexity)
    {
        return complexity switch
        {
            ComplexityLevel.Bajo => _lowClientTimeRange,
            ComplexityLevel.Medio => _mediumClientTimeRange,
            ComplexityLevel.Alto => _highClientTimeRange,
            ComplexityLevel.Rush => _rushClientTimeRange,
            _ => _lowClientTimeRange
        };
    }

    public float GetClientTimeLimit(DifficultyRoundData round)
    {
        if (!_useVariableClientTime || round == null)
            return _fixedClientTimeLimit;

        return round.ClientTimeLimit;
    }

    public void StartCurrentCycle()
    {
        _currentCycle = Mathf.Max(1, _startingCycle);
        GenerateCycle();
        onCycleStarted?.Invoke(_currentCycle);
    }

    public void AdvanceCycle()
    {
        _currentCycle++;
        GenerateCycle();
        onCycleStarted?.Invoke(_currentCycle);
    }

    public bool TryAdvanceRound(out DifficultyRoundData round)
    {
        if (_rounds.Count == 0)
            GenerateCycle();

        _currentRoundIndex++;

        if (_currentRoundIndex >= _rounds.Count)
        {
            CurrentRound = null;
            round = null;
            onCycleCompleted?.Invoke();
            return false;
        }

        CurrentRound = _rounds[_currentRoundIndex];
        round = CurrentRound;
        onRoundStarted?.Invoke(CurrentRound);
        return true;
    }

    private void GenerateCycle()
    {
        _rounds.Clear();
        _currentRoundIndex = -1;
        CurrentRound = null;
        _currentCyclePhaseShift = UnityEngine.Random.Range(0f, _maxCyclePhaseShift);
        _rounds.AddRange(CreateCycleRounds(_currentCycle, _currentCyclePhaseShift, true));
    }

    private float GetIntensity(int roundNumber, int totalRounds, float normalizedPhaseShift)
    {
        if (totalRounds <= 1)
            return 1f;

        float position = (roundNumber - 1f) / (totalRounds - 1f);
        float shiftedPosition = Mathf.Repeat(position + normalizedPhaseShift, 1f);

        return Mathf.Clamp01(_intensityCurve.Evaluate(shiftedPosition));
    }

    private List<DifficultyRoundData> CreateCycleRounds(int cycle, float phaseShift, bool randomizeClientAmount)
    {
        var rounds = new List<DifficultyRoundData>();

        for (int i = 1; i <= _roundsPerCycle; i++)
        {
            float intensity = GetIntensity(i, _roundsPerCycle, phaseShift);
            rounds.Add(new DifficultyRoundData(cycle, i, intensity));
        }

        AssignComplexitiesByIntensity(rounds, cycle);

        foreach (var round in rounds)
        {
            var clientRange = GetClientRange(round.Complexity);
            var timeRange = GetClientTimeRange(round.Complexity);
            int clientAmount = randomizeClientAmount ? clientRange.GetRandomAmount() : Mathf.RoundToInt((clientRange.Min + clientRange.Max) * 0.5f);
            float clientTimeLimit = GetRoundClientTimeLimit(timeRange, randomizeClientAmount);
            round.SetClientAmount(clientAmount);
            round.SetClientTimeLimit(clientTimeLimit);
        }

        return rounds;
    }

    private void AssignComplexitiesByIntensity(List<DifficultyRoundData> rounds, int cycle)
    {
        var orderedRounds = rounds
            .OrderByDescending(round => round.Intensity)
            .ThenByDescending(round => round.RoundNumber)
            .ToList();

        if (cycle >= 4)
        {
            AssignRushCycleComplexities(orderedRounds);
            return;
        }

        AssignBaseCycleComplexities(orderedRounds);
    }

    private void AssignBaseCycleComplexities(List<DifficultyRoundData> orderedRounds)
    {
        GetBalancedCounts(_roundsPerCycle, out int lowCount, out int mediumCount, out int highCount);

        int index = 0;
        AssignComplexity(orderedRounds, ref index, highCount, ComplexityLevel.Alto);
        AssignComplexity(orderedRounds, ref index, mediumCount, ComplexityLevel.Medio);
        AssignComplexity(orderedRounds, ref index, lowCount, ComplexityLevel.Bajo);
    }

    private void AssignRushCycleComplexities(List<DifficultyRoundData> orderedRounds)
    {
        int index = 0;
        AssignComplexity(orderedRounds, ref index, 1, ComplexityLevel.Rush);

        GetBalancedCounts(_roundsPerCycle - 1, out int lowCount, out int mediumCount, out int highCount);
        AssignComplexity(orderedRounds, ref index, highCount, ComplexityLevel.Alto);
        AssignComplexity(orderedRounds, ref index, mediumCount, ComplexityLevel.Medio);
        AssignComplexity(orderedRounds, ref index, lowCount, ComplexityLevel.Bajo);
    }

    private static void GetBalancedCounts(int amount, out int lowCount, out int mediumCount, out int highCount)
    {
        int baseCount = amount / 3;
        int remainder = amount % 3;

        lowCount = baseCount;
        mediumCount = baseCount;
        highCount = baseCount;

        if (remainder >= 1)
            mediumCount++;

        if (remainder >= 2)
            lowCount++;
    }

    private static void AssignComplexity(
        List<DifficultyRoundData> orderedRounds,
        ref int index,
        int count,
        ComplexityLevel complexity)
    {
        for (int i = 0; i < count && index < orderedRounds.Count; i++, index++)
            orderedRounds[index].SetComplexity(complexity);
    }

    private float GetRoundClientTimeLimit(ClientTimeRange timeRange, bool randomizeClientTime)
    {
        if (!_useVariableClientTime)
            return _fixedClientTimeLimit;

        return randomizeClientTime ? timeRange.GetRandomTime() : (timeRange.Min + timeRange.Max) * 0.5f;
    }
}
