using UnityEngine;

public class OldCardComparer 
{

    // may need this later idk
    private static bool CheckEquals(Card cardOne, Card cardTwo)
    {
        if (cardOne.color == cardTwo.color && 
            cardOne.element == cardTwo.element && 
            cardOne.value == cardTwo.value) { return true; }
        return false;
    }

    /// <summary>
    /// Returns a winning card base on values.
    /// </summary>
    /// <param name="player1Card">The first card</param>
    /// <param name="player2Card">The second card</param>
    /// <returns></returns>
    private static Card CompareValues(Card player1Card, Card player2Card)
    {
        if (player1Card.value > player2Card.value)
        {
            return player1Card;
        }
        else if (player2Card.value > player1Card.value)
        {
            return player2Card;
        }

        return null; // if tie return null
    }

    /// <summary>
    /// Returns a winning card based on elements.
    /// </summary>
    /// <param name="player1Card">The first card</param>
    /// <param name="player2Card">The second card</param>
    /// <returns></returns>
    private static Card CompareElements(Card player1Card, Card player2Card)
    {
        if (player1Card.element == Card.Element.Fire)
        {
            if (player2Card.element == Card.Element.Fire)
            {
                return null;
            }
            else if (player2Card.element == Card.Element.Water)
            {
                return player2Card;
            }
            else //computer is ice
            {
                return player1Card;
            }
        }
        else if (player1Card.element == Card.Element.Water)
        {
            if (player2Card.element == Card.Element.Fire)
            {
                return player1Card;
            }
            else if (player2Card.element == Card.Element.Water)
            {
                return null;
            }
            else //computer is ice
            {
                return player2Card;
            }
        }
        else //player is ice
        {
            if (player2Card.element == Card.Element.Fire)
            {
                return player2Card;
            }
            else if (player2Card.element == Card.Element.Water)
            {
                return player1Card;
            }
            else //computer is ice
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player1Card"></param>
    /// <param name="player2Card"></param>
    /// <returns></returns>
    public static Card CompareCards(Card player1Card, Card player2Card)
    {
        Card winningCard = CompareElements(player1Card, player2Card); // checks the elements of both cards, returns the winner
        if (winningCard) { return winningCard; } // if element does not tie, will return winning matchup

        // if no equal in elements check values
        winningCard = CompareValues(player1Card, player2Card);
        return winningCard; //returns winning card (if cards are the same, returns null)
    }
}
