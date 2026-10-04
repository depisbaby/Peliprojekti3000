using GameState;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ClientGameActionHandler : NetworkBehaviour
{
    #region Singleton
    public static ClientGameActionHandler Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    //A dict of the current client actions. Specific client's actions can found by using their clientId as a key.
    public Dictionary<ulong, ClientGameActionState> clientActions; 

    [HideInInspector] public ClientGameActionState localClientActionState;

    /// <summary>
    /// Called from server to request clients to send their current game actions via SendPlayerActionStateServerRpc(byte[] data)
    /// </summary>
    [ClientRpc]
    void RequestPlayersActionStatesClientRpc()
    {
        if (IsServer) return;
        SendPlayerActionStateServerRpc(NetworkManager.Singleton.LocalClientId, ObjectSerialization.SerializeToByteArray(localClientActionState));
    }

    [ServerRpc] void SendPlayerActionStateServerRpc(ulong sender ,byte[] data)
    {
        ClientGameActionState clientActions = ObjectSerialization.Deserialize<ClientGameActionState>(data);
    }

    /// <summary>
    /// Can be called locally to reset all actions including standing and move orders.
    /// </summary>
    public void ResetLocalGameActions()
    {
        localClientActionState = new ClientGameActionState(NetworkManager.Singleton.LocalClientId); //todo
        localClientActionState.standings = new bool[GameManager.Instance.sharableGameState.Players.Count]; 
        //todo update ui
    }

    //work in progress....
    public Task<Dictionary<ulong, ClientGameActionState>> GetClientActions()
    {
        if (!IsServer) return null;
        Dictionary<ulong, ClientGameActionState> actions = new Dictionary<ulong, ClientGameActionState>();
        actions.Add(0, new ClientGameActionState(69));
        actions.Add(1, new ClientGameActionState(67));
        return Task.FromResult(actions);
    }
}
