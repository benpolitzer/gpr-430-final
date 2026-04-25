public enum RoundOutcome
{
    PlayerOneWins,
    PlayerTwoWins,
    Tie
}
public static class CardComparer
{
    public static RoundOutcome Compare(CardData playerOneCard, CardData playerTwoCard)
    {
        if (playerOneCard.element == playerTwoCard.element)
        {
            if (playerOneCard.value > playerTwoCard.value)
                return RoundOutcome.PlayerOneWins;

            if (playerTwoCard.value > playerOneCard.value)
                return RoundOutcome.PlayerTwoWins;

            return RoundOutcome.Tie;
        }

        if (DoesElementBeat(playerOneCard.element, playerTwoCard.element))
            return RoundOutcome.PlayerOneWins;

        return RoundOutcome.PlayerTwoWins;
    }

    private static bool DoesElementBeat(CardData.Element attacker, CardData.Element defender)
    {
        return
            attacker == CardData.Element.Fire && defender == CardData.Element.Ice ||
            attacker == CardData.Element.Ice && defender == CardData.Element.Water ||
            attacker == CardData.Element.Water && defender == CardData.Element.Fire;
    }
}