using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLobbyUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameResultsPanel;

    [Header("Lobby")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text lobbyStatusText;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private FusionBootstrap fusionBootstrap;

    private NetworkGameManager gameManager;
    private bool clickedReady, sentName;


    private void Start()
    {
        connectionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);

        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(OnStartGameClicked);

        if (fusionBootstrap == null)
            fusionBootstrap = FindFirstObjectByType<FusionBootstrap>();

        fusionBootstrap.StartLobbyBrowser();
    }


    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
        {
            connectionPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            gamePanel.SetActive(false);
            return;
        }

        if (!sentName)
        {
            string chosenName = playerNameInput != null ? playerNameInput.text : "Player";
            gameManager.RPC_SetPlayerName(gameManager.Runner.LocalPlayer, chosenName);
            sentName = true;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (gameManager.Phase == MatchPhase.WaitingForPlayers)
        {
            clickedReady = false;

            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            gamePanel.SetActive(false);

            if (gameResultsPanel != null)
                gameResultsPanel.SetActive(false);

            startGameButton.interactable = false;
            SetStatus("Waiting for another player...");
        }
        else if (gameManager.Phase == MatchPhase.WaitingForReady)
        {
            clickedReady = false;

            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            gamePanel.SetActive(false);

            if (gameResultsPanel != null)
                gameResultsPanel.SetActive(false);

            startGameButton.interactable = true;
            SetStatus("Ready to start.");
        }
        else if (gameManager.Phase == MatchPhase.WaitingForSelections ||
                 gameManager.Phase == MatchPhase.RoundFinished)
        {
            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
    }

    private void OnStartGameClicked()
    {
        if (gameManager == null)
            return;

        if (gameManager.Phase != MatchPhase.WaitingForReady)
            return;

        clickedReady = true;
        startGameButton.interactable = false;
        SetStatus("Waiting for opponent...");

        gameManager.RPC_SetReady(gameManager.Runner.LocalPlayer);
    }

    private void SetStatus(string message)
    {
        if (lobbyStatusText != null)
            lobbyStatusText.text = message;
    }
}