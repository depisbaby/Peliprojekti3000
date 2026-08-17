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
        //TODO: mark the owner of the unit

        public BoardUnit(int size, Vector2Int Position)
        {
            Size = size;
            X = Position.x;
            Y = Position.y;
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
