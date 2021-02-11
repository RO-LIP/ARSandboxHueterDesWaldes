using System.Collections.Generic;
using UnityEngine;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    public class Grid
    {
        public class Borders
        {
            public Vector3 bottomLeft;
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 bottomRight;
        }
        public Borders borders { get; private set; }

        public float TileLengthZ { get; private set; }
        public float TileLengthX { get; private set; }

        public int ResolutionX { get; private set; }
        public int ResolutionZ { get; private set; }
        public Tile[,] Tiles { get; private set; }

        private readonly List<HeightLevel> heightLevels;

        public Grid(int ResolutionX, List<HeightLevel> heightLevels, Camera camera)
        {
            this.ResolutionX = ResolutionX;
            this.heightLevels = heightLevels;
            //calculate tiles with aspect ratio as close to 1:1 as possible
            //get aspect ratio of Viewport / Camera 
            //aspect = width / height
            ResolutionZ = Mathf.RoundToInt(ResolutionX / camera.aspect);

            InitializeBordersFromCamera(camera);

            Tiles = new Tile[ResolutionX, ResolutionZ];
        }

        /// <summary>
        /// Sets borders of AreaCalculator equally to borders of the Viewport. The borders are written into the borders parameter.
        /// </summary>
        /// <param name="cam">The camera which is used to set the borders.</param>
        /// <param name="borders"></param>
        private void InitializeBordersFromCamera(Camera cam)
        {
            borders = new Borders
            {
                bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)),
                topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane)),
                topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)),
                bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane))
            };

            //if (debugMode) DebugBorders();
        }

        public void Calculate()
        {
            //calculate tile size in meters from resolution
            TileLengthX = Vector3.Distance(borders.bottomLeft, borders.bottomRight) / ResolutionX;
            TileLengthZ = Vector3.Distance(borders.bottomLeft, borders.topLeft) / ResolutionZ;

            //initialize grid
            Tiles = new Tile[ResolutionX, ResolutionZ];

            //offset to get the middle of a tile
            Vector3 offset = new Vector3(TileLengthX / 2, 0, TileLengthZ / 2);

            //begin bottom left of viewport
            Vector3 iterator = borders.bottomLeft - offset;

            //iterate over rows and within rows iterate over collumns
            for (int z = 0; z < ResolutionZ; z++)
            {
                for (int x = 0; x < ResolutionX; x++)
                {
                    //get projection-point of current grid-tile
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(iterator, -Vector3.up, out RaycastHit hit))
                    {
                        Vector3 pos = hit.point;
                        HeightLevel heightLevel = HeightLevel.GetFrom(heightLevels, pos.y);
                        float size = Mathf.Max(TileLengthX, TileLengthZ);
                        Tile tile = new Tile(pos, x, z, heightLevel, size);
                        //tile.neighbours = CalculateNeighbours(tile, grid);
                        Tiles[x, z] = tile;
                    }
                    else
                    {
                        //null if the ray did not intersect with anything (point lies beyond game world)
                        Tiles[x, z] = null;
                    }
                    //create tile by position data (Vector3) and store it in 2D array
                    iterator.x -= TileLengthX;
                }
                iterator.x = borders.bottomLeft.x - offset.x;
                iterator.z -= TileLengthZ;
            }
            //if (debugMode) DebugGrid();
        }

        public List<Tile> CalculateNeighbours(Tile tile)
        {
            HeightLevelType heightLevelType = tile.HeightLevel.type;
            List<Tile> neighbours = new List<Tile>();
            List<Tile> candidates = new List<Tile>();
            int x = tile.GridPosX;
            int z = tile.GridPosZ;

            //fetch candidates
            //candidates north?
            if (z < ResolutionZ - 1)
                candidates.Add(Tiles[x, z + 1]);
            //candidates east?
            if (x < ResolutionX - 1)
                candidates.Add(Tiles[x + 1, z]);
            //candidates south?
            if (z > 0)
                candidates.Add(Tiles[x, z - 1]);
            //candidates west?
            if (x > 0)
                candidates.Add(Tiles[x - 1, z]);

            //candidate is neighbour?
            foreach (Tile candidate in candidates)
            {
                if (candidate != null && candidate.HeightLevel.type == heightLevelType)
                    neighbours.Add(candidate);
            }

            return neighbours;
        }

        public Tile TileAt(int x, int z)
        {
            return Tiles[x, z];
        }

        public override bool Equals(object other)
        {
            Grid gridB = (Grid)other;
            for (int x = 0; x < ResolutionX; x++)
            {
                for (int z = 0; z < ResolutionZ; z++)
                {
                    Tile tileA = Tiles[x, z];
                    Tile tileB = gridB.Tiles[x, z];

                    if (tileA == null && tileB == null)
                        continue;
                    else if (tileA == null || tileB == null)
                        return false;

                    if (tileA.CompareTo(tileB) != 0)
                        return false;
                }
            }
            return true;
        }
    }
}
