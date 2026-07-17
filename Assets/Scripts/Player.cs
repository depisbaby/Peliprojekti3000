//Author Luka Brännlund
//
//THE object that represents the player in game.

using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> username = new NetworkVariable<FixedString128Bytes>(); 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsServer) //server stuff
        {
            Lobby.Instance.AddNewPlayer(this);
        }

        if (IsOwner) //owner stuff
        {
            string _userName = MainMenu.Instance.GetLocalUserName();
            if (_userName == "")
            {
                _userName = "Guest";
            }
            username.Value = _userName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
