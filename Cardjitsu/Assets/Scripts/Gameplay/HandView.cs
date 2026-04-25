using System;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardView cardPrefab;
    private CardView selectedCard;

    private readonly List<CardView> spawnedCards = new();

    public event Action<CardData> CardSelected;

    public void DisplayHand(List<CardData> hand)
    {
        ClearHand();

        foreach (CardData card in hand)
        {
            CardView cardView = Instantiate(cardPrefab, cardContainer);
            cardView.SetCard(card);
            cardView.Clicked += HandleCardClicked;

            spawnedCards.Add(cardView);
        }
    }

    public void ClearSelection()
    {
        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = null;
    }
    public void SetCardsInteractable(bool interactable)
    {
        if (interactable)
            ClearSelection();

        foreach (CardView cardView in spawnedCards)
        {
            if (cardView != null)
                cardView.SetInteractable(interactable);
        }
    }
    private void HandleCardClicked(CardView cardView)
    {
        Debug.Log("HANDVIEW RECEIVED CARD CLICK");

        if (cardView == null || cardView.CurrentCard == null)
            return;

        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = cardView;
        selectedCard.SetSelected(true);

        Debug.Log($"Selected card: {cardView.CurrentCard.element} {cardView.CurrentCard.value}");
        CardSelected?.Invoke(cardView.CurrentCard);
    }
    public void ClearHand()
    {
        foreach (CardView cardView in spawnedCards)
        {
            if (cardView != null)
            {
                cardView.Clicked -= HandleCardClicked;
                Destroy(cardView.gameObject);
            }
        }

        spawnedCards.Clear();
        selectedCard = null;
    }
}