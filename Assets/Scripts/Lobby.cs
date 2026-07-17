//Author Luka Brännlund
//
//Tracks the players throught out the game, handles server-side user connection and disconnection
//
//


using UnityEngine;
using Unity.Netcode;
using UnityEditor;
using System.Collections.Generic;

using System.Threading.Tasks;
using Unity.Collections;
using TMPro;
public class Lobby : NetworkBehaviour
{
    #region Singleton
    public static Lobby Instance { get; private set; }

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

    private Dictionary<ulong,Player> players = new Dictionary<ulong,Player>();
    [SerializeField] GameObject uiBase;
    [SerializeField] TMP_Text chat;
    [SerializeField] TMP_InputField chatField;

    public void HostLobby()
    {
        MainMenu.Instance.CloseUI();
        uiBase.SetActive(true);
    }

    public void JoinLobby()
    {
        MainMenu.Instance.CloseUI();
        uiBase.SetActive(false);
    }

    public async void AddNewPlayer(Player player) //when a new player prefab is spawned, this adds the player into the game
    {
        if (!IsServer) return; 

        while (player.username.Value == default(string)) //wait until the owner of the player sets the user name
        {
            await Task.Yield();
        }

        players.Add(player.OwnerClientId, player);
        SendChatMessageClientRpc("[SYSTEM]: "+ player.username.Value+ " joined the game!");
    }

    #region ServerRPC
    [ServerRpc] public void SendChatMessageServerRpc(string message)
    {
        SendChatMessageClientRpc(message);
    }
    #endregion

    #region ClientRPC
    [ClientRpc]
    public void SendChatMessageClientRpc(string message)
    {
        chat.text = chat.text + message + "\n";
    }
    #endregion



}
