using UnityEngine;
using UnityEngine.Splines;
using System;

public class RoundManager : SingletonBasic<RoundManager>
{
    [SerializeField] private SplineContainer _spline;

    public Action<SO_Recipe> onDiscoveredRecipe;
    public Action onNextRound;

    public void DiscoverRecipe(SO_Recipe recipe) => onDiscoveredRecipe?.Invoke(recipe);
    public void CompleteRound() => onNextRound?.Invoke();
}