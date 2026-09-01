using GameState;
using Unity.Netcode;
using UnityEngine;

public class ClientGameActionHandler : NetworkBehaviour
{
    public ClientGameActionState localClientActionState;
    public bool localCanTakeActions;

    /// <summary>
    /// Called from server to request clients to send their current game actions via SendPlayerActionStateServerRpc(byte[] data)
    /// </summary>
    [ClientRpc]
    public void RequestPlayersActionStatesClientRpc()
    {
        if (IsServer) return;
        SendPlayerActionStateServerRpc(ObjectSerialization.SerializeToByteArray(localClientActionState));
    }

    [ServerRpc] void SendPlayerActionStateServerRpc(byte[] data)
    {

    }

    /// <summary>
    /// Can be called locally to reset all actions including standing and move orders.
    /// </summary>
    public void ResetGameActions()
    {
        localClientActionState = new ClientGameActionState(NetworkManager.Singleton.LocalClientId, 4); //todo
        
        //todo update ui
    }
}
