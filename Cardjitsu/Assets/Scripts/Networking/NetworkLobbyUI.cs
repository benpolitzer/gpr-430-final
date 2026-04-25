using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLobbyUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject gamePanel;

    [Header("Lobby")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text lobbyStatusText;
    [SerializeField] private TMP_InputField playerNameInput;

    private NetworkGameManager gameManager;
    private bool clickedReady, sentName;


    private void Start()
    {
        connectionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);

        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }


    private void Update()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<NetworkGameManager>();

        if (gameManager == null)
            return;

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
            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            gamePanel.SetActive(false);

            startGameButton.interactable = false;
            SetStatus("Waiting for another player...");
        }
        else if (gameManager.Phase == MatchPhase.WaitingForReady)
        {
            clickedReady = false;
            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            gamePanel.SetActive(false);

            startGameButton.interactable = !clickedReady;
            SetStatus(clickedReady ? "Waiting for opponent..." : "Ready to start.");
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

        if (gameManager.Phase == MatchPhase.WaitingForReady)
        {
            clickedReady = false;
            startGameButton.interactable = true;
            SetStatus("Ready to start.");
        }

        gameManager.RPC_SetReady(gameManager.Runner.LocalPlayer);
    }

    private void SetStatus(string message)
    {
        if (lobbyStatusText != null)
            lobbyStatusText.text = message;
    }
}