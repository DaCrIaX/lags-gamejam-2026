using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HOVCardsGroupHandler : MonoBehaviour
{
    protected GameplayManager _manager;
    protected InventoryManager _inventory;
    protected RoundManager _roundManager;
    protected HOVCardsGroup _group;

    protected virtual void Awake()
    {
        _manager = GameplayManager.Instance;
        _roundManager = RoundManager.Instance;
        _inventory = InventoryManager.Instance;
        _group = GetComponentInChildren<HOVCardsGroup>();
    }
}