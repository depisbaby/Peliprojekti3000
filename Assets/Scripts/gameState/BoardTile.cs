using UnityEngine;

namespace GameState
{
    /// <summary>
    /// a single tile of the gameboard
    /// </summary>
    [System.Serializable]
    public class BoardTile
    {

        public BoardTile(Vector2Int _position)
        {
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
}
