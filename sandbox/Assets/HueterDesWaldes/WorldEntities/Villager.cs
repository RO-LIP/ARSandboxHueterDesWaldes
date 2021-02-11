using Assets.HueterDesWaldes.AI;
using UnityEngine;

namespace Assets.HueterDesWaldes.WorldEntities
{
    /// <summary>
    /// Villager States for decision making.
    /// </summary>
    public enum VillagerState { THINKING, GOTOWELL, GOTOTREE }

    public class Villager : MonoBehaviour
    {
        [SerializeField]
        private string villagerName;

        [SerializeField]
        private VillagerState villagerState;

        [SerializeField]
        private Interactable focusedTarget;

        [SerializeField]
        private bool hasWaterInBucket;

        [SerializeField]
        private float bucketCapacity;

        private VillagerAI villagerAI;
        private VillagerMovement villagerMovement;

        /// <summary>
        /// Initialize the Villager with the ability to move to given targets and the villagerAI.
        /// Set his state to THINKING for further decision making.
        /// </summary>
        void Start()
        {
            villagerName = RandomVillagerNamer.GetRandomName();
            villagerAI = GetComponent<VillagerAI>();
            villagerMovement = GetComponent<VillagerMovement>();

            villagerState = VillagerState.THINKING;
        }

        /// <summary>
        /// Update is called once per frame.
        /// Call AI method for deciding where to go and start moving.
        /// Interact with target if near enough.
        /// Reset VillagerState to THINKING on errors to restart the decision making.
        /// </summary>
        void Update()
        {
            try
            {
                focusedTarget = villagerAI.DecideWhereToGoAndMove(this);
                if (villagerState != VillagerState.THINKING)
                    villagerAI.InteractWithTarget(this, focusedTarget);
            }
            catch (System.Exception ex)
            {
                //Debug.Log($"Villager had an exception: {ex.Message}");
                //villagerMovement.StopMoving(this);
                villagerState = VillagerState.THINKING;
            }
        }

        /// <summary>
        /// Move villager to given target.
        /// </summary>
        /// <param name="target">target to move to</param>
        public void MoveToTarget(Interactable target)
        {
            villagerMovement.MoveToTarget(target);
        }

        /// <summary>
        /// Set the VillagerState to a given state.
        /// </summary>
        /// <param name="villagerState">VillagerState</param>
        public void SetVillagerState(VillagerState villagerState)
        {
            this.villagerState = villagerState;
        }

        /// <summary>
        /// Get the VillagerState.
        /// </summary>
        /// <returns>VillagerState</returns>
        public VillagerState GetVillagerState()
        {
            return villagerState;
        }

        /// <summary>
        /// Set the flag if villager has water in the bucket.
        /// </summary>
        /// <param name="hasWaterInBucket">flag if villager has water in the bucket</param>
        public void SetHasWaterInBucket(bool hasWaterInBucket)
        {
            this.hasWaterInBucket = hasWaterInBucket;
        }

        /// <summary>
        /// Get flag if villager has water in the bucket.
        /// </summary>
        /// <returns>flag if villager has water in the bucket</returns>
        public bool GetHaswaterInBucket()
        {
            return hasWaterInBucket;
        }

        /// <summary>
        /// Get the focused target.
        /// </summary>
        /// <returns>the focused target</returns>
        public Interactable GetFocusedTarget()
        {
            return focusedTarget;
        }

        /// <summary>
        /// Set the capacity of the bucket.
        /// </summary>
        /// <param name="bucketCapacity">the capacity of the bucket</param>
        public void SetBucketCapacity(float bucketCapacity)
        {
            this.bucketCapacity = bucketCapacity;
        }

        /// <summary>
        /// Get the capacity of the bucket.
        /// </summary>
        /// <returns>the capacity of the bucket</returns>
        public float GetBucketCapacity()
        {
            return bucketCapacity;
        }
    }
}