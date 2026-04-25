using Fusion;
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
    public void RPC_SubmitCard(PlayerRef submittingPlayer, int element, int value)
    {
        if (!HasStateAuthority)
            return;

        if (Phase != MatchPhase.WaitingForSelections)
            return;

        if (submittingPlayer == PlayerA && !PlayerASubmitted)
        {
            PlayerAElement = element;
            PlayerAValue = value;
            PlayerASubmitted = true;
        }
        else if (submittingPlayer == PlayerB && !PlayerBSubmitted)
        {
            PlayerBElement = element;
            PlayerBValue = value;
            PlayerBSubmitted = true;
        }

        if (PlayerASubmitted && PlayerBSubmitted)
            ResolveRound();
    }

    private void ResolveRound()
    {
        CardData cardA = new CardData("A", (CardData.Element)PlayerAElement, PlayerAValue);
        CardData cardB = new CardData("B", (CardData.Element)PlayerBElement, PlayerBValue);

        RoundOutcome outcome = CardComparer.Compare(cardA, cardB);

        if (outcome == RoundOutcome.PlayerOneWins)
        {
            WinnerSlot = 0;
            PlayerARoundWins++;
        }
        else if (outcome == RoundOutcome.PlayerTwoWins)
        {
            WinnerSlot = 1;
            PlayerBRoundWins++;
        }
        else
        {
            WinnerSlot = 2;
        }

        if (PlayerARoundWins >= RoundsToWin || PlayerBRoundWins >= RoundsToWin)
        {
            Phase = MatchPhase.MatchFinished;
        }
        else
        {
            Phase = MatchPhase.RoundFinished;
        }
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
        Phase = MatchPhase.WaitingForSelections;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayAgain(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        if (player == PlayerA)
            PlayerAPlayAgain = true;
        else if (player == PlayerB)
            PlayerBPlayAgain = true;

        if (!PlayerAPlayAgain || !PlayerBPlayAgain)
            return;

        ResetFullMatch();

        Phase = MatchPhase.WaitingForReady;
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
        PlayerARoundWins = 0;
        PlayerBRoundWins = 0;

        RoundNumber = 1;

        ResetRoundState();
        Phase = MatchPhase.WaitingForSelections;
    }
    public void HandlePlayerLeft(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        PlayerDisconnected = true;

        ResetRoundState();

        PlayerA = PlayerRef.None;
        PlayerB = PlayerRef.None;

        PlayerAReady = false;
        PlayerBReady = false;
        PlayerAPlayAgain = false;
        PlayerBPlayAgain = false;

        PlayerARoundWins = 0;
        PlayerBRoundWins = 0;
        RoundNumber = 0;

        Phase = MatchPhase.WaitingForPlayers;
    }
}