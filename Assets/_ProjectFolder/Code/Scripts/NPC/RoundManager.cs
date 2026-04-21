using System;
using System.Collections;
using UnityEngine;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private NPC _character;

    [Header("Controller")]
    [SerializeField] private Score _score;
    [SerializeField] private GameObject _choiceCamera, _choiceArea, _recipeBuildArea;

    public event Action<SO_Recipe> onRecipeDiscovered;
    public event Action onChoiceEvent, onRoundBegin;

    private void Start()
    {
        _score.SetMaxRound(GameplayManager.Instance.MaxRounds);
        NextRound();
    }

    public void DiscoverRecipe(SO_Recipe recipe) => onRecipeDiscovered?.Invoke(recipe);
    public void SendedIngredients(int score)
    {
        _score.AddScore(score);
        //_timerAnimation.SwipeOut();
        //_timer.Stop();
    }
    public void CompleteRound()
    {
        //if (_score.CurrentRound >= _score.MaxRound)
        //{
        //    _loader.SwipeScene();
        //    return;
        //}

        //_score.NextRound();
        StartCoroutine(LeaveAnimation());
    }
    public void NextRound()
    {
        _choiceArea.SetActive(false);
        _choiceCamera.SetActive(false);
        _recipeBuildArea.SetActive(true);
        StartCoroutine(ShowAnimation());
    }

    private IEnumerator ShowAnimation()
    {
        float time = 0f;
        _character.SetSkinRandom();

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