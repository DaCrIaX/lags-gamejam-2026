using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HOVCardsGroupHandler : MonoBehaviour
{
    protected InventoryManager _inventory;
    protected RoundManager _roundManager;
    protected HOVCardsGroup _group;

    protected virtual void Awake()
    {
        _roundManager = RoundManager.Instance;
        _inventory = InventoryManager.Instance;
        _group = GetComponentInChildren<HOVCardsGroup>();
    }
}