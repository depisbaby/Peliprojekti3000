using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ClientGameActionState : MonoBehaviour
{
    /// <summary>
    /// Creates a new client game actions object. "_clientId" is the owner of these actions, "numberOfPlayers" is the number of players in the game.
    /// </summary>
    /// <param name="_clientId"></param>
    /// <param name="numberOfPlayers"></param>
    public ClientGameActionState(ulong _clientId, int numberOfPlayers)
    {
        clientId = _clientId;
        standings = new bool[numberOfPlayers];
        moveOrders = new List<MoveOrder>();
    }

    public ulong clientId; //the sender of the state object
    public bool[] standings; //standings[0] = standing towards player 1, standings[1] = standing towards player 2, etc. true = peaceful, false = hostile. TOWARDS SELF ALWAYS PEACEFUL
    public List<MoveOrder> moveOrders; // a list of the moves player wants to perform

    public struct MoveOrder
    {
        public int unitGlobalId;// the global unit id of the unit that is moved
        public Vector2 destinationPosition;// the destination position of the unit
    }
}
