using System;
using System.Collections;
using UnityEngine;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private NPC _character;
    [SerializeField] private ClientManager _clientManager;

    [Header("Controller")]
    [SerializeField] private Score _score;
    [SerializeField] private SuspiciousBar _suspicious;
    [SerializeField] private GameObject _choiceCamera, _choiceArea, _recipeBuildArea;

    public event Action<SO_ClientProfile> onSpecialClientAppears;
    public event Action<SO_Recipe> onRecipeDiscovered;
    public event Action onChoiceEvent, onRoundBegin;

    private SO_ClientProfile _currentClientProfile;
    private DishEvaluationResult _lastEvaluationResult;

    private void Start() => NextRound();
    public SO_ClientProfile GetCurrentClient() => _currentClientProfile;
    public void SendedIngredients(int score) => _score.AddScore(score);
    public void CompleteRound() => StartCoroutine(LeaveAnimation());
    public void DiscoverRecipe(SO_Recipe recipe) => onRecipeDiscovered?.Invoke(recipe);
    public void SetLastEvaluationResult(DishEvaluationResult result) => _lastEvaluationResult = result;
    public void UpdateSuspicion(int suspicionChange)
    {
        if (_currentClientProfile == null)
        {
            _suspicious.AddAmount(suspicionChange);
            return;
        }

        int finalSuspicionChange = ApplyClientModifiers(suspicionChange);
        _suspicious.AddAmount(finalSuspicionChange);
    }

    public void NextRound()
    {
        _choiceArea.SetActive(false);
        _choiceCamera.SetActive(false);
        _recipeBuildArea.SetActive(true);

        _currentClientProfile = _clientManager.SelectNextClient();

        if (_currentClientProfile.IsSpecial)
            onSpecialClientAppears?.Invoke(_currentClientProfile);

        StartCoroutine(ShowAnimation());
    }

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

        _choiceCamera.SetActive(true);
        _recipeBuildArea.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        _choiceArea.SetActive(true);
        onChoiceEvent?.Invoke();
    }
}