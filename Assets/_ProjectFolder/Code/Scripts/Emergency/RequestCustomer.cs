using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    private bool match;

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
            dialogManager.PlayAtIndex(firstIngredients[currentValueRequest]);
            Debug.Log(ingredientsRequest[firstIngredients[currentValueRequest]].textRequest);
            return;
        }

        int valueRandom = Random.Range(0, ingredientsRequest.Length - 1);
        currentValueRequest = valueRandom;
        dialogManager.PlayAtIndex(currentValueRequest);
        Debug.Log(ingredientsRequest[currentValueRequest].textRequest);
    }

    public void CheckRequest()
    {
        _cards = recipeContainer.GetComponentsInChildren<Card>();
        if (_cards.Length == 0) return;
        foreach (var card in _cards)
        {
            if (firstRequest)
            {
                if (card.Ingredient == ingredientsRequest[firstIngredients[currentValueRequest]].requestIngredient)
                {
                    match = true;
                }
            }
            else
            {
                if (card.Ingredient == ingredientsRequest[currentValueRequest].requestIngredient)
                {
                    match = true;
                }
            }
        }

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
}

[System.Serializable]
public class Request
{
    //El testRequest es solo de testeo, ya se integro junto con el DialogManager
    public string textRequest;
    public SO_IngredientBase requestIngredient;
}
