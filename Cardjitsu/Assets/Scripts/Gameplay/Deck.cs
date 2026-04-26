using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private Stack<CardData> cards = new Stack<CardData>();

    public int Count => cards.Count;

    public Deck()
    {
        BuildDeck();
    }

    private void BuildDeck()
    {
        cards.Clear();

        List<CardData> generated = new();

        AddElementSet(generated, CardData.Element.Fire);
        AddElementSet(generated, CardData.Element.Water);
        AddElementSet(generated, CardData.Element.Ice);

        Shuffle(generated);

        foreach (CardData card in generated)
            cards.Push(card);
    }

    private void AddElementSet(List<CardData> list, CardData.Element element)
    {
        list.Add(new CardData(Guid.NewGuid().ToString(), element, CardData.CardColor.Red, 2));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, CardData.CardColor.Blue, 4));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, CardData.CardColor.Yellow, 6));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, CardData.CardColor.Green, 8));
        list.Add(new CardData(Guid.NewGuid().ToString(), element, CardData.CardColor.Purple, 10));
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
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}