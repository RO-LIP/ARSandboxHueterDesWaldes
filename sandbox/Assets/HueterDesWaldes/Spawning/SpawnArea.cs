using Assets.HueterDesWaldes.AreaCalculation;
using Assets.HueterDesWaldes.WorldEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnArea : ISpawnArea
{   
    float treesPerSqm;
    float wellsPerSqm;
    float housesPerSqm;

    [SerializeField]
    float allowedEntitiesPerSquareMeter;
    float areaLevelUpperBorder;
    float areaLevelLowerBorder;
    int currentEntities;
    Area area;
    MonoBehaviour spawnable;
    HeightLevelType levelType;

    [SerializeField]
    GameObject treePrefab;
    [SerializeField]
    GameObject wellPrefab;
    [SerializeField]
    GameObject housePrefab;

    #region Konstruktoren
    public SpawnArea()
    {;}
    public SpawnArea (Area a, float lowerB, float upperB)
    {
        area = a;
        areaLevelLowerBorder = lowerB;
        areaLevelUpperBorder = upperB;
    }
    public SpawnArea(Area a, float lowerB, float upperB, int entities)
    {
        area = a;
        areaLevelLowerBorder = lowerB;
        areaLevelUpperBorder = upperB;
        currentEntities = entities;
    }
    public SpawnArea(Area a, float lowerB, float upperB, int entities, HeightLevelType level, List<float>allowedEntities)
    {
        area = a;
        areaLevelLowerBorder = lowerB;
        areaLevelUpperBorder = upperB;
        currentEntities = entities;
        levelType = level;
        treesPerSqm = allowedEntities.ElementAt(0);
        wellsPerSqm = allowedEntities.ElementAt(1);
        housesPerSqm = allowedEntities.ElementAt(2);
        SetAllowedEntitiesForArea();
    }
    #endregion

    #region  Setter
    public void SetSpawnable(MonoBehaviour spawnable)
    {
        this.spawnable = spawnable;
    }
    public void SetCurrentEntitiesCount(int count)
    {
        currentEntities = count;
    }
    public void SetAllowedEntitiesForArea()
    {          
        if (levelType == HeightLevelType.TREE)
            allowedEntitiesPerSquareMeter = treesPerSqm;
        else if (levelType == HeightLevelType.WATER)
            allowedEntitiesPerSquareMeter = wellsPerSqm;
        else if (levelType == HeightLevelType.VILLAGE)
            allowedEntitiesPerSquareMeter = housesPerSqm;
        else
            allowedEntitiesPerSquareMeter = 0.0f;
    }
    #endregion

    #region Getter
    public int GetCurrentEntitiesCount()
    {
        return currentEntities;
    }
    public Area GetArea()
    {
        return area;
    }
    public float GetLowerBorder()
    {
        throw new System.NotImplementedException();
    }

    public float GetUpperBorder()
    {
        throw new System.NotImplementedException();
    }
    public MonoBehaviour GetSpawnable()
    {
        return spawnable;
    }
    public HeightLevelType GetLevelType()
    {
        return levelType;
    }
    #endregion

    #region Spanwlogik
    public bool SpawnNewEntityAllowed()
    {
        return ((area.SizeInSquareMeters() * allowedEntitiesPerSquareMeter) - currentEntities >= 1.0f) ? true : false;
    }

    public bool DeSpawnEntityRequired()
    {
        return ((area.SizeInSquareMeters() * allowedEntitiesPerSquareMeter) - currentEntities < 0.0f) ? true : false;        
    }
    #endregion

}
