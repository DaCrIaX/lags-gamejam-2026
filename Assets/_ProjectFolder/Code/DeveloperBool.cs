using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeveloperBool : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> _onValueChagend;

    private void Awake() => DeveloperBehaviour.onValueChanged += OnPerforme;
    private void OnDestroy() => DeveloperBehaviour.onValueChanged -= OnPerforme;
    private void Start() => OnPerforme(DeveloperBehaviour.isActive);

    private void OnPerforme(bool isActive) => _onValueChagend.Invoke(isActive);
}