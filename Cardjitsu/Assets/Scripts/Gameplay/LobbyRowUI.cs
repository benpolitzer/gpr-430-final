using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionNameText;
    [SerializeField] private Image playerSlot1;
    [SerializeField] private Image playerSlot2;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text joinButtonText;

    [Header("Slot Sprites")]
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private Sprite filledSlotSprite;

    private string sessionName;
    private Action<string> onJoinClicked;

    public void Setup(string sessionName, int playerCount, int maxPlayers, Action<string> joinCallback)
    {
        this.sessionName = sessionName;
        onJoinClicked = joinCallback;

        if (sessionNameText != null)
            sessionNameText.text = sessionName;

        if (playerSlot1 != null)
            playerSlot1.sprite = playerCount >= 1 ? filledSlotSprite : emptySlotSprite;

        if (playerSlot2 != null)
            playerSlot2.sprite = playerCount >= 2 ? filledSlotSprite : emptySlotSprite;

        if (joinButton != null)
        {
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(JoinClicked);
            joinButton.interactable = playerCount < maxPlayers;
        }
        bool isFull = playerCount >= maxPlayers;

        joinButton.interactable = !isFull;

        if (joinButtonText != null)
            joinButtonText.text = isFull ? "FULL" : "JOIN";
    }

    private void JoinClicked()
    {
        onJoinClicked?.Invoke(sessionName);
    }
}