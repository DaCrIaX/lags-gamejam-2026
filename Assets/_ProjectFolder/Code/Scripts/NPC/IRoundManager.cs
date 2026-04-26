using UnityEngine;

/// <summary>
/// Interface que define el contrato para RoundManager
/// Implementa esto en tu RoundManager para que funcione con RecipeCheckerHandler
/// </summary>
public interface IRoundManager
{
    /// <summary>
    /// Se llama cuando el jugador envía ingredientes
    /// </summary>
    void SendedIngredients(int score);

    /// <summary>
    /// Se llama cuando se descubre una nueva receta
    /// </summary>
    void DiscoverRecipe(SO_Recipe recipe);

    /// <summary>
    /// Se llama para actualizar el valor de sospecha
    /// </summary>
    void UpdateSuspicion(int suspicionChange);

    /// <summary>
    /// Se llama para completar el round actual
    /// </summary>
    void CompleteRound();
}
