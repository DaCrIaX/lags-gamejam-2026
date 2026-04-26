using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeveloperValue : DeveloperBehaviour
{
    [SerializeField] private int _defaultValue, _newValue;
    [SerializeField] private UnityEvent<int> _onValueChagend;

    protected override void OnPerforme(InputAction.CallbackContext ctx)
    {
        _isActive = !_isActive;
        _onValueChagend.Invoke(_isActive ? _newValue : _defaultValue);
    }
}