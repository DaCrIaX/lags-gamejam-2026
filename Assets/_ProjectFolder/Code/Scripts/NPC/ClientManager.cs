using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private List<SO_ClientProfile> _allClientProfiles = new();
    [SerializeField] private SO_ClientProfile _defaultClientProfile;
    
    [Header("Configuración")]
    [SerializeField] private int _roundsBetweenSpecialClients = 5;
    [SerializeField] private int _maxConsecutiveRegularClients = 3;
    
    private SO_ClientProfile _currentClient;
    private int _roundsSinceLastSpecial = 0;
    private int _consecutiveRegular = 0;

    private static ClientManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    /// <summary>
    /// Selecciona un cliente para el siguiente round
    /// Incluye lógica de probabilidad y rotación
    /// </summary>
    public SO_ClientProfile SelectNextClient()
    {
        // Si han pasado suficientes rounds, fuerza un cliente especial
        if (_roundsSinceLastSpecial >= _roundsBetweenSpecialClients)
        {
            _currentClient = SelectSpecialClient();
            _roundsSinceLastSpecial = 0;
            _consecutiveRegular = 0;
            return _currentClient;
        }

        // Si hay demasiados regulares seguidos, fuerza un especial
        if (_consecutiveRegular >= _maxConsecutiveRegularClients)
        {
            _currentClient = SelectSpecialClient();
            _roundsSinceLastSpecial = 0;
            _consecutiveRegular = 0;
            return _currentClient;
        }

        // Probabilidad normal: intenta un cliente especial
        var specialClients = _allClientProfiles.Where(c => c.IsSpecial).ToList();
        if (specialClients.Count > 0 && Random.value < specialClients[0].AppearanceChance)
        {
            _currentClient = SelectSpecialClient();
            _roundsSinceLastSpecial = 0;
            _consecutiveRegular = 0;
            return _currentClient;
        }

        // Default: cliente regular
        _currentClient = _defaultClientProfile;
        _roundsSinceLastSpecial++;
        _consecutiveRegular++;

        return _currentClient;
    }

    /// <summary>
    /// Selecciona un cliente especial aleatorio de los disponibles
    /// </summary>
    private SO_ClientProfile SelectSpecialClient()
    {
        var specialClients = _allClientProfiles.Where(c => c.IsSpecial).ToList();
        
        if (specialClients.Count == 0)
        {
            Debug.LogWarning("[ClientManager] No hay clientes especiales disponibles");
            return _defaultClientProfile;
        }

        return specialClients[Random.Range(0, specialClients.Count)];
    }

    /// <summary>
    /// Obtiene el cliente actual
    /// </summary>
    public SO_ClientProfile GetCurrentClient() => _currentClient;

    /// <summary>
    /// Obtiene información sobre cuándo aparecerá el próximo cliente especial
    /// </summary>
    public int GetRoundsUntilSpecialClient()
    {
        int remaining = _roundsBetweenSpecialClients - _roundsSinceLastSpecial;
        return Mathf.Max(remaining, 0);
    }

    /// <summary>
    /// Obtiene el cliente por tipo
    /// </summary>
    public SO_ClientProfile GetClientByType(ClientType type)
    {
        return _allClientProfiles.FirstOrDefault(c => c.Type == type) ?? _defaultClientProfile;
    }

    /// <summary>
    /// Reset para una nueva sesión
    /// </summary>
    public void ResetClientRotation()
    {
        _roundsSinceLastSpecial = 0;
        _consecutiveRegular = 0;
        _currentClient = _defaultClientProfile;
    }
}
