//Author Luka Brännlund
//
//Responsible for the core game loop
//
//


using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using static GameManager;
using GameState;
using AtomicConsole;

[System.Serializable]
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
    public SharableGameState sharableGameState;
    


    //Entry point of the game
    //This gets called when all of the players in game have pressed the ready button in the lobby screen
    public void StartGame()
    {
        if (!IsServer) return; //server only
        if (isGameStarted) return;
        isGameStarted = true;
        Lobby.Instance.SendChatMessageClientRpc("Everyone is ready!");
        SendSimpleEventClientRpc("gamestart");
        
        sharableGameState.createMap(7,7);
        sharableGameState.Units.Add(new BoardUnit(1, new Vector2Int(3,3)));
        sharableGameState.Units.Add(new BoardUnit(2, new Vector2Int(5,3)));
        sharableGameState.Units.Add(new BoardUnit(3, new Vector2Int(3,6)));
        SyncSharableGameState();

        //1. Do stuff at the beginning of the game

        foreach (var client in Lobby.Instance.clients) // create player objects for each player
        {
            BindClientToPlayer(client.Value, true);
        }

        while (true)
        {
            //2. Do stuff at the beginning of the round.
            sharableGameState.turnNumber++; //increment turn number

            //3. Set timer for players to move stuff (~30-60sec) AND constantly check player "pass status"

            //4. Get player moves and apply them.

            //5. Sync state and set a small move animation timer (~5-10sec)

            //6. Get each players standing towards each other player and resolve battles accordingly

            //7. Sync state and set a small battle animation timer (~5-10sec)

            //8. Do stuff at the end of the round

        }

        //9. Do stuff at the end of the game

    }

    private void Update()
    {
        //debug stuff        
        sharableGameState.debugDrawMap();

        if (Input.GetKeyDown(KeyCode.W)) {
            foreach (BoardUnit u in sharableGameState.Units) {
                sharableGameState.MoveUnit(u, new Vector2Int(0, -1));
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (BoardUnit u in sharableGameState.Units)
            {
                sharableGameState.MoveUnit(u, new Vector2Int(-1, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (BoardUnit u in sharableGameState.Units)
            {
                sharableGameState.MoveUnit(u, new Vector2Int(0, 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (BoardUnit u in sharableGameState.Units)
            {
                sharableGameState.MoveUnit(u, new Vector2Int(1, 0));
            }
        }
    }

    [AtomicCommand(name: "MoveAllUp", description: "Move all units up 1 tile")]
    public void MoveAllUp()
    {
        foreach (BoardUnit u in sharableGameState.Units)
        {
            sharableGameState.MoveUnit(u, new Vector2Int(0, -1));
        }
    }

    /// <summary>
    /// Syncs the current game state with the clients. Call on server only.
    /// </summary>
    public void SyncSharableGameState()
    {
        if(!IsServer) return; //server only

        //TODO MAYBE: Each client should be sent a different version of the game state depending on hidden information (such as hidden units, and fog of war) to prevent cheating.
        //It depends of whether we care if players cheat or not in a friend-slop game.


        SyncSharableGameStateClientRpc(ObjectSerialization.SerializeToByteArray(sharableGameState)); //as of now every client is sent the same game state
    }

    [ClientRpc]
    void SyncSharableGameStateClientRpc(byte[] state)
    {
        SharableGameState NewState = ObjectSerialization.Deserialize<SharableGameState>(state);

        if (IsServer) return;//just in case lol
        sharableGameState = NewState;//set the game state variable of the clients just in case
        LocalGameBoard.Instance.SyncGameState(NewState); //sync local game board
    }

    /// <summary>
    ///A crude way to send some simple events to clients that can be fully parsed with a string tag alone (such as tell clients that game started using the tag "gamestart").
    /// </summary>
    [ClientRpc] public void SendSimpleEventClientRpc(string tag)
    {
        switch (tag)
        {
            case "gamestart"://sent when the game starts
                Lobby.Instance.CloseUI();
                return;
            default: return;
        }
    }
    /// <summary>
    /// Converts two-dimentional position value to one-dimentional index. Can be used to get a value from a 1D array that is ordered like a 2D array.
    /// </summary>
    /// <param name="position">Position in a 2D grid</param>
    /// <param name="mapSize">The size of the map</param>
    /// <returns></returns>
    public int Vector2ToArrayIndex(Vector2 position, int mapSize)
    {
        return (int)position.x + (int)position.y * mapSize;
    }

    /// <summary>
    /// Gets called on each connected client when the game starts and when a client reconnects to a game.
    /// </summary>
    /// <param name="client">The client you want the player bind to</param>
    /// <param name="canCreateNewPlayers">Weather this can create new players. At the start of the game this is true, but during the game this should be false. (New player objects should not be created during the game)</param>
    void BindClientToPlayer(Client client, bool canCreateNewPlayers)
    {
        foreach (var player in sharableGameState.Players) //check if there is already a player object without a controller and bind to it (e.g. when a client is reconnecting to a game)
        {
            if (!player.controllerConnected) //this player is missing a controller
            {
                //TODO Implement a way for the server to remember each client that has connected before and bind accordingly (using ip?)

                //FOR NOW, bind to first available player object (May lead to situations where two disconnected players may swap their player objects, once they reconnect)
                player.controllerConnected = true;
                player.controllerClientId = client.OwnerClientId;
            }
        }

        if (!canCreateNewPlayers) //is true when game is starting, false when game is on-going
        {
            //TODO handle spectator?
            return;
        }

        //create a new player object and bind to it
        Player newPlayer = new Player();
        newPlayer.controllerConnected = true;
        newPlayer.controllerClientId = client.OwnerClientId;
        sharableGameState.Players.Add(newPlayer);

    }

    private void OnDrawGizmos()
    {
        if (sharableGameState.map == null)
            return;

        foreach (BoardTile tile in sharableGameState.map)
        {

            Vector2Int tilepos = new Vector2Int(tile.positionX, tile.positionY);

            Vector2 point = sharableGameState.MapToWorldPos(tilepos);

            foreach (BoardTile nextTile in sharableGameState.GetAdjacentTiles(tilepos))
            {

                Vector2Int nextTilepos = new Vector2Int(nextTile.positionX, nextTile.positionY);

                Debug.DrawLine(point, sharableGameState.MapToWorldPos(nextTilepos), Color.red);
            }
        }

        //Gizmos.color = Color.green;
        foreach (BoardUnit u in sharableGameState.Units)
        {
            Gizmos.DrawWireSphere(sharableGameState.MapToWorldPos(u.GetPosition()), Mathf.Sqrt(u.Size) * 0.1f);            
        }
    }
}
