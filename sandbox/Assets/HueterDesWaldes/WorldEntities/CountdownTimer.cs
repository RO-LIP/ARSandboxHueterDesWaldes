using UnityEngine;

namespace Assets.HueterDesWaldes.WorldEntities
{
    public static class CountdownTimer
    {
        [SerializeField]
        private static float countdownInSec = 5f;

        /// <summary>
        /// Get the countdownInSec value for initializing all timer in Update methods.
        /// </summary>
        /// <returns>value of countdownInSec</returns>
        public static float GetCountdownInSec()
        {
            return countdownInSec;
        }
    }
}