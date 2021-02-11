using System.Collections.Generic;
using UnityEngine;

namespace Assets.HueterDesWaldes.WorldEntities
{
    public class House : StaticInteractable, IAirQualityParameter, ICountdownTimer
    {
        [SerializeField]
        private float timer;

        [SerializeField]
        private float airQualityImpact;

        [SerializeField]
        private int numberVillagers = 3;

        [SerializeField]
        GameObject villagerPrefab;

        private List<GameObject> villagers = new List<GameObject>();

        /// <summary>
        /// Start timer with central CountdownTimer value.
        /// Spawns villagers of the house.
        /// </summary>
        void Start()
        {
            ResetTimer(CountdownTimer.GetCountdownInSec());

            SpawnVillagers();
        }

        /// <summary>
        /// Check timer and change airQuality. Reset timer afterwards.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeAirQuality(airQualityImpact);
                ResetTimer(CountdownTimer.GetCountdownInSec());
            }
        }

        /// <summary>
        /// Spawns villagers at the house.
        /// </summary>
        private void SpawnVillagers()
        {
            for (int i = 0; i < numberVillagers; i++)
            {
                GameObject newVillager = Instantiate(villagerPrefab, transform, true);
                newVillager.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                villagers.Add(newVillager);
            }
        }

        /// <summary>
        /// Despawns all villagers from the house by destroying their gameobjects.
        /// </summary>
        private void DespawnVillagers()
        {
            villagers.ForEach(villager => Destroy(villager));
        }

        /// <summary>
        /// Changes the overall AirQuality by a given value.
        /// </summary>
        /// <param name="value">value to cahnge the overall airQuality</param>
        public void ChangeAirQuality(float value)
        {
            AirQuality.airQuality += value;
        }

        /// <summary>
        /// Resets the timer to the given countdownInSec.
        /// </summary>
        /// <param name="countdownInSec">countdownInSec</param>
        public void ResetTimer(float countdownInSec)
        {
            timer = countdownInSec;
        }
    }
}