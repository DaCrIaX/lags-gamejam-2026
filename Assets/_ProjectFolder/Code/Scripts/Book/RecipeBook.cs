using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    [SerializeField] private TextMeshProUGUI _name;

    [SerializeField] private Image _dish;
    [SerializeField] private Image[] _ingredients;

    private List<SO_Recipe> _discovered = new();
    private int _index = 0;

    private void Start() => _button.SetActive(false);
    private void OnEnable() => RoundManager.Instance.onRecipeDiscovered += OnUpdateList;
    private void OnDisable() => RoundManager.Instance.onRecipeDiscovered -= OnUpdateList;

    private void OnUpdateList(SO_Recipe recipe)
    {
        if (_discovered.Contains(recipe)) return;
        _discovered.Add(recipe);
        _button.SetActive(true);
    }
    private void UpdateIndex()
    {
        _name.SetText(_discovered[_index].Name);
        _dish.sprite = _discovered[_index].Image;

        for (int i = 0; i < _ingredients.Length; i++)
            _ingredients[i].sprite = _discovered[_index].Ingredients[i].ingredient.Image.LoadAsset();
    }

    public void OpenBook() => UpdateIndex();
    public void Next()
    {
        _index++;
        if (_index >= _discovered.Count) _index = 0;
        UpdateIndex();
    }
    public void Previous()
    {
        _index--;
        if (_index < 0) _index = _discovered.Count - 1;
        UpdateIndex();
    }
}