using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private NetworkPrefabRef gameManagerPrefab;

    private NetworkObject spawnedGameManager;
    private void Update()
    {
        if (runner == null)
            return;

        if (runner.SessionInfo.IsValid && statusText != null)
        {
            statusText.text = $"Connected\n" + $"Mode: {runner.GameMode}\n" + $"Session: {runner.SessionInfo.Name}\n" + 
                $"Region: {runner.SessionInfo.Region}\n" + $"Players: {runner.ActivePlayers.Count()}";
        }
    }
    private void SetStatus(string msg)
    {
        Debug.Log(msg);

        if (statusText != null)
            statusText.text = msg;
    }

    public async void StartHost(string sessionName)
    {
        await StartRunner(GameMode.Host, sessionName);
    }

    public async void StartClient(string sessionName)
    {
        await StartRunner(GameMode.Client, sessionName);
    }

    private async System.Threading.Tasks.Task StartRunner(GameMode mode, string sessionName)
    {
        if (runner != null)
            return;

        SetStatus($"Starting {mode}...");

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = false;

        var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            SetStatus($"{mode} started. Waiting for connection...");
        }
        else
        {
            SetStatus($"StartGame failed: {result.ShutdownReason}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        SetStatus("Connected to Photon/Fusion session");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SetStatus($"Player joined. Players now: {runner.ActivePlayers.Count()}");

        if (runner.IsServer && spawnedGameManager == null)
        {
            spawnedGameManager = runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);
            SetStatus($"Spawned GameManager. Players now: {runner.ActivePlayers.Count()}");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        SetStatus($"Player left. Players now: {runner.ActivePlayers.Count()}");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        SetStatus($"Disconnected: {reason}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SetStatus($"Shutdown: {shutdownReason}");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        SetStatus($"Connect failed: {reason}");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}