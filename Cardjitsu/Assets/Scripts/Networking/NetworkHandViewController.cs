using System.Collections.Generic;
using UnityEngine;

public class NetworkHandViewController : MonoBehaviour
{
    [SerializeField] private HandView handView;

    private NetworkGameManager gameManager;
    private int lastDisplayedRound = -1;

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
            return;

        if (gameManager.Phase == MatchPhase.WaitingForReady ||
            gameManager.Phase == MatchPhase.WaitingForPlayers ||
            gameManager.Phase == MatchPhase.MatchFinished)
        {
            lastDisplayedRound = -1;
            handView.ClearHand();
            return;
        }

        if (gameManager.Phase == MatchPhase.WaitingForSelections &&
            gameManager.RoundNumber != lastDisplayedRound)
        {
            DisplayLocalPlayerHand();
            lastDisplayedRound = gameManager.RoundNumber;
        }
    }

    private void DisplayLocalPlayerHand()
    {
        List<CardData> cards = new List<CardData>();

        bool localIsA = gameManager.Runner.LocalPlayer == gameManager.PlayerA;
        bool localIsB = gameManager.Runner.LocalPlayer == gameManager.PlayerB;

        if (localIsA)
        {
            for (int i = 0; i < gameManager.PlayerAHandCount; i++)
            {
                cards.Add(new CardData(
                    $"A_Hand_{i}",
                    (CardData.Element)gameManager.PlayerAHandElements[i],
                    (CardData.CardColor)gameManager.PlayerAHandColors[i],
                    gameManager.PlayerAHandValues[i],
                    i
                ));
            }
        }
        else if (localIsB)
        {
            for (int i = 0; i < gameManager.PlayerBHandCount; i++)
            {
                cards.Add(new CardData(
                    $"B_Hand_{i}",
                    (CardData.Element)gameManager.PlayerBHandElements[i],
                    (CardData.CardColor)gameManager.PlayerBHandColors[i],
                    gameManager.PlayerBHandValues[i],
                    i
                ));
            }
        }

        handView.DisplayHand(cards);
        handView.ClearSelection();
        handView.SetCardsInteractable(true);
    }
}