using UnityEngine;

[CreateAssetMenu(fileName = "ScoringConfig", menuName = "Game/ScoringConfig")]
public class SO_ScoringConfig : ScriptableObject
{
    [Header("Puntos Base")]
    [SerializeField] private int _perfectMatchBaseScore = 500;
    [SerializeField] private int _commonDishBaseScore = 100;
    [SerializeField] private int _minCardsForValidDish = 2;
    [SerializeField] private int _minTypeForCommon = 2;
    
    [Header("Multiplicadores por cantidad de cartas")]
    [SerializeField] private float _scoreMultiplierPerCard = 50f;
    
    [Header("Sistema de Sospecha")]
    [SerializeField] private int _perfectMatchSuspicion = 10;
    [SerializeField] private int _commonDishSuspicion = -15; // Negativo = reduce sospecha
    [SerializeField] private int _invalidDishSuspicion = 5;
    
    [Header("Penalizaciones")]
    [SerializeField] private float _randomIngredientsMultiplier = 0f; // 0 = sin puntos
    
    public int PerfectMatchBaseScore => _perfectMatchBaseScore;
    public int CommonDishBaseScore => _commonDishBaseScore;
    public int MinCardsForValidDish => _minCardsForValidDish;
    public int MinTypeForCommon => _minTypeForCommon;
    public float ScoreMultiplierPerCard => _scoreMultiplierPerCard;
    
    public int PerfectMatchSuspicion => _perfectMatchSuspicion;
    public int CommonDishSuspicion => _commonDishSuspicion;
    public int InvalidDishSuspicion => _invalidDishSuspicion;
    
    public float RandomIngredientsMultiplier => _randomIngredientsMultiplier;
}
