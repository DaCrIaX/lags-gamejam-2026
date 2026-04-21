using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CardsSpawnHandler : HOVCardsGroupHandler
{
    [SerializeField] private AudioEmitter _audioSpawn;
    [SerializeField] private Card _prefab;

    private void OnEnable() => _roundManager.onChoiceEvent += OnNextRound;
    private void OnDisable() => _roundManager.onChoiceEvent -= OnNextRound;
    private void OnNextRound() => StartCoroutine(SpawnCardsRoutine(_manager.RoundAmount - _group.Amount));

    private IEnumerator SpawnCardsRoutine(int amount)
    {
        yield return new WaitForSeconds(_manager.StartDelay);

        int limit = Mathf.Clamp(amount, 0, _group.MaxAmount);
        if (!_inventory.TryGetIngredients(limit, out var ingredients)) yield break;

        _audioSpawn.PlayOneShot();

        foreach (var item in ingredients)
        {
            yield return new WaitForSeconds(_manager.SpawnDelay);
            Instantiate(_prefab, _group.Container).Setup(item);
        }
    }
}