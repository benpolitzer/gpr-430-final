public enum RoundOutcome
{
    PlayerOneWins,
    PlayerTwoWins,
    Tie
}

public static class CardComparer
{
    public static RoundOutcome Compare(CardData a, CardData b)
    {
        if (a.element == b.element)
        {
            if (a.value > b.value)
                return RoundOutcome.PlayerOneWins;

            if (b.value > a.value)
                return RoundOutcome.PlayerTwoWins;

            return RoundOutcome.Tie;
        }

        if (ElementBeats(a.element, b.element))
            return RoundOutcome.PlayerOneWins;

        return RoundOutcome.PlayerTwoWins;
    }

    private static bool ElementBeats(CardData.Element attacker, CardData.Element defender)
    {
        return
            attacker == CardData.Element.Fire && defender == CardData.Element.Ice ||
            attacker == CardData.Element.Ice && defender == CardData.Element.Water ||
            attacker == CardData.Element.Water && defender == CardData.Element.Fire;
    }
}