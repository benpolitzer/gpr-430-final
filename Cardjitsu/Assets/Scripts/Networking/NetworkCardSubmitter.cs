using UnityEngine;

public class NetworkCardSubmitter : MonoBehaviour
{
    [SerializeField] private HandView handView;

    private NetworkGameManager gameManager;
    private bool submittedThisRound;

    private void Start()
    {
        handView.CardSelected += HandleCardSelected;
        handView.SetCardsInteractable(false);
    }

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
            return;

        if (gameManager.Phase == MatchPhase.WaitingForSelections && !submittedThisRound)
        {
            handView.SetCardsInteractable(true);
        }
    }

    private void HandleCardSelected(CardData card)
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
        {
            Debug.LogWarning("No NetworkGameManager found.");
            return;
        }

        if (submittedThisRound)
            return;

        if (gameManager.Phase != MatchPhase.WaitingForSelections)
            return;

        submittedThisRound = true;
        handView.SetCardsInteractable(false);

        gameManager.RPC_SubmitCard(gameManager.Runner.LocalPlayer, card.handIndex);

        Debug.LogWarning($"Submitted card to network: {card.element} {card.value}");
    }

    public void ResetForNextRound()
    {
        submittedThisRound = false;
        handView.ClearSelection();
        handView.SetCardsInteractable(true);
    }

    private void OnDestroy()
    {
        if (handView != null)
            handView.CardSelected -= HandleCardSelected;
    }
}