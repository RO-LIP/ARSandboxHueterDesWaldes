using UnityEngine;

namespace Assets.HueterDesWaldes.WorldEntities
{
    /// <summary>
    /// Inheriting Objects get types for being targeted by villagers.
    /// </summary>
    public enum InteractableType { WELL, TREE, SPROUT, UNDEFINED }

    public class Interactable : MonoBehaviour
    {
        [SerializeField]
        private float radius = 15f;

        [SerializeField]
        private InteractableType interactableType = InteractableType.UNDEFINED;

        /// <summary>
        /// Interact method to be overwritten and called from children.
        /// </summary>
        /// <param name="actor">actor who interacts with the Interactable</param>
        public virtual void Interact(Villager actor)
        {
            
        }

        /// <summary>
        /// Enables the object to get a visible wireframe for the interaction radius in the editor.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        /// <summary>
        /// Set the radius for interacting with the object.
        /// </summary>
        /// <param name="radius">radius for interacting</param>
        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Get the radius for interacting with the object.
        /// </summary>
        /// <returns>radius for interacting</returns>
        public float GetRadius()
        {
            return radius;
        }

        /// <summary>
        /// Set the type of the interactable object.
        /// </summary>
        /// <param name="interactableType">type of the interactable object</param>
        public void SetInteractableType(InteractableType interactableType)
        {
            this.interactableType = interactableType;
        }

        /// <summary>
        /// Get the type of the interactable object.
        /// </summary>
        /// <returns>type of the interactable object</returns>
        public InteractableType GetInteractableType()
        {
            return interactableType;
        }
    }
}