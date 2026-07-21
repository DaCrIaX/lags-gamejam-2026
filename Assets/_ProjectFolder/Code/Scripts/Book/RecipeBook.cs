using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    [SerializeField] private bool _pauseClientTimerWhenOpen = true;
    [SerializeField] private TextMeshProUGUI _name;

    [SerializeField] private Image _dish;
    [SerializeField] private Image[] _ingredients;

    private List<SO_Recipe> _discovered = new();
    private int _index = 0;
    private bool _isTimerPausedByBook;

    private void Start() => _button.SetActive(false);
    private void OnEnable() => RoundManager.Instance.onRecipeDiscovered += OnUpdateList;
    private void OnDisable()
    {
        RoundManager.Instance.onRecipeDiscovered -= OnUpdateList;
        ResumeTimer();
    }

    private void OnUpdateList(SO_Recipe recipe)
    {
        if (_discovered.Contains(recipe)) return;
        _discovered.Add(recipe);
        _button.SetActive(true);
    }
    private void UpdateIndex()
    {
        if (_discovered.Count == 0) return;

        _name.SetText(_discovered[_index].Name);
        _dish.sprite = _discovered[_index].Image.LoadAsset();

        for (int i = 0; i < _ingredients.Length; i++)
            _ingredients[i].sprite = _discovered[_index].Ingredients[i].ingredient.Image.LoadAsset();
    }

    public void OpenBook()
    {
        PauseTimer();
        UpdateIndex();
    }

    public void CloseBook() => ResumeTimer();

    public void PauseTimer()
    {
        if (!_pauseClientTimerWhenOpen || _isTimerPausedByBook) return;

        RoundManager.Instance.PauseClientTimer();
        _isTimerPausedByBook = true;
    }

    public void ResumeTimer()
    {
        if (!_isTimerPausedByBook) return;

        RoundManager.Instance.ResumeClientTimer();
        _isTimerPausedByBook = false;
    }

    public void Next()
    {
        if (_discovered.Count == 0) return;

        _index++;
        if (_index >= _discovered.Count) _index = 0;
        UpdateIndex();
    }
    public void Previous()
    {
        if (_discovered.Count == 0) return;

        _index--;
        if (_index < 0) _index = _discovered.Count - 1;
        UpdateIndex();
    }
}
