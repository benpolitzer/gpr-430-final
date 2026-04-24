using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    private const int CARD_SEED_AMOUNT = 50;
    private Stack<GameObject> cards;

    [SerializeField] private GameObject cardPrefab;

    // should be used when starting a match
    private void SeedDeck()
    {
        cards.Clear();
        for (int i = 0; i < CARD_SEED_AMOUNT; i++)
        {
            GameObject card = Instantiate(cardPrefab);

            card.GetComponent<Card>().Randomize();

            cards.Push(card);
        }
    }

    public GameObject PullCard()
    {
        if (cards.Count == 0) SeedDeck();
        return cards.Pop();
    }

    private void Start()
    {
        
    }
}
