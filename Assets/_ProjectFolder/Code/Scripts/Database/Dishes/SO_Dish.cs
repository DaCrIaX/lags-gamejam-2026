using UnityEngine;

[CreateAssetMenu(fileName = "Plate", menuName = "cards/dish/plate")]
public class SO_Dish : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _name;

    public Sprite Image => _sprite;
    public string Name => _name;
}