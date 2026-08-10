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

    //IMPORTANT!!!
    //A large portion, if not all, of the game state is tracked as a "SharableGameState" struct.
    //Things such as player unit positions, unit combat values, tile statuses, turn number etc. are all tracked here.
    //The the game state can be synced by calling SyncSharableGameState().
    [System.Serializable]
    public class SharableGameState
    {
        //the current turn number
        public int turnNumber;

        //the width and the height of the map
        public int mapSize;

        /// <summary>
        /// tiles of the map.
        /// See \Peliprojekti3000\Documentation\tiling_diagram.png for visualisation of their arrangement
        /// </summary>
        public BoardTile[,] map;

        /// <summary>
        /// retruns adjacent tiles to the given tile. Only returns existing tiles \Peliprojekti3000\Documentation\tiling_diagram.png for tiling reference
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>returns a list of adjacent tiles. arbitrary length of max 6</returns>
        public List<BoardTile> GetAdjacentTiles(Vector2Int position) { 

            
            List<BoardTile> result = new List<BoardTile>();

            BoardTile t;
            t = getTile(position + new Vector2Int(1, 0));
            if (t != null) { result.Add(t); }

            t = getTile(position + new Vector2Int(0, 1));
            if (t != null) { result.Add(t); }

            t = getTile(position + new Vector2Int(-1, 0));
            if (t != null) { result.Add(t); }

            t = getTile(position + new Vector2Int(0, -1));
            if (t != null) { result.Add(t); }

            //gets adjacent tiles in directions that are diagonal on a square grid. these alternate to account for the way the map is laid out (\Peliprojekti3000\Documentation\tiling_diagram.png)
            if (position.y % 2 == 0)
            {
                t = getTile(position + new Vector2Int(-1, 1));
                if (t != null) { result.Add(t); }

                t = getTile(position + new Vector2Int(-1, -1));
                if (t != null) { result.Add(t); }
            }
            else {
                t = getTile(position + new Vector2Int(1, 1));
                if (t != null) { result.Add(t); }

                t = getTile(position + new Vector2Int(1, -1));
                if (t != null) { result.Add(t); }
            }
                



            return result;

            //local method to make getting a single tile easier
            BoardTile getTile(Vector2Int position) {
   
                //retrun null if position is out of bounds
                if (position.x < 0 || position.y < 0 || position.x >= map.GetLength(0) || position.y >= map.GetLength(1))
                    return null;

                BoardTile t = map[position.x, position.y];

                return t;
            }
        }

        /// <summary>
        /// create a map of size Xsize, Ysize
        /// </summary>
        /// <param name="Xsize"></param>
        /// <param name="Ysize"></param>
        public void createMap(int Xsize, int Ysize) {
            map = new BoardTile[Xsize, Ysize];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x,y] = new BoardTile(new Vector2Int(x,y));
                }
            }
        }


        /// <summary>
        /// draw the maps connections as a set of lines for debug purposes
        /// </summary>
        public void debugDrawMap() {
            Vector2[,] points = new Vector2[map.GetLength(0), map.GetLength(1)];

            foreach (BoardTile tile in map) {

                Vector2Int tilepos = new Vector2Int(tile.positionX, tile.positionY);

                Vector2 point = MapToWorldPos(tilepos);

                foreach (BoardTile nextTile in GetAdjacentTiles(tilepos)) { 

                    Vector2Int nextTilepos = new Vector2Int(nextTile.positionX, nextTile.positionY);

                    Debug.DrawLine(point, MapToWorldPos(nextTilepos), Color.red);
                }
            }


        }

        /// <summary>
        /// convert from an integer position on the map to a corresponding grid position according to \Peliprojekti3000\Documentation\tiling_diagram.png
        /// </summary>
        /// <param name="mapPos"></param>
        /// <returns></returns>
        Vector2 MapToWorldPos(Vector2Int mapPos)
        {
            Vector2 right = Vector2.right;
            Vector2 down = Vector2.down + (Vector2.right * 0.5f);

            return (right * (mapPos.x - (mapPos.y / 2))) + (down * mapPos.y);
        }
    }

    [System.Serializable]
    public class BoardTile {

        public BoardTile(Vector2Int _position) {
            type = 1;
            positionX = _position.x;
            positionY = _position.y;
        }

        /// <summary>
        /// 
        /// type of tile on the map. 
        /// 0=no tile
        /// 1=normal tile      
        /// 2=forest tile     
        /// </summary>
        public int type;

        public int positionX;
        public int positionY;
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

        sharableGameState.createMap(7,7);
        SyncSharableGameState();
    }

    private void Update()
    {
        //debug stuff        
        sharableGameState.debugDrawMap();
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
        GameBoard.Instance.SyncGameState(NewState); //sync game board
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
}
