using System;
using UnityEngine;

namespace Assets.HueterDesWaldes.AreaCalculation
{
/// <summary>
/// A Tile is the most atomic element within the AreaCalculation process.
/// A Tile represents a specific world position.
/// It has a specific (nearly) square size and specific coordinates in the grid.
/// </summary>
    public class Tile : IComparable
    {
        public int GridPosX { get; private set; }
        public int GridPosZ { get; private set; }
        public Vector3 WorldPos { get; private set; }
        public float Size { get; private set; }
        public HeightLevel HeightLevel { get; private set; }

        public Tile(Vector3 worldPos, int gridPosX, int gridPosZ, HeightLevel height, float size)
        {
            WorldPos = worldPos;
            GridPosX = gridPosX;
            GridPosZ = gridPosZ;
            HeightLevel = height;
            Size = size;
        }

        public Vector3 GetWorldPos()
        {
            return WorldPos;
        }

        /// <summary>
        /// Compare two Vector3 objects. Returns:
        /// -1 if a is smaller than b
        ///  0 if a has the same position as b
        ///  1 if a is bigger than b
        /// 
        /// a Vector is "smaller" than another Vector if it's x value is smaller
        /// if the x values are the same then
        /// a Vector is "smaller" than another Vector if it's z value is smaller
        /// </summary>
        public int CompareTo(object obj)
        {
            Tile tileA = this;
            Tile tileB = (Tile)obj;
            Vector3 posA = tileA.WorldPos;
            Vector3 posB = tileB.WorldPos;

            //checking if two float values are equal may lead to undefined results
            //if two tiles are not within the same row or collumn they should be exactly one size apart from eachother
            //if the tiles don't have the same size (this should be avoided) they should be at least the half of their middled sizes away from each other
            //so we check if the tiles are more or less than half their middled sizes away from eachother
            float middledSize = (tileA.Size + tileB.Size) / 2;
            float threshold = middledSize / 2;

            if (Mathf.Abs(posA.x - posB.x) < threshold)
            {
                if (Mathf.Abs(posA.z - posB.z) < threshold)
                {

                    if (Mathf.Abs(posA.y - posB.y) < threshold)
                        return 0;

                    //tiles differ on y-axis
                    if (posA.y < posB.y)
                        return -1;
                    else if (posA.y > posB.y)
                        return 1;
                }
                //tiles differ on z-axis
                if (posA.z < posB.z)
                    return -1;
                else if (posA.z > posB.z)
                    return 1;
            }

            //tiles differ on x-axis
            if (posA.x < posB.x)
                return -1;
            else if (posA.x > posB.x)
                return 1;

            throw new Exception("Could not compare position of tiles.");
        }
    }
}
