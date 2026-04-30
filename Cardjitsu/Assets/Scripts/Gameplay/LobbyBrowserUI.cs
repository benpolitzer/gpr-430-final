using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VG.Tweener;

public class LobbyBrowserUI : MonoBehaviour
{
    [SerializeField] private FusionBootstrap fusionBootstrap;
    [SerializeField] private Transform contentParent;
    [SerializeField] private LobbyRowUI lobbyRowPrefab;
    [SerializeField] private TMP_Text noLobbiesText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button hostButton; 
    [SerializeField] private TMP_Text playersOnlineText;

    private readonly List<LobbyRowUI> spawnedRows = new();
    private int lastSessionHash = -1;

    private void Start()
    {
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshLobbyList);

        if (fusionBootstrap == null)
            fusionBootstrap = FindFirstObjectByType<FusionBootstrap>();

        fusionBootstrap.StartLobbyBrowser();

        RefreshLobbyList();
    }

    private void Update()
    {
        int hash = 17;

        foreach (SessionInfo session in FusionBootstrap.CachedSessions)
        {
            hash = hash * 31 + session.Name.GetHashCode();
            hash = hash * 31 + session.PlayerCount;
            hash = hash * 31 + session.IsOpen.GetHashCode();
            hash = hash * 31 + session.IsVisible.GetHashCode();
        }

        if (hash != lastSessionHash)
        {
            lastSessionHash = hash;
            RefreshLobbyList();
        }
    }

    public void RefreshLobbyList()
    {
        ClearRows();

        int shownCount = 0;
        int playersInLobbies = 0;

        foreach (SessionInfo session in FusionBootstrap.CachedSessions)
        {
            if (!session.IsVisible)
                continue;

            playersInLobbies += session.PlayerCount;

            if (!session.IsOpen)
                continue;

            LobbyRowUI row = Instantiate(lobbyRowPrefab, contentParent);

            row.Setup(
                session.Name,
                session.PlayerCount,
                session.MaxPlayers,
                HandleJoinClicked
            );

            spawnedRows.Add(row);
            shownCount++;
        }

        if (playersOnlineText != null)
            playersOnlineText.text = $"PLAYERS ONLINE: {playersInLobbies}";

        if (noLobbiesText != null)
            noLobbiesText.gameObject.SetActive(shownCount == 0);
    }

    private void HandleJoinClicked(string sessionName)
    {
        if (fusionBootstrap == null)
            fusionBootstrap = FindFirstObjectByType<FusionBootstrap>();

        fusionBootstrap.StartClient(sessionName);
    }

    private void ClearRows()
    {
        foreach (LobbyRowUI row in spawnedRows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }

        spawnedRows.Clear();
    }

    // UI events
    public void OnMouseEnterHost(BaseEventData data)
    {
        TMP_Text label = hostButton.transform.GetChild(0).GetComponent<TMP_Text>();

        Tweener.LocalScale(hostButton.gameObject, Vector3.one * 1.2f, 0.25f, EasingStyle.Quintic);
        Tweener.TextColor(label, new(1f, .95f, 0), .5f, EasingStyle.Quintic);
    }

    public void OnMouseLeaveHost()
    {
        TMP_Text label = hostButton.transform.GetChild(0).GetComponent<TMP_Text>();

        Tweener.LocalScale(hostButton.gameObject, Vector3.one, 0.25f, EasingStyle.Quintic);
        Tweener.TextColor(label, Color.white, .5f, EasingStyle.Quintic);
    }

    public void OnMouseEnterRefresh()
    {
        TMP_Text label = refreshButton.transform.GetChild(0).GetComponent<TMP_Text>();

        Tweener.LocalScale(refreshButton.gameObject, Vector3.one * 1.2f, 0.25f, EasingStyle.Quintic);
        Tweener.TextColor(label, new(1f, .95f, 0), .5f, EasingStyle.Quintic);
    }

    public void OnMouseLeaveRefresh()
    {
        TMP_Text label = refreshButton.transform.GetChild(0).GetComponent<TMP_Text>();

        Tweener.LocalScale(refreshButton.gameObject, Vector3.one, 0.25f, EasingStyle.Quintic);
        Tweener.TextColor(label, Color.white, .5f, EasingStyle.Quintic);
    }
}