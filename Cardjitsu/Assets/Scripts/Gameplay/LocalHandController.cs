using UnityEngine;

public class LocalHandController : MonoBehaviour
{
    [SerializeField] private HandView handView;

    private NetworkGameManager gameManager;
    private int lastDealtRound = -1;
    private bool handCleared = true;

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
            return;
        if (gameManager.Phase == MatchPhase.WaitingForReady || gameManager.Phase == MatchPhase.WaitingForPlayers)
        {
            lastDealtRound = -1;

            if (!handCleared)
            {
                handView.ClearHand();
                handCleared = true;
            }

            return;
        }
        if (gameManager.Phase == MatchPhase.WaitingForSelections && gameManager.RoundNumber != lastDealtRound)
        {
            DealNewHand();
            lastDealtRound = gameManager.RoundNumber;
        }
    }

    private void DealNewHand()
    {
        PlayerCardState playerState = new PlayerCardState();
        playerState.DrawToHandSize(5);

        handView.DisplayHand(playerState.Hand);
        handView.ClearSelection();
        handView.SetCardsInteractable(true);

        handCleared = false;

        Debug.LogWarning($"Dealt new hand for round {gameManager.RoundNumber}");
    }

    public void ResetDealState()
    {
        lastDealtRound = -1;
        handView.ClearHand();
    }
}