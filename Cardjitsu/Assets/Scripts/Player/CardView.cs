using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image cardBack;
    [SerializeField] private Image art;
    [SerializeField] private Image elementIcon;
    [SerializeField] private TMP_Text valueText;

    [Header("Shared Sprites")]
    [SerializeField] private Sprite sharedArt;
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite iceIcon;

    [Header("Element Colors")]
    [SerializeField] private Color fireColor = Color.red;
    [SerializeField] private Color waterColor = Color.blue;
    [SerializeField] private Color iceColor = Color.cyan;

    [Header("Special Card Art")]
    [SerializeField] private Sprite fire10Art;

    private Button button;
    private CardData currentCard;

    public CardData CurrentCard => currentCard;

    public event Action<CardView> Clicked;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveListener(HandleClicked);
            button.onClick.AddListener(HandleClicked);
        }
    }
    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? Vector3.one * 1.12f : Vector3.one;
    }
    public void SetCard(CardData card)
    {
        currentCard = card;

        if (art != null)
            art.sprite = GetArt(card);

        if (elementIcon != null)
            elementIcon.sprite = GetIcon(card.element);

        if (valueText != null)
            valueText.text = card.value.ToString();

        if (cardBack != null)
            cardBack.color = GetColor(card.element);
    }
    private Sprite GetArt(CardData card)
    {
        if (card.element == CardData.Element.Fire && card.value == 10 && fire10Art != null)
            return fire10Art;

        return sharedArt;
    }
    public void SetInteractable(bool interactable)
    {
        if (button != null)
            button.enabled = interactable;
    }
    private void HandleClicked()
    {
        Debug.Log("CARDVIEW HANDLE CLICKED");

        if (currentCard == null)
        {
            Debug.LogWarning("Current card is null.");
            return;
        }

        Clicked?.Invoke(this);
    }

    private Sprite GetIcon(CardData.Element element)
    {
        switch (element)
        {
            case CardData.Element.Fire:
                return fireIcon;
            case CardData.Element.Water:
                return waterIcon;
            case CardData.Element.Ice:
                return iceIcon;
            default:
                return null;
        }
    }

    private Color GetColor(CardData.Element element)
    {
        switch (element)
        {
            case CardData.Element.Fire:
                return fireColor;
            case CardData.Element.Water:
                return waterColor;
            case CardData.Element.Ice:
                return iceColor;
            default:
                return Color.white;
        }
    }
    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClicked);
    }
}