using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private Transform _npc;
    [SerializeField] private float _npcSpeed = 1f;
    [SerializeField] private Material _material;
    [SerializeField] private Sprite[] _images;

    [SerializeField] private Score _score;
    [SerializeField] private TimerBase _timer;
    [SerializeField] private TweenRectPositionSwipe _timerAnimation;
    [SerializeField] private SceneLoader _loader;

    public Action<SO_Recipe> onRecipeDiscovered;
    public Action onNextRound;

    private void Start()
    {
        StartCoroutine(ShowAnimation());
        _score.SetMaxRound(GameplayManager.Instance.MaxRounds);
    }

    public void DiscoverRecipe(SO_Recipe recipe) => onRecipeDiscovered?.Invoke(recipe);
    public void SendedIngredients(int score)
    {
        _score.AddScore(score);
        _timerAnimation.SwipeOut();
        _timer.Stop();
    }
    public void NextRound()
    {
        if (_score.CurrentRound >= _score.MaxRound)
        {
            _loader.SwipeScene();
            return;
        }

        _score.NextRound();
        StartCoroutine(LeaveAnimation());
    }

    private IEnumerator ShowAnimation()
    {
        float time = 0f;
        _material.mainTexture = _images[Random.Range(0, _images.Length)].texture;

        while (time <= 0.5f)
        {
            yield return null;
            time += Time.deltaTime * _npcSpeed;
            _npc.position = _spline.EvaluatePosition(time);
        }

        _npc.position = _spline.EvaluatePosition(0.5f);
    }
    private IEnumerator LeaveAnimation()
    {
        float time = 0.5f;

        while (time < 1f)
        {
            yield return null;
            time += Time.deltaTime * _npcSpeed;
            _npc.position = _spline.EvaluatePosition(time);
        }

        yield return ShowAnimation();
        onNextRound?.Invoke();
        _timerAnimation.SwipeIn();
        _timer.Play(GameplayManager.Instance.ResponseTime);
    }
}