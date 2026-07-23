//Author Luka Brännlund
//
//Responsible for the core game loop
//
//


using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine.Playables;
using Unity.Collections;

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

    //IMPORTANT!!!
    //A large portion, if not all, of the game state is tracked as a "SharableGameState" struct.
    //Things such as player unit positions, unit combat values, tile statuses, turn number etc. are all tracked here.
    //The the game state can be synced by calling SyncSharableGameState().
    public struct SharableGameState : INetworkSerializable
    {
        //the current turn number
        public int turnNumber;

        //the width and the height of the map
        public int mapSize;

        //The representation of the tiles on the map. Using 2D array didn't work with the NetworkSerialize. Use Vector2ToArrayIndex() to convert 2D position into 1D index.
        //The map is not going to be big (maybe 19x19 max) so this is fine.
        //See \Peliprojekti3000\Documentation\tiling_diagram.png
        //0=no tile
        //1=normal tile
        //2=forest tile
        public int[] map;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref turnNumber);
            serializer.SerializeValue(ref mapSize);
            serializer.SerializeValue(ref map);
        }
    }

    //Entry point of the game
    //This gets called when all of the players in game have pressed the ready button in the lobby screen
    public void StartGame()
    {
        if (!IsServer) return; //server only
        if (isGameStarted) return;
        isGameStarted = true;
        Lobby.Instance.SendChatMessageClientRpc("Everyone is ready!");
        SendSimpleEventClientRpc("gamestart");
        

    }

    /// <summary>
    /// Syncs the current game state with the clients. Call on server only.
    /// </summary>
    public void SyncSharableGameState()
    {
        if(!IsServer) return; //server only

        //TODO MAYBE: Each client should be sent a different version of the game state depending on hidden information (such as hidden units, and fog of war) to prevent cheating.
        //It depends of whether we care if players cheat or not in a friend-slop game.
       

        SyncSharableGameStateClientRpc(sharableGameState); //as of now every client is sent the same game state
    }

    [ClientRpc]
    void SyncSharableGameStateClientRpc(SharableGameState state)
    {
        if (IsServer) return;//just in case lol
        sharableGameState = state;//set the game state variable of the clients just in case
        GameBoard.Instance.SyncGameState(state); //sync game board
    }

    /// <summary>
    ///A crude way to send some simple events to clients that can be fully parsed with a string tag alone (such as tell clients that game started using the tag "gamestart").
    /// </summary>
    [ClientRpc] public void SendSimpleEventClientRpc(string tag)
    {
        switch (tag)
        {
            case "gamestart":
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


}
