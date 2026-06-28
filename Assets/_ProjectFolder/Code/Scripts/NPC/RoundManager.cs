using System;
using System.Collections;
using UnityEngine;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private NPC _character;
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private DifficultyManager _difficultyManager;
    [SerializeField] private CycleQuotaManager _cycleQuotaManager;

    [Header("Controller")]
    [SerializeField] private Score _score;
    [SerializeField] private SuspiciousBar _suspicious;
    [SerializeField] private GameObject _choiceCamera, _choiceArea, _recipeBuildArea;
    [SerializeField] private TimerUIBar _timer;
    [SerializeField, Min(0.1f)] private float _fallbackClientTimeLimit = 10f;

    public event Action<SO_ClientProfile> onSpecialClientAppears;
    public event Action<SO_Recipe> onRecipeDiscovered;
    public event Action onChoiceEvent, onRoundBegin;

    private SO_ClientProfile _currentClientProfile;
    private DishEvaluationResult _lastEvaluationResult;
    private DifficultyRoundData _currentRoundData;
    private int _remainingClientsInRound = int.MaxValue;
    private bool _isCompletingClient;
    private bool _isGameOver;

    private void OnEnable()
    {
        if (_cycleQuotaManager)
        {
            _cycleQuotaManager.onCycleSurvived += OnCycleSurvived;
            _cycleQuotaManager.onGameOver += OnGameOver;
        }
    }

    private void OnDisable()
    {
        if (_cycleQuotaManager)
        {
            _cycleQuotaManager.onCycleSurvived -= OnCycleSurvived;
            _cycleQuotaManager.onGameOver -= OnGameOver;
        }
    }

    private void Start() => StartComponents();
    public SO_ClientProfile GetCurrentClient() => _currentClientProfile;
    public void SendedIngredients(int score) => _score.AddScore(score);
    public void CompleteRound()
    {
        if (_isCompletingClient || _isGameOver) return;

        _isCompletingClient = true;
        _timer.Stop();
        StartCoroutine(LeaveAnimation());
    }
    public void DiscoverRecipe(SO_Recipe recipe) => onRecipeDiscovered?.Invoke(recipe);
    public void SetLastEvaluationResult(DishEvaluationResult result) => _lastEvaluationResult = result;
    public void UpdateSuspicion(int suspicionChange)
    {
        //Comentado sistema de sospecha
        /*if (_currentClientProfile == null)
        {
            _suspicious.AddAmount(suspicionChange);
            return;
        }

        int finalSuspicionChange = ApplyClientModifiers(suspicionChange);
        _suspicious.AddAmount(finalSuspicionChange);*/
    }

    public void StartComponents()
    {
        _timer.gameObject.SetActive(true);

        if (_difficultyManager)
        {
            _difficultyManager.StartCurrentCycle();

            if (!TryStartNextRound())
                return;
        }

        NextClient();
    }

    public void NextClient()
    {
        if (_isGameOver) return;

        _isCompletingClient = false;
        _timer.Play(GetCurrentClientTimeLimit());
        _choiceArea.SetActive(false);
        _choiceCamera.SetActive(false);
        _recipeBuildArea.SetActive(true);

        _currentClientProfile = _clientManager.SelectNextClient();

        if (_currentClientProfile.IsSpecial)
            onSpecialClientAppears?.Invoke(_currentClientProfile);

        StartCoroutine(ShowAnimation());
    }

    public void TimeoutCurrentClient() => CompleteRound();

    private int ApplyClientModifiers(int suspicionChange)
    {
        if (_currentClientProfile == null)
            return suspicionChange;

        // Mapear cambio de sospecha al tipo de plato
        int modifier = 0;

        if (_lastEvaluationResult != null)
        {
            switch (_lastEvaluationResult.Type)
            {
                case DishEvaluationResult.DishType.PerfectMatch:
                    modifier = _currentClientProfile.PerfectMatchSuspicionMod;
                    break;
                case DishEvaluationResult.DishType.CommonDish:
                    modifier = _currentClientProfile.CommonDishSuspicionMod;
                    break;
                case DishEvaluationResult.DishType.InvalidDish:
                    modifier = _currentClientProfile.InvalidDishSuspicionMod;
                    break;
            }
        }

        return suspicionChange + modifier;
    }
    private IEnumerator ShowAnimation()
    {
        //Debug.Log("anim");
        float time = 0f;
        _character.SetClientProfile(_currentClientProfile);

        while (time <= 0.5f)
        {
            yield return null;
            time += Time.deltaTime * _character.Speed;
            _character.SetOnSpline(time);
        }

        _character.SetOnSpline(0.5f);
        onRoundBegin?.Invoke();
    }
    private IEnumerator LeaveAnimation()
    {
        float time = 0.5f;

        while (time < 1f)
        {
            yield return null;
            time += Time.deltaTime * _character.Speed;
            _character.SetOnSpline(time);
        }

        //_choiceCamera.SetActive(true);
        //_recipeBuildArea.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        OnClientCompleted();
    }

    private void OnClientCompleted()
    {
        if (_remainingClientsInRound != int.MaxValue)
            _remainingClientsInRound--;

        if (_remainingClientsInRound > 0)
        {
            onChoiceEvent?.Invoke();
            NextClient();
            return;
        }

        if (TryStartNextRound())
        {
            onChoiceEvent?.Invoke();
            NextClient();
        }
    }

    private bool TryStartNextRound()
    {
        if (!_difficultyManager)
        {
            _remainingClientsInRound = int.MaxValue;
            return true;
        }

        if (!_difficultyManager.TryAdvanceRound(out _currentRoundData))
        {
            _remainingClientsInRound = 0;
            return false;
        }

        _remainingClientsInRound = _currentRoundData.ClientAmount;
        return _remainingClientsInRound > 0;
    }

    private float GetCurrentClientTimeLimit()
    {
        if (_difficultyManager)
            return _difficultyManager.GetClientTimeLimit(_currentRoundData);

        return _fallbackClientTimeLimit;
    }

    private void OnCycleSurvived(CycleEvaluationResult result)
    {
        if (_isGameOver) return;
        if (!TryStartNextRound()) return;

        onChoiceEvent?.Invoke();
        NextClient();
    }

    private void OnGameOver(CycleEvaluationResult result)
    {
        _isGameOver = true;
        _timer.Stop();
        _recipeBuildArea.SetActive(false);
    }
}
