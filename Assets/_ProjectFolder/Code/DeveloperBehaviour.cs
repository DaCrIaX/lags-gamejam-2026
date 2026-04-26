using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DeveloperBehaviour : MonoBehaviour
{
    [SerializeField] protected InputActionReference _action;
    protected bool _isActive;

    protected virtual void Start() => _action.action.performed += OnPerforme;
    protected virtual void OnDestroy() => _action.action.performed -= OnPerforme;

    protected abstract void OnPerforme(InputAction.CallbackContext ctx);
}