using UnityEngine;

namespace GameState
{
    /// <summary>
    /// A player object that lives inside of the SharableGameState. Each client can control one of these.
    /// </summary>
    [System.Serializable]
    public class Player
    {
        public ulong controllerClientId; //the NFGO Client ID of the current controller. Can be used to figure out which of the users control which Player object.
        public bool controllerConnected; //true= controller client is connected in game, false= controller is disconnected from the game.
    }
    
}

