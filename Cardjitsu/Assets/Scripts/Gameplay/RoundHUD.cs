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

    [Header("Round Tallies")]
    [SerializeField] private Image[] playerATallies;
    [SerializeField] private Image[] playerBTallies;
    [SerializeField] private Sprite emptyTallySprite;
    [SerializeField] private Sprite filledTallySprite;

    [Header("Player Name Labels")]
    [SerializeField] private TMP_Text playerANameText;
    [SerializeField] private TMP_Text playerBNameText;

    private NetworkGameManager gameManager;
    private MatchPhase lastPhase = (MatchPhase)(-999);
    private int lastRoundNumber = -1;
    private int lastPlayerAWins = -1;
    private int lastPlayerBWins = -1;

    private void Start()
    {
        roundResultPanel.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        nextRoundButton.onClick.AddListener(OnNextRoundClicked);

        gameResultsPanel.SetActive(false);
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        playerReadyText.SetActive(false);
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
            gameManager.PlayerARoundWins == lastPlayerAWins &&
            gameManager.PlayerBRoundWins == lastPlayerBWins)
        {
            return;
        }

        lastPhase = gameManager.Phase;
        lastRoundNumber = gameManager.RoundNumber;
        lastPlayerAWins = gameManager.PlayerARoundWins;
        lastPlayerBWins = gameManager.PlayerBRoundWins;

        RefreshUI();
    }
    private void UpdatePlayerNameLabels()
    {
        if (gameManager == null)
            return;

        string playerAName = string.IsNullOrWhiteSpace(gameManager.PlayerAName.ToString())
            ? "Player A"
            : gameManager.PlayerAName.ToString();

        string playerBName = string.IsNullOrWhiteSpace(gameManager.PlayerBName.ToString())
            ? "Player B"
            : gameManager.PlayerBName.ToString();

        if (playerANameText != null)
            playerANameText.text = playerAName;

        if (playerBNameText != null)
            playerBNameText.text = playerBName;
    }
    private void UpdateTallies()
    {
        for (int i = 0; i < playerATallies.Length; i++)
        {
            playerATallies[i].sprite = i < gameManager.PlayerARoundWins
                ? filledTallySprite
                : emptyTallySprite;
        }

        for (int i = 0; i < playerBTallies.Length; i++)
        {
            playerBTallies[i].sprite = i < gameManager.PlayerBRoundWins
                ? filledTallySprite
                : emptyTallySprite;
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
        {
            playerReadyText.SetActive(gameManager.PlayerBSubmitted && !gameManager.PlayerASubmitted);
        }
        else if (localIsB)
        {
            playerReadyText.SetActive(gameManager.PlayerASubmitted && !gameManager.PlayerBSubmitted);
        }
        else
        {
            playerReadyText.SetActive(false);
        }
    }
    private void RefreshUI()
    {
        UpdatePlayerNameLabels();
        UpdateTallies();

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
        else if (gameManager.Phase == MatchPhase.WaitingForReady || gameManager.Phase == MatchPhase.WaitingForPlayers)
        {
            handPanel.SetActive(false);
            roundResultPanel.SetActive(false);
            nextRoundButton.gameObject.SetActive(false);
            gameResultsPanel.SetActive(false);
        }
    }
    private void ShowMatchFinished()
    {
        handPanel.SetActive(false);
        roundResultPanel.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        gameResultsPanel.SetActive(true);

        string playerAName = gameManager.PlayerAName.ToString();
        string playerBName = gameManager.PlayerBName.ToString();

        if (gameManager.PlayerARoundWins >= 3)
            winnerText.text = $"{playerAName} WINS!";
        else
            winnerText.text = $"{playerBName} WINS!";
    }
    public void OnQuitClicked()
    {
        NetworkGameManager manager = FindFirstObjectByType<NetworkGameManager>();

        if (manager != null && manager.Runner != null)
            manager.Runner.Shutdown();

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    private void OnPlayAgainClicked()
    {
        if (gameManager != null)
            gameManager.RPC_PlayAgain(gameManager.Runner.LocalPlayer);
    }

    private void ShowRoundResult()
    {
        handPanel.SetActive(false);
        roundResultPanel.SetActive(true);
        nextRoundButton.gameObject.SetActive(true);
        string playerAName = gameManager.PlayerAName.ToString();
        string playerBName = gameManager.PlayerBName.ToString();

        if (gameManager.WinnerSlot == 0)
            gameLabel.text = $"{playerAName} wins!";
        else if (gameManager.WinnerSlot == 1)
            gameLabel.text = $"{playerBName} wins!";
        else
            gameLabel.text = "Tie!";

        CardData cardA = new CardData(
            "A",
            (CardData.Element)gameManager.PlayerAElement,
            gameManager.PlayerAValue
        );

        CardData cardB = new CardData(
            "B",
            (CardData.Element)gameManager.PlayerBElement,
            gameManager.PlayerBValue
        );

        playerACardView.SetCard(cardA);
        playerBCardView.SetCard(cardB);

        playerACardView.transform.localScale = gameManager.WinnerSlot == 0
            ? Vector3.one * 1.25f
            : Vector3.one;

        playerBCardView.transform.localScale = gameManager.WinnerSlot == 1
            ? Vector3.one * 1.25f
            : Vector3.one;
    }

    public void OnNextRoundClicked()
    {
        if (gameManager != null)
            gameManager.RPC_StartNextRound();
    }
}