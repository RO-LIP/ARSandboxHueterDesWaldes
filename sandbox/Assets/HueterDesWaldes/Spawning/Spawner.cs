using Assets.HueterDesWaldes.AreaCalculation;
using Assets.HueterDesWaldes.WorldEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour, ISubscriber
{
    Dictionary<Tile, GameObject> entitiesPosition = new Dictionary<Tile, GameObject>();
    Dictionary<Tile, HeightLevelType> entitiesHeightLevel = new Dictionary<Tile, HeightLevelType>();
    AreaCalculator areaCalculator;
    List<SpawnArea> spawnAreas;
    List<float> entitiesAllowed = new List<float>();

    [SerializeField]
    GameObject treePrefab;
    [SerializeField]
    GameObject wellPrefab;
    [SerializeField]
    GameObject housePrefab;
    [SerializeField]
    float treesPerSquareMeter = 0.0003f;
    [SerializeField]
    float wellsPerSquareMeter = 0.0003f;
    [SerializeField]
    float housesPerSquareMeter = 0.0003f;

    // Start is called before the first frame update
    void Start()
    {
        areaCalculator = FindObjectOfType<AreaCalculator>(); 
        areaCalculator.Attach(this);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    #region Setup new SpawnAreas
    private void CalculateSpawnAreas()
    {
        spawnAreas = new List<SpawnArea>();

        Dictionary<Tile, GameObject> oldEntitiesPosition = this.entitiesPosition;
        Dictionary<Tile, HeightLevelType> oldEntitiesHeightLevel = this.entitiesHeightLevel;
        this.entitiesPosition = new Dictionary<Tile, GameObject>();
        this.entitiesHeightLevel = new Dictionary<Tile, HeightLevelType>();
        areaCalculator.Calculate();

        foreach (HeightLevelType levelType in Enum.GetValues(typeof(HeightLevelType)))
        {           
            //alle Flächen auf einem Höhenlevel bestimmen          
            List<Area> areas = areaCalculator.ContiguousAreasOnHeightLevel(levelType);           
            //aus den Flächen die SpawnAreas generieren
            if(areas != null && levelType != HeightLevelType.UNDEFINED)
            {
                DetermineCorrectlyLocatedEntities(areas, levelType, oldEntitiesPosition, oldEntitiesHeightLevel);
                IEnumerable<SpawnArea> spawnAreasOnHeightLevel = SetupSpawnAreasForHeightLevel(areas, levelType);
                spawnAreas.AddRange(spawnAreasOnHeightLevel);
            }
        }

        DestroyOrphanGameObjects(oldEntitiesPosition);
    }

    private void DetermineCorrectlyLocatedEntities(List<Area> areas, HeightLevelType levelType, Dictionary<Tile, GameObject> entities, Dictionary<Tile, HeightLevelType> heightEntities)
    {
        List<Tile> oldEntityTiles = entities.Keys.ToList();
        List<Tile> allTilesOnHeightLevel = new List<Tile>();

        foreach(Area area in areas)
        {
            allTilesOnHeightLevel.AddRange(area.GetAllTiles().ToList());

        }

        foreach(Tile newTile in allTilesOnHeightLevel)
        {
            foreach(Tile oldTile in oldEntityTiles)
            {
                heightEntities.TryGetValue(oldTile, out HeightLevelType heightTile);
                if (newTile.GridPosX == oldTile.GridPosX && newTile.GridPosZ == oldTile.GridPosZ && heightTile == levelType)
                {
                    entities.TryGetValue(oldTile, out GameObject entity);
                    entitiesPosition.Add(newTile, entity);
                    entitiesHeightLevel.Add(newTile, levelType);
                    entities.Remove(oldTile);
                }
            }
        }
    }

    private void DestroyOrphanGameObjects(Dictionary<Tile, GameObject> oldEntitiesPosition)
    {
        List<GameObject> oldEntities = oldEntitiesPosition.Values.ToList();
        
        foreach (GameObject entity in oldEntities){
            Destroy(entity);
        }
    }
    #endregion

    #region Hilfsmethoden Setup SpawnArea
   
    private IEnumerable<SpawnArea> SetupSpawnAreasForHeightLevel (List<Area> areas, HeightLevelType levelType)
    {
        List<SpawnArea> spawnAreas = new List<SpawnArea>();
        entitiesAllowed.Clear();
        entitiesAllowed.Add(treesPerSquareMeter);
        entitiesAllowed.Add(wellsPerSquareMeter);
        entitiesAllowed.Add(housesPerSquareMeter);

        //obere und untere Grenze des Höhenleveltyps bestimmen
        float[] borders = GetHeightLevelBorders(levelType);

        foreach (Area area in areas)
        {           
            //int entityCount = CountEntitiesInArea(area);
            int entityCount = area.GetAllTiles().ToList().Where(t => entitiesPosition.ContainsKey(t)).Count();
            SpawnArea s = new SpawnArea(area, borders[0], borders[1], entityCount, levelType, entitiesAllowed);
            //SetSpawnableInSpawnArea(s, levelType);
            spawnAreas.Add(s);
        }
        return spawnAreas;
    }

    private float[] GetHeightLevelBorders (HeightLevelType levelType)
    {
        float[] borders = new float[2];

        if(levelType != HeightLevelType.UNDEFINED)
        {
            List<HeightLevel> levels = areaCalculator.GetHeightLevels();
            borders[0] = levels.Where(a => a.type == levelType).FirstOrDefault().lowerBorder;
            borders[1] = levels.Where(a => a.type == levelType).FirstOrDefault().upperBorder;
        }
        return borders;
    }
    #endregion

    #region Spawning und DeSpawning für komplette Welt
    private void SpawnEntity(SpawnArea spawnArea)
    {      
        List<Tile> tilesWithEntities = entitiesPosition.Keys.ToList();
        List<Tile> tiles = spawnArea.GetArea().GetAllTiles().ToList();
        IEnumerable<Tile> spawnableTiles = new List<Tile>();

        spawnableTiles = tiles.Where(t => !tilesWithEntities.Contains(t)).ToList();
        Tile spawningTile = spawnableTiles.ElementAt(UnityEngine.Random.Range(0, tiles.Count()));
     
        GameObject newEntity = Spawn(spawningTile.GetWorldPos(), spawnArea.GetLevelType());

        //neue Entity in die Dictionaries aufnehmen
        entitiesPosition.Add(spawningTile, newEntity);
        entitiesHeightLevel.Add(spawningTile, spawnArea.GetLevelType());
        //Anzahl Entities in spawnArea hochzählen; sonst Endlosschleife       
        spawnArea.SetCurrentEntitiesCount(spawnArea.GetCurrentEntitiesCount() + 1);
    }

    private void DeSpawnEntity(SpawnArea spawnArea)
    {
        System.Random rnd = new System.Random();
        List<Tile> tilesWithEntities = entitiesPosition.Keys.ToList();
        List<Tile> tiles = spawnArea.GetArea().GetAllTiles().ToList();
        IEnumerable<Tile> despawnableTiles = new List<Tile>();

        despawnableTiles = tiles.Where(t => tilesWithEntities.Contains(t)).ToList();
        Tile despawningTile = despawnableTiles.ElementAt(UnityEngine.Random.Range(0, despawnableTiles.Count()));

        GameObject despawningEntity =  entitiesPosition.Where(a => a.Key == despawningTile).FirstOrDefault().Value;

        //Eintrag aus den Dictionaries entfernen
        entitiesPosition.Remove(despawningTile);
        entitiesHeightLevel.Remove(despawningTile);
        //eigentliches GameObject zerstören
        Destroy(despawningEntity);
        //Anzahl entities in spawnArea runterzählen; sonst Endlosschleife       
        spawnArea.SetCurrentEntitiesCount(spawnArea.GetCurrentEntitiesCount() - 1);
    }

    public GameObject Spawn(Vector3 position, HeightLevelType levelType)
    {
        GameObject prefab = null;

        if (levelType == HeightLevelType.VILLAGE)
            prefab = housePrefab;
        else if (levelType == HeightLevelType.TREE)
            prefab = treePrefab;
        else if (levelType == HeightLevelType.WATER)
            prefab = wellPrefab;

        GameObject newEntity = GameObject.Instantiate(prefab, position, Quaternion.identity, transform);
        
        newEntity.transform.rotation = prefab.transform.rotation;
        if (prefab == housePrefab)
            newEntity.transform.Rotate(0, UnityEngine.Random.Range(-25, 25), 0);

        return newEntity;
    }

    private void UpdateEntitiesForAllAreas()
    {
        //spawn despawn new entities
        foreach (SpawnArea spawnArea in spawnAreas)
        {
            while (spawnArea.SpawnNewEntityAllowed())
                SpawnEntity(spawnArea);
            while (spawnArea.DeSpawnEntityRequired())
                DeSpawnEntity(spawnArea);
        }
    }

    public void Notify()
    {
        CalculateSpawnAreas();
        UpdateEntitiesForAllAreas();
        Debug.Log("Notified Spawner");
    }
    #endregion

    public Dictionary<Tile, GameObject> GetEntitiesPosition()
    {
        return entitiesPosition;
    }
}
