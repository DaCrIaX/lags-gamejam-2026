using UnityEngine;
using UnityEngine.Splines;

public class NPC : MonoBehaviour
{
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private float _npcSpeed = 1f;

    [SerializeField] private Material _material;

    private Transform _transform;
    private SO_ClientProfile _currentProfile;

    [Header("Indicadores Visuales")]
    [SerializeField] private GameObject _specialClientIndicator;

    public float Speed => _npcSpeed;
    public SO_ClientProfile CurrentProfile => _currentProfile;

    private void Awake()
    {
        _transform = transform;
        SetOnSpline(0f);
    }

    /// <summary>
    /// Configura el NPC con el perfil del cliente
    /// </summary>
    public void SetClientProfile(SO_ClientProfile profile)
    {
        _currentProfile = profile;

        // Cambiar skin
        Sprite skinSprite = profile.GetRandomSkin();
        if (_material != null)
            _material.mainTexture = skinSprite.texture;

        // Indicador visual si es especial
        if (_specialClientIndicator != null)
            _specialClientIndicator.SetActive(profile.IsSpecial);

        Debug.Log($"[NPC] Cliente asignado: {profile.ClientName} ({profile.Type})");
    }

    /// <summary>
    /// Anima al NPC a lo largo de la spline
    /// </summary>
    public void SetOnSpline(float time) => _transform.position = _spline.EvaluatePosition(time);

    /// <summary>
    /// Obtiene si el cliente actual es especial
    /// </summary>
    public bool IsSpecialClient() => _currentProfile != null && _currentProfile.IsSpecial;
}
