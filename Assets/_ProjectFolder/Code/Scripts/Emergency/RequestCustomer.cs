using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RequestCustomer : MonoBehaviour
{
    public Request[] ingredientsRequest;
    public GameObject recipeContainer;

    private RoundManager _roundManager;
    private Card[] _cards;

    private bool firstRequest = true;
    private List<int> firstIngredients = new List<int> { 1, 6, 7 };
    private int currentValueRequest;
    private bool match;

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
            Debug.Log(ingredientsRequest[firstIngredients[currentValueRequest]].textRequest);
            firstRequest = false;
            return;
        }

        int valueRandom = Random.Range(0, ingredientsRequest.Length - 1);
        currentValueRequest = valueRandom;
        Debug.Log(ingredientsRequest[currentValueRequest].textRequest);
    }

    public void CheckRequest()
    {
        _cards = recipeContainer.GetComponentsInChildren<Card>();
        if (_cards == null) return;
        foreach (var card in _cards)
        {
            if(card.Ingredient == ingredientsRequest[currentValueRequest].requestIngredient)
            {
                match = true;
            }
        }

        if (match)
        {
            Debug.Log("Match");
        }
        else
        {
            Debug.Log("No points");
        }

        match = false;
    }
}

[System.Serializable]
public class Request
{
    public string textRequest;
    public SO_IngredientBase requestIngredient;
}
