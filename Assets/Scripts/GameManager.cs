//Author Luka Brännlund
//
//Responsible for the core game loop
//
//


using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

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

    public bool isGameStarted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //Entry point of the game
    //This gets called when all of the players in game have pressed the ready button in the lobby screen
    //
    public void StartGame()
    {
        if (!IsServer) return; //server only
        if (isGameStarted) return;
        isGameStarted = true;
        Lobby.Instance.SendChatMessageClientRpc("Everyone is ready!");


    }
    
}
