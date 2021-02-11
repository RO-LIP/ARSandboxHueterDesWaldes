using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    /// <summary>
    /// This class calculates Contiguous Areas from a grid of Tiles that are connected and lie on the same height level.
    /// ATTENTION: values for horizontal grid resulution above 64 to 128 (depending on hardware) may lead to massive performance issues.
    /// Der Viewport wird in ein Grid aus Tiles (= Kacheln) unterteilt. Die Auflösung des Grids kann dabei im Unity Editor eingestellt werden.
    /// Jedem Tile, dass sich aus dem Grid ergibt, wird ein senkrecht nach unten gerichteter Raycast zugeordnet.
    /// Mit dem Ray wird die Entfernung zwischen Viewport und Terrain gemessen. Jedem Tile wird so ein Höhenniveau zugeordnet.
    /// Areas werden aus Höheninformationen der Tiles gebildet indem Tiles mit ähnlichem Höhenlevel zusammengefasst werden.Ähnlich bedeutet, dass sich das Höhenlevel innerhalb einer oberen und unteren Schranke befindet. Die Höhenlevel-Schranken werden der Klasse von außen übergeben. 
    /// Die Areas werden jetzt noch einmal in weitere Areas aufgespalten.Areas, die nicht über angrenzende Tiles verbunden sind bilden eigenständige Areas.
    /// </summary>
    public class AreaCalculator : MonoBehaviour, IAreaCalculator, IPublisher
    {
        private enum State { UNSTABLE, STABLE }
        private State state;

        [SerializeField]
        private Camera sandboxCamera = null;

        [SerializeField]
        private List<HeightLevel> heightLevels;

        private Grid grid;
        private Grid bufferA;
        private Grid bufferB;
        [SerializeField]
        private int gridResolutionX = 128;
        [SerializeField]
        private int underscanResolution = 96; //horizontal resolution of refresh-scan
        [SerializeField]
        float underscanInterval = 2;
        float timer;

        private Dictionary<HeightLevelType, List<Area>> contiguousAreas;

        private readonly List<ISubscriber> subscribers = new List<ISubscriber>();

        [SerializeField]
        private bool debugMode = false;

        private void Start()
        {
            state = State.UNSTABLE;
            grid = new Grid(gridResolutionX, heightLevels, sandboxCamera);
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= underscanInterval)
            {
                state = RefreshState(state);
                timer = 0;
            }
        }

        private State RefreshState(State state)
        {
            if (TerrainChanged())
            {
                if (state == State.STABLE)
                    Debug.Log("State became UNSTABLE.");
                return State.UNSTABLE;
            }
            else
            {
                if (state == State.UNSTABLE)
                {
                    Debug.Log("State became STABLE. Notify Subscribers.");
                    NotifySubscibers();
                    return State.STABLE;
                }
            }
            return state;
        }

        private bool TerrainChanged()
        {
            //first run
            if (bufferA == null)
            {
                bufferA = new Grid(underscanResolution, heightLevels, sandboxCamera);
                bufferA.Calculate();
                return true;
            }

            bufferB = new Grid(underscanResolution, heightLevels, sandboxCamera);
            bufferB.Calculate();

            if (bufferA.Equals(bufferB))
                return false;
            else
            {
                bufferA = bufferB;
                bufferB = null;
            }
            return true;
        }

        public void Calculate()
        {
            grid.Calculate();
            if (debugMode)
            {
                DebugBorders();
                DebugGrid();
            }
            CalculateContiguousAreas(grid, out contiguousAreas);
        }

        private void CalculateContiguousAreas(Grid grid, out Dictionary<HeightLevelType, List<Area>> contiguousAreas)
        {
            contiguousAreas = new Dictionary<HeightLevelType, List<Area>>();
            HashSet<Tile> explored = new HashSet<Tile>();
            int gridResolutionZ = grid.ResolutionZ;
            //iterate over tiles in grid
            for (int x = 0; x < gridResolutionX; x++)
            {
                for (int z = 0; z < gridResolutionZ; z++)
                {
                    Tile tileInGrid = grid.TileAt(x, z);

                    //tiles can be null after grid calculation
                    if (tileInGrid == null)
                        continue;

                    //if the tile has already been explored, then it already is allocated to a contiguous area
                    //and can be skipped
                    if (explored.Contains(tileInGrid))
                        continue;

                    //define height for new contiguous area from current tile in grid
                    HeightLevelType heightLevelType = tileInGrid.HeightLevel.type;

                    //initialize new contiguous area
                    Area contiguousArea = new Area();

                    //initialize set of detected tiles
                    SortedSet<Tile> detected = new SortedSet<Tile>();
                    //add first tile to detected (current tile in grid)
                    detected.Add(tileInGrid);

                    //as long as detected contains elements get one element
                    while (detected.Count > 0)
                    {
                        Tile detectedTile = detected.Min;

                        //get all neighbours of current detected tile
                        List<Tile> neighbours = grid.CalculateNeighbours(detectedTile);

                        //for all neighbours check if they already have been detected or explored
                        while (neighbours.Count > 0)
                        {
                            Tile neighbour = neighbours[0];
                            if (!detected.Contains(neighbour) && !explored.Contains(neighbour))
                            {
                                detected.Add(neighbour);
                            }
                            neighbours.RemoveAt(0);
                        }

                        //if all neighbours are detected or explored, then mark current detected tile as explored
                        explored.Add(detectedTile);
                        //remove explored tile from detected set
                        detected.Remove(detectedTile);
                        //allocate current detected tile to current contiguous area
                        contiguousArea.Add(detectedTile);
                    }
                    //all tiles in contiguous area have been added
                    //add contiguous area to set of contiguous areas
                    //check if set has already a slot for height level of current contiguous area
                    List<Area> contiguousAreasOnLevel;
                    contiguousAreas.TryGetValue(heightLevelType, out contiguousAreasOnLevel);
                    if (contiguousAreasOnLevel == null)
                    {
                        //create new slot for height level of current contiguous area
                        contiguousAreasOnLevel = new List<Area>();
                        contiguousAreas.Add(heightLevelType, contiguousAreasOnLevel);
                    }
                    contiguousAreasOnLevel.Add(contiguousArea);
                }
            }
            if (debugMode) DebugContiguousAreas();
        }

        public List<Area> ContiguousAreasOnHeightLevel(HeightLevelType heightLevelType)
        {
            contiguousAreas.TryGetValue(heightLevelType, out List<Area> contiguousAreasOnHeightLevel);
            return contiguousAreasOnHeightLevel;
        }

        public List<HeightLevel> GetHeightLevels()
        {
            return heightLevels;
        }

        private void DebugBorders()
        {
            //disable visual debugging in stopped editor mode
            if (!(EditorApplication.isPlaying))
                return;

            DebugVisualizer debugVisualizer = DebugVisualizer.GetInstance(transform, "Borders");
            Grid.Borders borders = grid.borders;

            //draw borders
            List<Vector3> lines = new List<Vector3>();
            lines.Add(borders.bottomLeft);
            lines.Add(borders.topLeft);
            lines.Add(borders.topRight);
            lines.Add(borders.bottomRight);

            debugVisualizer.VisualDebuggingLines(lines);
        }

        private void DebugGrid()
        {
            //disable visual debugging in stopped editor mode
            if (!(EditorApplication.isPlaying))
                return;

            DebugVisualizer debugVisualizer = DebugVisualizer.GetInstance(transform, "Grid");

            //draw grid
            Tile[,] tiles = grid.Tiles;
            List<Vector3> gridV = new List<Vector3>();
            foreach (Tile tile in tiles)
            {
                if (tile != null)
                    gridV.Add(tile.WorldPos);
            }
            debugVisualizer.VisualDebuggingTiles(gridV, grid.TileLengthX, grid.TileLengthZ);
        }

        private void DebugContiguousAreas()
        {
            //disable visual debugging in stopped editor mode
            if (!(EditorApplication.isPlaying))
                return;

            foreach (HeightLevel heightLevel in heightLevels)
            {
                HeightLevelType heightLevelType = heightLevel.type;
                List<Area> heighGrid = ContiguousAreasOnHeightLevel(heightLevelType);

                if (heighGrid == null)
                    continue;

                int j = 0;
                foreach (Area contiguousArea in heighGrid)
                {
                    //draw contiguous areas
                    string innerName = heightLevelType.ToString() + j++;
                    DebugVisualizer contiguousAreaDV = DebugVisualizer.GetInstance(transform, innerName);
                    List<Vector3> contiguousAreaTiles = contiguousArea.GetTiles();
                    contiguousAreaDV.VisualDebuggingTiles(contiguousAreaTiles, grid.TileLengthX, grid.TileLengthZ);
                }
            }
        }

        public void Attach(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void Detach(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void NotifySubscibers()
        {
            foreach (ISubscriber subscriber in subscribers)
                subscriber.Notify();
        }
    }
}
