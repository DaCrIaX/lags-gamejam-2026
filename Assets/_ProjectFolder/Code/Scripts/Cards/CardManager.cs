using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    [SerializeReference] private SO_Database _database;
    [SerializeField] private CardGroup _handGroup;
    [SerializeField] private Card _prefab;

    [SerializeField] private float _startDelay, _spawnDelay;
    [SerializeField] private int _giveAmount;

    [ContextMenu("Add New Cards")]
    public void AddNewCards() => StartCoroutine(SpawnCards(_giveAmount));

    private IEnumerator SpawnCards(int amount)
    {
        int limit = Mathf.Min(amount, _handGroup.MaxAmount) - _handGroup.Amount;

        yield return new WaitForSeconds(_startDelay);

        for (int i = 0; i < limit; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Instantiate(_prefab, _handGroup.Container).Setup(_database.GetRandomIngredient());
            _handGroup.RefreshCardsArray();
        }
    }
}