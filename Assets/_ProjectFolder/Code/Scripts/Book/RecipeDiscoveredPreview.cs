using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class RecipeDiscoveredPreview : MonoBehaviour
{
    [SerializeField] private TweenCanvasGroup _animation;
    [SerializeField] private Image _imagePreview;

    private void OnEnable() => RoundManager.Instance.onRecipeDiscovered += PerformeNewRecipe;
    private void OnDisable() => RoundManager.Instance.onRecipeDiscovered -= PerformeNewRecipe;

    private void PerformeNewRecipe(SO_Recipe recipe)
    {
        _animation.FadeIn();
        _imagePreview.sprite = recipe.Image;
        StartCoroutine(PreviewDelay());
    }
    private IEnumerator PreviewDelay()
    {
        yield return new WaitForSeconds(GameplayManager.Instance.PreviewNewRecipeDuration);
        _animation.FadeOut();
    }
}