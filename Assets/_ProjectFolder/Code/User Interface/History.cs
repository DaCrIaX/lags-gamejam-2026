using UnityEngine;
using TMPro;

public class History : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, TextArea(1, 10)] private string[] _pages;

    private int _index = 0;

    private void Start() => UpdateUI();
    private void UpdateUI() => _text.SetText(_pages[_index]);

    public void Next()
    {
        if (_index >= _pages.Length - 1) return;
        _index++;
        UpdateUI();
    }
    public void Previus()
    {
        if (_index <= 0) return;
        _index--;
        UpdateUI();
    }
}