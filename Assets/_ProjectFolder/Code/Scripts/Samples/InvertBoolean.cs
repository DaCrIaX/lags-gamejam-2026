using UnityEngine;
using UnityEngine.Events;

public class InvertBoolean : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> _onValueChanged;

    public void OnValueChange(bool value) => _onValueChanged.Invoke(!value);
}