using UnityEngine;


namespace GameState
{
    /// <summary>
    /// a single unit on the gameboard
    /// </summary>
    [System.Serializable]
    public class BoardUnit
    {
        public int Size = 1;
        public int X;
        public int Y;

        public int ownerPlayerId; //TODO Needs to be assigned. Units owned by player 1 has ownerId=1 etc.
        public int globalId; //TODO Needs to be assigned. Global id may never be repeated. (maybe just increment the int forever?)

        static int UnitIDCounter;// static int that gets incremented to have a unique id for all units

        public BoardUnit(int size, Vector2Int Position, int _ownerId)
        {
            UnitIDCounter++;
            globalId = UnitIDCounter;

            Size = size;
            X = Position.x;
            Y = Position.y;
            ownerPlayerId = _ownerId;
        }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(X, Y);
        }

        public void SetPosion(Vector2Int NewPosition)
        {
            X = NewPosition.x;
            Y = NewPosition.y;
        }
    }
}
