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

    //display-ready, long string of the connected players and their readiness status, that can be read by the clients
    public NetworkVariable<FixedString4096Bytes> playerList = new NetworkVariable<FixedString4096Bytes>(
    default,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
    );

    public Player localPlayer; //A reference to the local player object
    private Dictionary<ulong,Player> players = new Dictionary<ulong,Player>();// the main dictionary that tracks player in joined
    [SerializeField] GameObject uiBase; //base object of the ui elements 
    [SerializeField] TMP_Text chat; 
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] TMP_Text readyButtonText;
    [SerializeField] TMP_Text playersInLobbyText;
    bool isLocalReady;
    ushort tick;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "") //allows the local player to send messages in lobby
        {
            SendChatMessageServerRpc("["+localPlayer.username.Value+"]: " + chatInput.text);
            chatInput.text = "";
            
        }
    }

    private void FixedUpdate()
    {
        tick++;

        if (tick % 50 == 0) //happens every now and then
        {
            UpdatePlayersInLobby();//updates the display-ready player list
            CheckLobbyReadiness();//server checks if players in lobby are ready
        }
    }

    public void HostLobby() //called on host locally after connection has been established
    {
        MainMenu.Instance.CloseUI();//close main menu ui
        uiBase.SetActive(true);//open lobby ui
    }

    public void JoinLobby() //called on joining client locally after connection has been established
    {
        MainMenu.Instance.CloseUI();//close main menu ui
        uiBase.SetActive(true); //open lobby ui
    }

    public async void AddNewPlayer(Player player) //when a new player prefab is spawned, this adds the player into the game
    {
        if (!IsServer) return; //server only

        while (player.username.Value == default(string)) //wait until the owner of the player sets the user name
        {
            await Task.Yield();
        }

        players.Add(player.OwnerClientId, player);//add to the player dict
        SendChatMessageClientRpc("[SYSTEM]: "+ player.username.Value+ " joined the game!"); //send notification to other players

        if (GameManager.Instance.isGameStarted)//is game already running
        {
            //TODO: Handle reconnections to ongoing game
        }

    }

    public void RemovePlayer(Player player)
    {
        SendChatMessageClientRpc("[SYSTEM]: " + player.username.Value + " left the game!");
        players.Remove(player.OwnerClientId);
    }

    /// <summary>
    /// Any client may call this to send a message into chat
    /// </summary>
    /// <param name="message"></param>
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]public void SendChatMessageServerRpc(string message)
    {
        SendChatMessageClientRpc(message); //broadcast message to clients
    }

    /// <summary>
    /// Server may call this to send a message into chat
    /// </summary>
    /// <param name="message"></param>
    [ClientRpc] public void SendChatMessageClientRpc(string message)
    {
        chat.text = chat.text + message + "\n";
    }

    void CheckLobbyReadiness() //go through each player connected and start the game if everyone is ready
    {
        if (!IsServer) return;//server only
        if (GameManager.Instance.isGameStarted) return;

        bool _ready = true;
        foreach (var player in players)
        {
            if (!player.Value.ready.Value)
            {
                _ready = false;
            }

        }
        if (_ready )
        {
            GameManager.Instance.StartGame();
        }
        
    }

    //updates the display-ready playerlist network variable
    void UpdatePlayersInLobby()
    {
        playersInLobbyText.text = "Players in lobby: \n" + playerList.Value.ToString(); //happens on every client

        if (!IsServer) return; //server only, updates the network variable

        string list="";
        foreach (var player in players)
        {
            string readiness;
            if (player.Value.ready.Value)
            {
                readiness = "Ready";
            }
            else
            {
                readiness = "Not ready";
            }

            list = list + "[" + readiness + "] " + player.Value.username.Value + "\n";
        }
        playerList.Value = list;
        
    }

    public void ReadyButtonPressed()//is called by the ready button object in the scene
    {
        if (isLocalReady)
        {
            isLocalReady = false;
            //SendChatMessageServerRpc("[SYSTEM]: "+ localPlayer.username.Value+ " is not ready!");
            localPlayer.ready.Value = false;
            readyButtonText.text = "Ready";
        }
        else
        {
            isLocalReady = true;
            //SendChatMessageServerRpc("[SYSTEM]: " + localPlayer.username.Value + " is ready!");
            localPlayer.ready.Value = true;
            readyButtonText.text = "Unready";
        }
    }






}
