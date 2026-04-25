using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private Stack<CardData> cards = new Stack<CardData>();

    public int Count => cards.Count;

    public Deck()
    {
        BuildStarterDeck();
    }

    private void BuildStarterDeck()
    {
        cards.Clear();

        List<CardData> generatedCards = new List<CardData>();

        AddElementSet(generatedCards, CardData.Element.Fire);
        AddElementSet(generatedCards, CardData.Element.Water);
        AddElementSet(generatedCards, CardData.Element.Ice);

        Shuffle(generatedCards);

        for (int i = 0; i < generatedCards.Count; i++)
        {
            cards.Push(generatedCards[i]);
        }
    }

    private void AddElementSet(List<CardData> list, CardData.Element element)
    {
        // 5 cards per element = 15-card deck total
        list.Add(new CardData(Guid.NewGuid().ToString(), element, 2));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, 4));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, 6));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, 8));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, 10));
    }

    public CardData PullCard()
    {
        if (cards.Count == 0)
            return null;

        return cards.Pop();
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);

            CardData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}