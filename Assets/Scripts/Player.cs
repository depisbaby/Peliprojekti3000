//Author Luka Brännlund
//
//THE object that represents the player in game.

using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using WebSocketSharp;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;


public class Player : NetworkBehaviour
{
    //THE username to refer to
    public NetworkVariable<FixedString128Bytes> username = new NetworkVariable<FixedString128Bytes>(
    default,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
    );

    //used in lobby to determine if player is ready to play
    public NetworkVariable<bool> ready = new NetworkVariable<bool>( 
    default,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
    );


    //This is called as soon as the player object is spawned on the server.
    //This acts as an entry point for all the needed operations once a new player joins such as adding player to lobby, requesting the username from the client etc.
    void Start()
    {
        if (IsServer) //server stuff
        {
            Lobby.Instance.AddNewPlayer(this);
        }

        if (IsOwner) //owner stuff
        {
            Lobby.Instance.localPlayer = this;//set this object as the local player for future reference

            string _userName = MainMenu.Instance.GetLocalUserName(); 
            if (_userName == "" || _userName.IsNullOrEmpty())//make sure you have a username
            {
                _userName = "Unknown" + Random.Range(0,1000000).ToString();
            }
            username.Value = _userName;
        }
    }

    //This is called as soon as the player object is despawned on the server.
    //This acts as an entry point for all the needed operations once a player leaves the game such as removing player from lobby, etc.
    private void OnDestroy()
    {
        if (IsServer) //server stuff
        {
            Lobby.Instance.RemovePlayer(this);
        }
    }


}
