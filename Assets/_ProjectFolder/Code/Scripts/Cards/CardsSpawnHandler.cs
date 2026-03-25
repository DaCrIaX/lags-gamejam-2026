using System.Collections;
using UnityEngine;

public class CardsSpawnHandler : HOVCardsGroupHandler
{
    [SerializeField] private Card _prefab;

    [SerializeField] private float _startDelay, _spawnDelay;
    [SerializeField] private int _startAmount, _roundAmount;

    private void Start() => StartCoroutine(SpawnCardsRoutine(Mathf.Min(_startAmount, _group.MaxAmount)));
    private void OnEnable() => _roundManager.onNextRound += OnNextRound;
    private void OnDisable() => _roundManager.onNextRound -= OnNextRound;
    private void OnNextRound() => StartCoroutine(SpawnCardsRoutine(_startAmount - _group.Amount));

    private IEnumerator SpawnCardsRoutine(int amount)
    {
        yield return new WaitForSeconds(_startDelay);

        int limit = Mathf.Clamp(amount, 0, _group.MaxAmount);
        if (!_inventory.TryGetIngredients(limit, out var ingredients)) yield break;

        foreach (var item in ingredients)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Instantiate(_prefab, _group.Container).Setup(item);
        }
    }
}