using UnityEngine;

namespace GameState
{
    /// <summary>
    /// A player object that lives inside of the SharableGameState. Each client can control one of these.
    /// </summary>
    [System.Serializable]
    public class Player
    {
        public int playerId; //each player is given an id 0-max
        public ulong controllerClientId; //the NFGO Client ID of the current controller. Can be used to figure out which of the users control which Player object.
        public bool controllerConnected; //true= controller client is connected in game, false= controller is disconnected from the game.

        public bool playerPassedTurn;

        public float[] playerColor = {0.0f, 0.0f, 0.0f }; //3 elements for RGB

        static int playerIdCounter;

        public Player(ulong _controllerClientId)
        {
            playerId = playerIdCounter;
            playerIdCounter++;

            controllerClientId = _controllerClientId;
        }

        public void SetColor(Color color)
        {
            playerColor[0] = color.r;
            playerColor[1] = color.g;
            playerColor[2] = color.b;
        }

        public Color GetColor()
        {
            return new Color(playerColor[0], playerColor[1], playerColor[2]);
           
        }


    }

    
    
}

