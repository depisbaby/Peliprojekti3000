//Author Luka Brännlund
//
//The physical game board and user interface for selecting units, moving them etc.
//Lets try to keep this local only lol

using UnityEngine;
using GameState;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using System.Collections.Generic;

public class LocalGameBoard : MonoBehaviour
{
    #region Singleton
    public static LocalGameBoard Instance { get; private set; }

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

    [SerializeField] private LocalTile localTilePrefab;
    [SerializeField] private LocalTile unitPrefab;
    public LocalTile[,] localMap;
    int mapSize;
    List<LocalUnit> localUnits = new List<LocalUnit>();
    

    private void Start()
    {
        //InitLocalMap(20);
    }

    //Is called everytime the game state is synced in GameManager.
    //Syncs the game state. Moves units, updates combat values etc.
    public void SyncGameState(SharableGameState gameState)
    {
        if (mapSize == 0) //map is not initialized
        {
            InitLocalMap(mapSize);
        }

        //sync tiles
        for (int y = 0; y < gameState.mapSize; y++)
        {
            for (int x = 0; x < gameState.mapSize; x++)
            {
                SyncTile(localMap[x, y], gameState.map[x, y]);
            }
        }

        //Todo sync units
        SyncUnits(gameState);

        //todo sync what ever the fuk
    }

    //syncs singular tile
    void SyncTile(LocalTile localTile, BoardTile serverTile)
    {
        if (localTile.type == serverTile.type) return; // tiles already in sync
        SetTileType(localTile,serverTile.type);

    }

    //sync all local units
    void SyncUnits(SharableGameState gameState)
    {
        //code feels verys stupid but its prolly fine.

        List<BoardUnit> serverUnits = new List<BoardUnit>(); //add server to a new list where they can be removed along the way
        foreach (BoardUnit serverUnit in gameState.Units)
        {
            serverUnits.Add(serverUnit);
        }

        foreach (var localUnit in localUnits) //go through each local unit
        {
            bool found = false;
            foreach (var serverUnit in serverUnits)// go through each server unit
            {
                if (serverUnit.globalId == localUnit.globalId)//unit still exists on the gameboard
                {
                    SyncUnitState(localUnit, serverUnit);//sync the unit
                    found = true;
                    serverUnits.Remove(serverUnit);
                    break;
                }
            }

            if (found) continue;

            //local unit no longer exists according to server
            DespawnUnit(localUnit);

        }

        if(serverUnits.Count > 0) //there are new units that need to be spawned locally
        {

        }
    }

    void SyncUnitState(LocalUnit localUnit, BoardUnit serverUnit)
    {
        //TODO sync position
    }

    //set the local type of the tile
    void SetTileType(LocalTile localTile, int type)
    {
        localTile.type = type;
    }

    //initializes the local game board spawning the hexagons etc.
    void InitLocalMap(int _mapSize)
    {

        mapSize = _mapSize;
        for (int y = 0; y < _mapSize; y++)
        {
            for (int x = 0; x < _mapSize; x++)
            {
                GameObject go = Instantiate(localTilePrefab.gameObject);
                LocalTile localTile = go.GetComponent<LocalTile>();
                go.transform.position = new Vector3(x * 1.74f + (y * 0.87f), 0, y * -1.51f);
                localTile.gridPosition = new Vector2(x,y);
            }
        }
    }

    void SpawnUnit(
        int ownerId,
        int globalId,
        Vector2 gridPosition
        )
    {
        
    }

    void DespawnUnit(LocalUnit localUnit)
    {
        //TODO some sort of death animation

        localUnits.Remove(localUnit);
        Destroy(localUnit.gameObject);
    }


}
