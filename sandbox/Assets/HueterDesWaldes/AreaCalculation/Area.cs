using System.Collections.Generic;
using UnityEngine;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    public class Area
    {
        public SortedSet<Tile> tiles;
        //Todo delete parameter
        private float tileSizeInSquareMeters;

        public Area()
        {
            tiles = new SortedSet<Tile>();
        }

        public List<Vector3> GetTiles()
        {
            List<Vector3> tilesListVector = new List<Vector3>();
            foreach (Tile tile in tiles)
                tilesListVector.Add(tile.WorldPos);
            return tilesListVector;
        }

        public float SizeInSquareMeters()
        {
            tileSizeInSquareMeters = Mathf.Pow(tiles.Min.Size, 2);
            float areaSizeInSquareMeters = tiles.Count * tileSizeInSquareMeters;
            return areaSizeInSquareMeters;
        }

        //public float tileSizeInSquareMeters { get => tileSizeInSquareMeters; set => tileSizeInSquareMeters = value; }
        public SortedSet<Tile> GetAllTiles()
        {
            return tiles;
        }

        /// <summary>
        /// Area does not support duplicate / multiple equal tiles. Check Tile.CompareTo() documentation for how equality is defined.
        /// </summary>
        /// <param name="tile"></param>
        public void Add(Tile tile)
        {
            tiles.Add(tile);
        }

        public bool Contains(Tile tile)
        {
            return tiles.Contains(tile);
        }
    }
}
