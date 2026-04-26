using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ClientType
{
    Regular,          // Cliente normal, sin comportamiento especial
    Suspicious,       // Cliente sospechoso - penaliza platos comunes
    VIP,              // Cliente VIP - bonus por match perfecto
    Picky             // Cliente exigente - penaliza intentos fallidos más
}

[CreateAssetMenu(fileName = "ClientProfile", menuName = "Game/ClientProfile")]
public class SO_ClientProfile : ScriptableObject
{
    [SerializeField] private string _clientName;
    [SerializeField] private ClientType _type;
    [SerializeField] private Sprite[] _skins;
    
    [Header("Configuración de Comportamiento")]
    [SerializeField] private float _appearanceChance = 0.15f; // 15% de probabilidad
    [SerializeField] private bool _isSpecial = false;
    
    [Header("Modificadores de Sospecha")]
    [SerializeField] private int _perfectMatchSuspicionMod = 0;
    [SerializeField] private int _commonDishSuspicionMod = 0;
    [SerializeField] private int _invalidDishSuspicionMod = 0;
    
    [Header("Multiplicadores de Puntos")]
    [SerializeField] private float _scoreMultiplier = 1f;
    
    [TextArea(3, 5)]
    [SerializeField] private string _description;

    public string ClientName => _clientName;
    public ClientType Type => _type;
    public Sprite[] Skins => _skins;
    public float AppearanceChance => _appearanceChance;
    public bool IsSpecial => _isSpecial;
    
    public int PerfectMatchSuspicionMod => _perfectMatchSuspicionMod;
    public int CommonDishSuspicionMod => _commonDishSuspicionMod;
    public int InvalidDishSuspicionMod => _invalidDishSuspicionMod;
    
    public float ScoreMultiplier => _scoreMultiplier;
    public string Description => _description;

    public Sprite GetRandomSkin() => _skins[Random.Range(0, _skins.Length)];
}
