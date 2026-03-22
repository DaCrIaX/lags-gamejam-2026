using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckController : MonoBehaviour
{
    [SerializeReference] private SO_Database _database;
    [SerializeField] private CardGroup _holder;
    [SerializeField] private Card _prefab;

    [SerializeField] private float _spawnDelay;
    [SerializeField] private int _giveAmount;

    private void Start() => AddNewCards();

    [ContextMenu("Add New Cards")]
    public void AddNewCards() => StartCoroutine(SpawnCards(_giveAmount));

    private IEnumerator SpawnCards(int amount)
    {
        _holder.ClearChildren();

        int limit = Mathf.Min(amount, _holder.MaxAmount);

        for (int i = 0; i < limit; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Instantiate(_prefab, _holder.Container).Setup(_database.GetRandomIngredient());
            _holder.RefreshCardsArray();
        }
    }
}