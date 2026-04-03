using System.Linq;
using Fusion;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    [Networked] public int ConnectedPlayerCount { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            ConnectedPlayerCount = Runner.ActivePlayers.Count();
        }

        Debug.Log("NetworkGameManager spawned");
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            ConnectedPlayerCount = Runner.ActivePlayers.Count();
        }
    }
}