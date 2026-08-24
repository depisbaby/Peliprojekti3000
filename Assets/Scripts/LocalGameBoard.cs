//Author Luka Brännlund
//
//The physical game board and user interface for selecting units, moving them etc.
//Lets try to keep this local only lol

using UnityEngine;
using GameState;
using System.Runtime.InteropServices.WindowsRuntime;

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
    public LocalTile[,] localMap;
    int mapSize;


    private void Start()
    {
        InitLocalMap(20);
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

        //todo sync what ever the fuk
    }

    //syncs singular tile
    void SyncTile(LocalTile localTile, BoardTile serverTile)
    {
        if (localTile.type == serverTile.type) return; // tiles already in sync
        SetTileType(localTile,serverTile.type);

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

}
