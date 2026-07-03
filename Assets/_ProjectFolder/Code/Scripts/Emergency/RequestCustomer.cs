using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RequestCustomer : MonoBehaviour
{
    public Request[] ingredientsRequest;
    public GameObject recipeContainer;
    public DialogManager dialogManager;

    private RoundManager _roundManager;
    private Card[] _cards;

    private bool firstRequest = true;
    private List<int> firstIngredients = new List<int> { 1, 6, 7 };
    private int currentValueRequest;
    private int currentRequestIndex = -1;
    private bool match;

    public Request CurrentRequest => GetCurrentRequest();
    public bool LastRequestMatched { get; private set; } = true;

    public UnityEvent onMatch;
    public UnityEvent onMatchLose;

    private void OnEnable() => _roundManager.onRoundBegin += SetRequest;
    private void OnDisable() => _roundManager.onRoundBegin -= SetRequest;

    private void Awake()
    {
        _roundManager = RoundManager.Instance;
    }

    public void SetRequest()
    {
        if (firstRequest)
        {
            int randomIndex = Random.Range(0, firstIngredients.Count);
            currentValueRequest = randomIndex;
            currentRequestIndex = firstIngredients[currentValueRequest];
            dialogManager.PlayAtIndex(currentRequestIndex);
            Debug.Log(ingredientsRequest[currentRequestIndex].textRequest);
            return;
        }

        int valueRandom = Random.Range(0, ingredientsRequest.Length - 1);
        currentValueRequest = valueRandom;
        currentRequestIndex = currentValueRequest;
        dialogManager.PlayAtIndex(currentRequestIndex);
        Debug.Log(ingredientsRequest[currentRequestIndex].textRequest);
    }

    public void CheckRequest()
    {
        _cards = recipeContainer.GetComponentsInChildren<Card>();
        LastRequestMatched = false;

        foreach (var card in _cards)
        {
            if (card.Ingredient == CurrentRequest?.requestIngredient)
            {
                match = true;
            }
        }

        LastRequestMatched = match;

        if (match)
        {
            Debug.Log("Match");
            onMatch?.Invoke();
            dialogManager.Stop();
        }
        else
        {
            Debug.Log("No points");
            onMatchLose?.Invoke();
            dialogManager.Stop();
        }

        match = false;
        firstRequest = false;
    }

    public bool DoesPlateMatchCurrentRequest(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        Request request = CurrentRequest;
        if (request == null || request.requestIngredient == null)
        {
            return true;
        }

        return plateIngredients.TryGetValue(request.requestIngredient, out int amount) && amount > 0;
    }

    private Request GetCurrentRequest()
    {
        if (ingredientsRequest == null || ingredientsRequest.Length == 0)
        {
            return null;
        }

        if (currentRequestIndex < 0 || currentRequestIndex >= ingredientsRequest.Length)
        {
            return null;
        }

        return ingredientsRequest[currentRequestIndex];
    }
}

[System.Serializable]
public class Request
{
    //El testRequest es solo de testeo, ya se integro junto con el DialogManager
    public string textRequest;
    public SO_IngredientBase requestIngredient;
}
