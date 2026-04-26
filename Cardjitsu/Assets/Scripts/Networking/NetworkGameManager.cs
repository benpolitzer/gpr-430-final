using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    [Networked] public MatchPhase Phase { get; set; }
    [Networked] public PlayerRef PlayerA { get; set; }
    [Networked] public PlayerRef PlayerB { get; set; }
    [Networked] public bool PlayerASubmitted { get; set; }
    [Networked] public bool PlayerBSubmitted { get; set; }
    [Networked] public int PlayerAElement { get; set; }
    [Networked] public int PlayerAValue { get; set; }
    [Networked] public int PlayerBElement { get; set; }
    [Networked] public int PlayerBValue { get; set; }
    [Networked] public int WinnerSlot { get; set; } // -1 none, 0 A, 1 B, 2 tie
    [Networked] public int PlayerARoundWins { get; set; }
    [Networked] public int PlayerBRoundWins { get; set; }
    [Networked] public bool PlayerAReady { get; set; }
    [Networked] public bool PlayerBReady { get; set; }
    [Networked] public int RoundNumber { get; set; }
    [Networked] public NetworkString<_16> PlayerAName { get; set; }
    [Networked] public NetworkString<_16> PlayerBName { get; set; }
    private const int RoundsToWin = 3;

    [Networked] public bool PlayerAPlayAgain { get; set; }
    [Networked] public bool PlayerBPlayAgain { get; set; }
    [Networked] public bool PlayerDisconnected { get; set; }
    [Networked] public int PlayerAColor { get; set; }
    [Networked] public int PlayerBColor { get; set; }
    private const int MaxWonCards = 9;

    [Networked] public int PlayerAWonCount { get; set; }
    [Networked] public int PlayerBWonCount { get; set; }

    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerAWonElements { get; }
    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerAWonColors { get; }
    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerAWonValues { get; }

    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerBWonElements { get; }
    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerBWonColors { get; }
    [Networked, Capacity(MaxWonCards)] public NetworkArray<int> PlayerBWonValues { get; }
    [Networked] public int MatchWinner { get; set; } // -1 none, 0 A, 1 B

    private const int HandSize = 5;

    [Networked] public int PlayerAHandCount { get; set; }
    [Networked] public int PlayerBHandCount { get; set; }

    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerAHandElements { get; }
    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerAHandColors { get; }
    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerAHandValues { get; }

    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerBHandElements { get; }
    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerBHandColors { get; }
    [Networked, Capacity(HandSize)] public NetworkArray<int> PlayerBHandValues { get; }
    private void DealHandsForRound()
    {
        Deck deckA = new Deck();
        Deck deckB = new Deck();

        PlayerAHandCount = 0;
        PlayerBHandCount = 0;

        for (int i = 0; i < HandSize; i++)
        {
            CardData cardA = deckA.PullCard();
            SetPlayerAHandCard(i, cardA);

            CardData cardB = deckB.PullCard();
            SetPlayerBHandCard(i, cardB);
        }

        PlayerAHandCount = HandSize;
        PlayerBHandCount = HandSize;
    }

    private void SetPlayerAHandCard(int index, CardData card)
    {
        PlayerAHandElements.Set(index, (int)card.element);
        PlayerAHandColors.Set(index, (int)card.color);
        PlayerAHandValues.Set(index, card.value);
    }

    private void SetPlayerBHandCard(int index, CardData card)
    {
        PlayerBHandElements.Set(index, (int)card.element);
        PlayerBHandColors.Set(index, (int)card.color);
        PlayerBHandValues.Set(index, card.value);
    }
    private void AddWonCardToPlayerA(CardData card)
    {
        if (PlayerAWonCount >= MaxWonCards)
            return;

        PlayerAWonElements.Set(PlayerAWonCount, (int)card.element);
        PlayerAWonColors.Set(PlayerAWonCount, (int)card.color);
        PlayerAWonValues.Set(PlayerAWonCount, card.value);

        PlayerAWonCount++;
    }

    private void AddWonCardToPlayerB(CardData card)
    {
        if (PlayerBWonCount >= MaxWonCards)
            return;

        PlayerBWonElements.Set(PlayerBWonCount, (int)card.element);
        PlayerBWonColors.Set(PlayerBWonCount, (int)card.color);
        PlayerBWonValues.Set(PlayerBWonCount, card.value);

        PlayerBWonCount++;
    }
    private List<CardData> GetPlayerAWonCards()
    {
        List<CardData> cards = new List<CardData>();

        for (int i = 0; i < PlayerAWonCount; i++)
        {
            cards.Add(new CardData(
                $"A_Won_{i}",
                (CardData.Element)PlayerAWonElements[i],
                (CardData.CardColor)PlayerAWonColors[i],
                PlayerAWonValues[i]
            ));
        }

        return cards;
    }

    private List<CardData> GetPlayerBWonCards()
    {
        List<CardData> cards = new List<CardData>();

        for (int i = 0; i < PlayerBWonCount; i++)
        {
            cards.Add(new CardData(
                $"B_Won_{i}",
                (CardData.Element)PlayerBWonElements[i],
                (CardData.CardColor)PlayerBWonColors[i],
                PlayerBWonValues[i]
            ));
        }

        return cards;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerName(PlayerRef player, string playerName)
    {
        if (!HasStateAuthority)
            return;

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        if (player == PlayerA)
            PlayerAName = playerName;
        else if (player == PlayerB)
            PlayerBName = playerName;
    }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Phase = MatchPhase.WaitingForPlayers;
            ResetRoundState();
        }
    }

    public void RegisterPlayer(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        if (PlayerA == PlayerRef.None)
        {
            PlayerA = player;
        }
        else if (PlayerB == PlayerRef.None && player != PlayerA)
        {
            PlayerB = player;
        }

        if (PlayerA != PlayerRef.None && PlayerB != PlayerRef.None)
        {
            Phase = MatchPhase.WaitingForReady;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SubmitCard(PlayerRef submittingPlayer, int handIndex)
    {
        if (!HasStateAuthority)
            return;

        if (Phase != MatchPhase.WaitingForSelections)
            return;

        if (handIndex < 0 || handIndex >= HandSize)
            return;

        if (submittingPlayer == PlayerA && !PlayerASubmitted)
        {
            PlayerAElement = PlayerAHandElements[handIndex];
            PlayerAColor = PlayerAHandColors[handIndex];
            PlayerAValue = PlayerAHandValues[handIndex];
            PlayerASubmitted = true;
        }
        else if (submittingPlayer == PlayerB && !PlayerBSubmitted)
        {
            PlayerBElement = PlayerBHandElements[handIndex];
            PlayerBColor = PlayerBHandColors[handIndex];
            PlayerBValue = PlayerBHandValues[handIndex];
            PlayerBSubmitted = true;
        }

        if (PlayerASubmitted && PlayerBSubmitted)
            ResolveRound();
    }

    private void ResolveRound()
    {
        CardData cardA = new CardData(
            "A",
            (CardData.Element)PlayerAElement,
            (CardData.CardColor)PlayerAColor,
            PlayerAValue
        );

        CardData cardB = new CardData(
            "B",
            (CardData.Element)PlayerBElement,
            (CardData.CardColor)PlayerBColor,
            PlayerBValue
        );

        RoundOutcome outcome = CardComparer.Compare(cardA, cardB);

        if (outcome == RoundOutcome.PlayerOneWins)
        {
            WinnerSlot = 0;
            AddWonCardToPlayerA(cardA);

            WinType winType = WinChecker.GetWinningSet(GetPlayerAWonCards());

            if (winType != WinType.None)
            {
                MatchWinner = 0;
                Phase = MatchPhase.MatchFinished;
                return;
            }
        }
        else if (outcome == RoundOutcome.PlayerTwoWins)
        {
            WinnerSlot = 1;
            AddWonCardToPlayerB(cardB);

            WinType winType = WinChecker.GetWinningSet(GetPlayerBWonCards());

            if (winType != WinType.None)
            {
                MatchWinner = 1;
                Phase = MatchPhase.MatchFinished;
                return;
            }
        }
        else
        {
            WinnerSlot = 2;
        }

        Phase = MatchPhase.RoundFinished;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartNextRound()
    {
        if (!HasStateAuthority)
            return;

        if (Phase != MatchPhase.RoundFinished)
            return;

        RoundNumber++;
        ResetRoundState();
        DealHandsForRound();
        Phase = MatchPhase.WaitingForSelections;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayAgain(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        if (Phase != MatchPhase.MatchFinished)
            return;

        if (player == PlayerA)
            PlayerAPlayAgain = true;
        else if (player == PlayerB)
            PlayerBPlayAgain = true;

        Debug.LogWarning($"Play again: A={PlayerAPlayAgain}, B={PlayerBPlayAgain}");

        if (PlayerAPlayAgain && PlayerBPlayAgain)
        {
            ResetFullMatch();
            Phase = MatchPhase.WaitingForReady;
        }
    }
    public void HandlePlayerLeft(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        Debug.LogWarning($"Player left: {player}");

        PlayerA = PlayerRef.None;
        PlayerB = PlayerRef.None;

        PlayerAReady = false;
        PlayerBReady = false;
        PlayerAPlayAgain = false;
        PlayerBPlayAgain = false;

        PlayerARoundWins = 0;
        PlayerBRoundWins = 0;
        RoundNumber = 0;

        ResetRoundState();

        Phase = MatchPhase.WaitingForPlayers;
    }
    private void ResetFullMatch()
    {
        PlayerAReady = false;
        PlayerBReady = false;

        PlayerAPlayAgain = false;
        PlayerBPlayAgain = false;

        PlayerARoundWins = 0;
        PlayerBRoundWins = 0;

        RoundNumber = 0;

        PlayerAWonCount = 0;
        PlayerBWonCount = 0;

        ResetRoundState();
    }
    private void ResetRoundState()
    {
        PlayerASubmitted = false;
        PlayerBSubmitted = false;

        PlayerAElement = -1;
        PlayerAValue = -1;

        PlayerBElement = -1;
        PlayerBValue = -1;

        WinnerSlot = -1;

        PlayerAColor = -1;
        PlayerBColor = -1;
        MatchWinner = -1;

    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetReady(PlayerRef readyPlayer)
    {
        if (!HasStateAuthority)
            return;

        Debug.LogWarning($"Ready from player: {readyPlayer}");

        if (readyPlayer == PlayerA)
            PlayerAReady = true;
        else if (readyPlayer == PlayerB)
            PlayerBReady = true;

        if (PlayerAReady && PlayerBReady)
            StartGame();
    }

    private void StartGame()
    {
        PlayerAWonCount = 0;
        PlayerBWonCount = 0;

        MatchWinner = -1;
        RoundNumber = 1;

        ResetRoundState();
        DealHandsForRound();

        Phase = MatchPhase.WaitingForSelections;
    }
}