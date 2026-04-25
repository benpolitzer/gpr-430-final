using System.Collections.Generic;

public class PlayerCardState
{
    public Deck Deck { get; private set; }
    public List<CardData> Hand { get; private set; } = new();
    public List<CardData> WonCards { get; private set; } = new();

    public PlayerCardState()
    {
        Deck = new Deck();
    }

    public void DrawToHandSize(int targetSize)
    {
        while (Hand.Count < targetSize && Deck.Count > 0)
        {
            CardData drawnCard = Deck.PullCard();

            if (drawnCard != null)
                Hand.Add(drawnCard);
        }
    }

    public bool RemoveFromHand(CardData card)
    {
        return Hand.Remove(card);
    }

    public void AddWonCard(CardData card)
    {
        WonCards.Add(card);
    }
}