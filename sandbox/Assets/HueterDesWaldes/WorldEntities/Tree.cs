using UnityEngine;

namespace Assets.HueterDesWaldes.WorldEntities
{
    public class Tree : StaticInteractable, IAirQualityParameter, ICountdownTimer
    {
        [SerializeField]
        private float timer;

        [SerializeField]
        private float amountWater;

        [SerializeField]
        private float threshholdWaterForGrowth;

        [SerializeField]
        private float growthMaxScale;

        [SerializeField]
        private float airQualityImpactAsSprout;

        [SerializeField]
        private float airQualityImpactAsTree;

        [SerializeField]
        private float airQualityImpactCosts;

        /// <summary>
        /// Start timer with central CountdownTimer value.
        /// </summary>
        void Start()
        {
            ResetTimer(CountdownTimer.GetCountdownInSec());
            SetInteractableType(InteractableType.SPROUT);
        }

        /// <summary>
        /// Interacting with the tree.
        /// Check if villager has water in the bucket before watering the tree.
        /// Set HasWaterInBucket to false.
        /// </summary>
        /// <param name="villager">villager who is interacting</param>
        public override void Interact(Villager villager)
        {
            base.Interact(villager);
            if (villager.GetHaswaterInBucket())
                Water(villager.GetBucketCapacity());
            villager.SetHasWaterInBucket(false);
        }

        /// <summary>
        /// Update is called once per frame.
        /// Check timer and change airQuality. Reset timer afterwards.
        /// Only affects airQuality if tree has water and is able to spend the costs.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (GetInteractableType() == InteractableType.SPROUT && amountWater >= 1)
                    ChangeAirQuality(airQualityImpactAsSprout);
                else if (GetInteractableType() == InteractableType.TREE && amountWater >= 1)
                    ChangeAirQuality(airQualityImpactAsTree);

                ResetTimer(CountdownTimer.GetCountdownInSec());
            }

            ChangeScaleOfTree();
            DistinguishSproutOrTree();
        }

        /// <summary>
        /// Changes the overall AirQuality by a given value.
        /// Changes the amount of water.
        /// </summary>
        /// <param name="value">value to cahnge the overall airQuality</param>
        public void ChangeAirQuality(float value)
        {
            amountWater -= airQualityImpactCosts;
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

        /// <summary>
        /// Watering the tree or sprout with water fills the amount of water.
        /// </summary>
        /// <param name="water">amount of water</param>
        public void Water(float water)
        {
            amountWater += water;
        }

        /// <summary>
        /// Changes the scale of the tree according to the amount of water. Minimum is 1, maximum is growthMaxScale.
        /// </summary>
        private void ChangeScaleOfTree()
        {
            float scaleChange = GetAmountWater();
            if (scaleChange >= 1 && scaleChange <= growthMaxScale)
                transform.localScale = new Vector3(scaleChange, scaleChange, scaleChange);
        }

        /// <summary>
        /// Distinguishes if it is a TREE or SPROUT according to the amount of water and the threshholdWaterForGrowth.
        /// </summary>
        private void DistinguishSproutOrTree()
        {
            if (amountWater < threshholdWaterForGrowth)
                SetInteractableType(InteractableType.SPROUT);
            else
                SetInteractableType(InteractableType.TREE);
        }

        /// <summary>
        /// Set the amount of water.
        /// </summary>
        /// <param name="amountWater">amount of water</param>
        public void SetAmountWater(float amountWater)
        {
            this.amountWater = amountWater;
        }

        /// <summary>
        /// Get the current amount of water.
        /// </summary>
        /// <returns>amount of water</returns>
        public float GetAmountWater()
        {
            return amountWater;
        }

        /// <summary>
        /// Set the threshhold of water for the tree to grow.
        /// </summary>
        /// <param name="threshholdWaterForGrowth">threshhold of water for the tree to grow</param>
        public void SetThreshholdWaterForGrowth(float threshholdWaterForGrowth)
        {
            this.threshholdWaterForGrowth = threshholdWaterForGrowth;
        }

        /// <summary>
        /// Get the threshhold of water for the tree to grow.
        /// </summary>
        /// <returns>threshhold of water for the tree to grow</returns>
        public float GetThreshholdWaterForGrowth()
        {
            return threshholdWaterForGrowth;
        }
    }
}