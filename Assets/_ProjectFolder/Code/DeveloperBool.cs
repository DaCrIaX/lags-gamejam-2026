using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeveloperBool : DeveloperBehaviour
{
    [SerializeField] private UnityEvent<bool> _onValueChagend;

    protected override void OnPerforme(InputAction.CallbackContext ctx)
    {
        _isActive = !_isActive;
        _onValueChagend.Invoke(_isActive);
    }
}