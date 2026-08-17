using System.Collections.Generic;
using UnityEngine;

namespace GameState { 
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
        /// units on the map
        /// See \Peliprojekti3000\Documentation\tiling_diagram.png for visualisation of their arrangement
        /// </summary>
        public List<BoardUnit> Units = new List<BoardUnit>();

        /// <summary>
        /// retruns adjacent tiles to the tile at given pos. Only returns existing tiles \Peliprojekti3000\Documentation\tiling_diagram.png for tiling reference
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>returns a list of adjacent tiles. arbitrary length of max 6</returns>
        public List<BoardTile> GetAdjacentTiles(Vector2Int position)
        {


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
            else
            {
                t = getTile(position + new Vector2Int(1, 1));
                if (t != null) { result.Add(t); }

                t = getTile(position + new Vector2Int(1, -1));
                if (t != null) { result.Add(t); }
            }

            return result;


        }

        /// <summary>
        /// get a tile from given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BoardTile getTile(Vector2Int position)
        {

            //retrun null if position is out of bounds
            if (position.x < 0 || position.y < 0 || position.x >= map.GetLength(0) || position.y >= map.GetLength(1))
                return null;

            BoardTile t = map[position.x, position.y];

            return t;
        }

        /// <summary>
        /// create a map of size Xsize, Ysize
        /// </summary>
        /// <param name="Xsize"></param>
        /// <param name="Ysize"></param>
        public void createMap(int Xsize, int Ysize)
        {
            map = new BoardTile[Xsize, Ysize];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = new BoardTile(new Vector2Int(x, y));
                }
            }
        }



        /// <summary>
        /// moves an unity on the map by PositionDelta tiles. fails if moving outside the map
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="PositionDelta"></param>
        public void MoveUnit(BoardUnit Unit, Vector2Int PositionDelta)
        {
            BoardTile tile = getTile((Unit.GetPosition() + PositionDelta));

            //return if trying to move outside map
            if (tile == null)
                return;

            Unit.SetPosion(Unit.GetPosition() + PositionDelta);

        }

        /// <summary>
        /// draw the maps connections as a set of lines for debug purposes
        /// </summary>
        public void debugDrawMap()
        {




        }

        /// <summary>
        /// convert from an integer position on the map to a corresponding vector2 position according to \Peliprojekti3000\Documentation\tiling_diagram.png
        /// </summary>
        /// <param name="mapPos"></param>
        /// <returns></returns>
        public Vector2 MapToWorldPos(Vector2Int mapPos)
        {
            Vector2 right = Vector2.right;
            Vector2 down = Vector2.down + (Vector2.right * 0.5f);

            return (right * (mapPos.x - (mapPos.y / 2))) + (down * mapPos.y);
        }
    }
}
