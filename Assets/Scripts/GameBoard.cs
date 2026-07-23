//Author Luka Brännlund
//
//The physical game board and user interface for selecting units, moving them etc.
//Lets try to keep this local only lol

using UnityEngine;

public class GameBoard : MonoBehaviour
{
    #region Singleton
    public static GameBoard Instance { get; private set; }

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

    //Is called everytime the game state is synced in GameManager.
    //Syncs the game state. Moves units, updates combat values etc.
    public void SyncGameState(GameManager.SharableGameState gameState)
    {

    }

}
