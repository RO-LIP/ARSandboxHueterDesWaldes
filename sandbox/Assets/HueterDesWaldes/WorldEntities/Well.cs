namespace Assets.HueterDesWaldes.WorldEntities
{
    public class Well : StaticInteractable
    {
        /// <summary>
        /// Initializes the Well with the propper InteractableType.
        /// </summary>
        public void Start()
        {
            SetInteractableType(InteractableType.WELL);
        }

        /// <summary>
        /// Interacting with the well gives the interacting villager water as flag hasWaterInBucket.
        /// </summary>
        /// <param name="villager">villager who is interacting</param>
        public override void Interact(Villager villager)
        {
            base.Interact(villager);
            villager.SetHasWaterInBucket(true);
        }
    }
}