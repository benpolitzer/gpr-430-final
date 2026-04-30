using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundHUD : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private TMP_Text gameLabel;
    [SerializeField] private GameObject handPanel;
    [SerializeField] private Button nextRoundButton;

    [Header("Round Result UI")]
    [SerializeField] private GameObject roundResultPanel;
    [SerializeField] private CardView playerACardView;
    [SerializeField] private CardView playerBCardView;
    [SerializeField] private GameObject playerReadyText;

    [Header("Selection")]
    [SerializeField] private NetworkCardSubmitter submitter;

    [Header("Game Results")]
    [SerializeField] private GameObject gameResultsPanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;

    [Header("Player Name Labels")]
    [SerializeField] private TMP_Text playerANameText;
    [SerializeField] private TMP_Text playerBNameText;

    [Header("Won Card UI")]
    [SerializeField] private CardView[] playerAWonCardViews;
    [SerializeField] private CardView[] playerBWonCardViews;

    private NetworkGameManager gameManager;
    private MatchPhase lastPhase = (MatchPhase)(-999);
    private int lastRoundNumber = -1;
    private int lastPlayerAWonCount = -1;
    private int lastPlayerBWonCount = -1;

    private void Start()
    {
        roundResultPanel.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        gameResultsPanel.SetActive(false);
        playerReadyText.SetActive(false);

        nextRoundButton.onClick.AddListener(OnNextRoundClicked);
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        playerACardView.SetInteractable(false);
        playerBCardView.SetInteractable(false);
        foreach (CardView card in playerAWonCardViews)
            card.gameObject.SetActive(false);

        foreach (CardView card in playerBWonCardViews)
            card.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
            return;

        UpdateReadyText();

        if (gameManager.Phase == lastPhase &&
            gameManager.RoundNumber == lastRoundNumber &&
            gameManager.PlayerAWonCount == lastPlayerAWonCount &&
            gameManager.PlayerBWonCount == lastPlayerBWonCount)
        {
            return;
        }

        lastPhase = gameManager.Phase;
        lastRoundNumber = gameManager.RoundNumber;
        lastPlayerAWonCount = gameManager.PlayerAWonCount;
        lastPlayerBWonCount = gameManager.PlayerBWonCount;

        RefreshUI();
    }

    private void RefreshUI()
    {
        UpdatePlayerNameLabels();
        UpdateWonCardDisplays();

        if (gameManager.Phase == MatchPhase.WaitingForSelections)
        {
            gameLabel.text = $"ROUND {gameManager.RoundNumber}";

            handPanel.SetActive(true);
            roundResultPanel.SetActive(false);
            nextRoundButton.gameObject.SetActive(false);
            gameResultsPanel.SetActive(false);

            submitter.ResetForNextRound();
        }
        else if (gameManager.Phase == MatchPhase.RoundFinished)
        {
            ShowRoundResult();
        }
        else if (gameManager.Phase == MatchPhase.MatchFinished)
        {
            ShowMatchFinished();
        }
        else if (gameManager.Phase == MatchPhase.WaitingForReady ||
                 gameManager.Phase == MatchPhase.WaitingForPlayers)
        {
            handPanel.SetActive(false);
            roundResultPanel.SetActive(false);
            nextRoundButton.gameObject.SetActive(false);
            gameResultsPanel.SetActive(false);
        }
    }

    private void ShowRoundResult()
    {
        handPanel.SetActive(false);
        roundResultPanel.SetActive(true);
        nextRoundButton.gameObject.SetActive(true);
        gameResultsPanel.SetActive(false);

        string playerAName = GetPlayerAName();
        string playerBName = GetPlayerBName();

        if (gameManager.WinnerSlot == 0)
            gameLabel.text = $"{playerAName} wins!";
        else if (gameManager.WinnerSlot == 1)
            gameLabel.text = $"{playerBName} wins!";
        else
            gameLabel.text = "Tie!";

        CardData cardA = new CardData(
            "A",
            (CardData.Element)gameManager.PlayerAElement,
            (CardData.CardColor)gameManager.PlayerAColor,
            gameManager.PlayerAValue
        );

        CardData cardB = new CardData(
            "B",
            (CardData.Element)gameManager.PlayerBElement,
            (CardData.CardColor)gameManager.PlayerBColor,
            gameManager.PlayerBValue
        );

        playerACardView.SetCard(cardA);
        playerBCardView.SetCard(cardB);

        playerACardView.SetInteractable(false);
        playerBCardView.SetInteractable(false);

        playerACardView.transform.localScale =
            gameManager.WinnerSlot == 0 ? Vector3.one * 1.25f : Vector3.one;

        playerBCardView.transform.localScale =
            gameManager.WinnerSlot == 1 ? Vector3.one * 1.25f : Vector3.one;
    }

    private void ShowMatchFinished()
    {
        handPanel.SetActive(false);
        roundResultPanel.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        gameResultsPanel.SetActive(true);

        if (gameManager.MatchWinner == 0)
            winnerText.text = $"{GetPlayerAName()} WINS!";
        else if (gameManager.MatchWinner == 1)
            winnerText.text = $"{GetPlayerBName()} WINS!";
        else
            winnerText.text = "MATCH ENDED";
    }

    private void UpdatePlayerNameLabels()
    {
        if (playerANameText != null)
            playerANameText.text = GetPlayerAName();

        if (playerBNameText != null)
            playerBNameText.text = GetPlayerBName();
    }

    private void UpdateWonCardDisplays()
    {
        for (int i = 0; i < playerAWonCardViews.Length; i++)
        {
            bool hasCard = i < gameManager.PlayerAWonCount;

            playerAWonCardViews[i].gameObject.SetActive(hasCard);
            playerAWonCardViews[i].transform.localScale = Vector3.one * 0.4f;

            if (hasCard)
            {
                CardData card = new CardData(
                    $"A_Won_{i}",
                    (CardData.Element)gameManager.PlayerAWonElements[i],
                    (CardData.CardColor)gameManager.PlayerAWonColors[i],
                    gameManager.PlayerAWonValues[i]
                );

                playerAWonCardViews[i].SetCard(card);
                playerAWonCardViews[i].SetInteractable(false);
            }
        }

        for (int i = 0; i < playerBWonCardViews.Length; i++)
        {
            bool hasCard = i < gameManager.PlayerBWonCount;

            playerBWonCardViews[i].gameObject.SetActive(hasCard);
            playerBWonCardViews[i].transform.localScale = Vector3.one * 0.4f;

            if (hasCard)
            {
                CardData card = new CardData(
                    $"B_Won_{i}",
                    (CardData.Element)gameManager.PlayerBWonElements[i],
                    (CardData.CardColor)gameManager.PlayerBWonColors[i],
                    gameManager.PlayerBWonValues[i]
                );

                playerBWonCardViews[i].SetCard(card);
                playerBWonCardViews[i].SetInteractable(false);
            }
        }
    }

    private void UpdateReadyText()
    {
        if (playerReadyText == null || gameManager == null)
            return;

        if (gameManager.Phase != MatchPhase.WaitingForSelections)
        {
            playerReadyText.SetActive(false);
            return;
        }

        bool localIsA = gameManager.Runner.LocalPlayer == gameManager.PlayerA;
        bool localIsB = gameManager.Runner.LocalPlayer == gameManager.PlayerB;

        if (localIsA)
            playerReadyText.SetActive(gameManager.PlayerBSubmitted && !gameManager.PlayerASubmitted);
        else if (localIsB)
            playerReadyText.SetActive(gameManager.PlayerASubmitted && !gameManager.PlayerBSubmitted);
        else
            playerReadyText.SetActive(false);
    }

    public string GetPlayerAName()
    {
        string name = gameManager.PlayerAName.ToString();
        return string.IsNullOrWhiteSpace(name) ? "Player A" : name;
    }

    public string GetPlayerBName()
    {
        string name = gameManager.PlayerBName.ToString();
        return string.IsNullOrWhiteSpace(name) ? "Player B" : name;
    }

    private void OnNextRoundClicked()
    {
        if (gameManager != null)
            gameManager.RPC_StartNextRound();
    }

    private void OnPlayAgainClicked()
    {
        if (gameManager != null)
            gameManager.RPC_PlayAgain(gameManager.Runner.LocalPlayer);
    }

    public void OnQuitClicked()
    {
        if (gameManager != null && gameManager.Runner != null)
            gameManager.Runner.Shutdown();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}