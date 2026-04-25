using NUnit.Framework;
using UnityEngine;

using System.Collections.Generic;

public class OldPlayer : MonoBehaviour
{
    private List<Card> mHand = new List<Card>();
    private List<Card> mWonCards = new List<Card>();


    private bool ElementWin(Card.Element one, Card.Element two, Card.Element three)
    {
        if ((one == three && one == two) || 
            (one != two && one != three && two != three)) return true;

        return false;
    }

    private bool ColorWin(Card.Color one, Card.Color two, Card.Color three)
    {
        if (one != two && one != three && two != three) return true;
        return false;
    }

    public bool HasPlayerWon()
    {
        // two ways to win
        // 1. three cards of the same element with different colors
        // 2. one card of each element with different colors

        if (mWonCards.Count >= 3) // only check win conditions if we have 3 or more cards
        {                                                                   
            for (int i = 0; i < mWonCards.Count - 2; i++) // this loop will end at the 3rd to last card 
            {                                                               
                Card.Element elementOne = mWonCards[i].element;             
                                                                            
                for (int j = i + 1; j < mWonCards.Count - 1; j++) // this loop will end at the 2nd to last card
                {                                                           
                    Card.Element elementTwo = mWonCards[j].element;

                    for (int k = j + 1; k < mWonCards.Count; k++) // this loop will end at the last card in the hand
                    {                                                       
                        Card.Element elementThree = mWonCards[k].element;    

                        // we do these three loops since we need to check every possible non repeating triplet of cards
                        // we gather elements first and check conditions
                        if (ElementWin(elementOne, elementTwo, elementThree))
                        {
                            // if elements are good then check if all colors are different
                            Card.Color colorOne = mWonCards[i].color;
                            Card.Color colorTwo = mWonCards[j].color;
                            Card.Color colorThree = mWonCards[k].color;

                            if (ColorWin(colorOne, colorTwo, colorThree)) return true;
                        }
                    }
                }
            }
        }

        return false;
    }

}
