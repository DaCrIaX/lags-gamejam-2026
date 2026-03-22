using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckController : MonoBehaviour
{
    [SerializeField] private CardHolder _holder;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _spawnDelay;

    [SerializeField] private int _giveAmount;

    [ContextMenu("Add New Cards")]
    public void AddNewCards() => StartCoroutine(SpawnCards(_giveAmount));

    private IEnumerator SpawnCards(int amount)
    {
        _holder.ClearChildren();

        int limit = Mathf.Min(amount, _holder.MaxAmount);

        for (int i = 0; i < limit; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Instantiate(_prefab, _holder.Container);
            _holder.RefreshCardsArray();
        }
    }
}