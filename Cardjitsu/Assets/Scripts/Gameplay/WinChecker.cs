using System.Collections.Generic;

public enum WinType
{
    None,
    SameElement,
    AllDifferentElements
}

public static class WinChecker
{
    public static WinType GetWinningSet(List<CardData> cards)
    {
        if (cards == null || cards.Count < 3)
            return WinType.None;

        for (int i = 0; i < cards.Count - 2; i++)
        {
            for (int j = i + 1; j < cards.Count - 1; j++)
            {
                for (int k = j + 1; k < cards.Count; k++)
                {
                    WinType result = CheckTriple(cards[i], cards[j], cards[k]);

                    if (result != WinType.None)
                        return result;
                }
            }
        }

        return WinType.None;
    }

    private static WinType CheckTriple(CardData a, CardData b, CardData c)
    {
        bool colorsDifferent =
            a.color != b.color &&
            a.color != c.color &&
            b.color != c.color;

        if (!colorsDifferent)
            return WinType.None;

        bool sameElement =
            a.element == b.element &&
            a.element == c.element;

        if (sameElement)
            return WinType.SameElement;

        bool allDifferentElements =
            a.element != b.element &&
            a.element != c.element &&
            b.element != c.element;

        if (allDifferentElements)
            return WinType.AllDifferentElements;

        return WinType.None;
    }

    public static bool HasWinningSet(List<CardData> cards)
    {
        return GetWinningSet(cards) != WinType.None;
    }
}