using UnityEngine;

public interface ISpawnArea
{
    void SetSpawnable(MonoBehaviour spawnable);
    int GetCurrentEntitiesCount();

    float GetLowerBorder();

    float GetUpperBorder();

}