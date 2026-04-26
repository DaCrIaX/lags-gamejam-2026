using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeveloperValue : MonoBehaviour
{
    [SerializeField] private int _defaultValue, _newValue;
    [SerializeField] private UnityEvent<int> _onValueChagend;

    private void Awake() => DeveloperBehaviour.onValueChanged += OnPerforme;
    private void OnDestroy() => DeveloperBehaviour.onValueChanged -= OnPerforme;
    private void Start() => OnPerforme(DeveloperBehaviour.isActive);
    
    private void OnPerforme(bool isActive) => _onValueChagend.Invoke(isActive ? _newValue : _defaultValue);
}