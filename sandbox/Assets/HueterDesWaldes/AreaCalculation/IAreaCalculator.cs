using System.Collections.Generic;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    public interface IAreaCalculator
    {
        /// <summary>
        /// This Function reinitializes all height information.
        /// May lead to performance issues if called to often.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Use this Function to gather previously calculated contiguous areas on a specific height level.
        /// </summary>
        /// <param name="heightLevelType">Identifier of specific height level.</param>
        /// <returns></returns>
        List<Area> ContiguousAreasOnHeightLevel(HeightLevelType heightLevelType);

        /// <summary>
        /// Use this Function to get all defined height levels.
        /// </summary>
        /// <returns></returns>
        List<HeightLevel> GetHeightLevels();
    }
}