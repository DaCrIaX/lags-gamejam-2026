using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class CardsSpawnHandler : HOVCardsGroupHandler
{
    [SerializeField] private AudioEmitter _audioSpawn;
    [SerializeField] private Card _prefab;
    [SerializeField] private bool _clearExistingCardsOnStart;
    [SerializeField] private List<SO_IngredientBase> _startingHand = new List<SO_IngredientBase>();

    public bool IsRandom { private get; set; }
    public event Action<int> onCardsSpawned;

    private void OnEnable() => _roundManager.onChoiceEvent += OnNextRound;
    private void OnDisable() => _roundManager.onChoiceEvent -= OnNextRound;

    private void Start()
    {
        if (_clearExistingCardsOnStart)
        {
            _group.ClearChildren();
        }
    }

    public void SpawnStartingHand()
    {
        if (_startingHand.Count > 0)
        {
            StartCoroutine(SpawnSpecificCardsRoutine(_startingHand));
            return;
        }

        SpawnCardsUntilAmount(_manager.StartAmount);
    }

    public void SpawnRoundHand()
    {
        SpawnCardsUntilAmount(_manager.RoundAmount);
    }

    private void OnNextRound()
    {
        int amount = IsRandom ? Random.Range(2, _manager.RoundAmount) : _manager.RoundAmount;
        SpawnCardsUntilAmount(amount);
    }

    private void SpawnCardsUntilAmount(int targetAmount)
    {
        StartCoroutine(SpawnCardsRoutine(targetAmount - _group.Amount));
    }

    private IEnumerator SpawnCardsRoutine(int amount)
    {
        yield return new WaitForSeconds(_manager.StartDelay);

        int limit = Mathf.Clamp(amount, 0, _group.MaxAmount);
        if (!_inventory.TryGetIngredients(limit, out var ingredients)) yield break;

        _audioSpawn.PlayOneShot();

        int spawnedAmount = 0;
        foreach (var item in ingredients)
        {
            yield return new WaitForSeconds(_manager.SpawnDelay);
            Instantiate(_prefab, _group.Container).Setup(item);
            spawnedAmount++;
        }

        onCardsSpawned?.Invoke(spawnedAmount);
    }

    private IEnumerator SpawnSpecificCardsRoutine(IReadOnlyList<SO_IngredientBase> ingredients)
    {
        yield return new WaitForSeconds(_manager.StartDelay);

        int limit = Mathf.Min(ingredients.Count, _group.MaxAmount - _group.Amount);
        if (limit <= 0)
        {
            yield break;
        }

        _audioSpawn.PlayOneShot();

        int spawnedAmount = 0;
        for (int i = 0; i < limit; i++)
        {
            SO_IngredientBase ingredient = ingredients[i];
            if (ingredient == null)
            {
                continue;
            }

            yield return new WaitForSeconds(_manager.SpawnDelay);
            Instantiate(_prefab, _group.Container).Setup(ingredient);
            spawnedAmount++;
        }

        onCardsSpawned?.Invoke(spawnedAmount);
    }
}
