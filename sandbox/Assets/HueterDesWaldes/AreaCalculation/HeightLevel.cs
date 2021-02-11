using System;
using System.Collections.Generic;

public enum HeightLevelType { UNDEFINED, WATER, VILLAGE, TREE, MOUNTAIN }

[Serializable]
public class HeightLevel
{
    public HeightLevelType type = HeightLevelType.UNDEFINED;
    public float lowerBorder = 0;
    public float upperBorder = 0;

    internal static HeightLevel GetFrom(List<HeightLevel> heights, float y)
    {
        HeightLevel height = new HeightLevel();

        foreach (HeightLevel current in heights)
        {
            float upperBorder = current.upperBorder;
            float lowerBorder = current.lowerBorder;

            //lower border is inclusive, higher border is exclusive
            //heightLevel.Type is defined like [lowerBorder; upperBorder[
            if (y >= lowerBorder && y < upperBorder)
            {
                height = current;
            }
        }
        return height;
    }
}
