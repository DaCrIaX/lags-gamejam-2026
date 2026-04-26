using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeveloperBehaviour : MonoBehaviour
{
    [SerializeField] protected InputActionReference _action;

    public static bool isActive;
    public static Action<bool> onValueChanged;

    protected void Start() => _action.action.performed += OnPerforme;
    protected void OnDestroy() => _action.action.performed -= OnPerforme;

    protected void OnPerforme(InputAction.CallbackContext ctx)
    {
        isActive = !isActive;
        onValueChanged?.Invoke(isActive);
    }
}