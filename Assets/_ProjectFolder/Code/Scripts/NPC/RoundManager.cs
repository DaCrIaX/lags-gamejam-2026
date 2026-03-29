using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private Transform _npc;
    [SerializeField] private float _speed = 1f;

    public Action<SO_Recipe> onRecipeDiscovered;
    public Action onNextRound;

    private void Start() => StartCoroutine(ShowAnimation());
    public void DiscoverRecipe(SO_Recipe recipe) => onRecipeDiscovered?.Invoke(recipe);
    public void CompleteRound() => StartCoroutine(LeaveAnimation());

    private IEnumerator ShowAnimation()
    {
        float time = 0f;

        while (time <= 0.5f)
        {
            yield return null;
            time += Time.deltaTime * _speed;
            _npc.position = _spline.EvaluatePosition(time);
        }
        
    }
    private IEnumerator LeaveAnimation()
    {
        float time = 0.5f;

        while (time < 1f)
        {
            yield return null;
            time += Time.deltaTime * _speed;
            _npc.position = _spline.EvaluatePosition(time);
        }

        yield return ShowAnimation();
        onNextRound?.Invoke();
    }
}