using Assets.HueterDesWaldes.WorldEntities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.HueterDesWaldes.AI
{
    public class VillagerAI : MonoBehaviour
    {
        /// <summary>
        /// If VillagerState is not THINKING return null and do nothing.
        /// Decide where to go if VillagerState is THINKING.
        /// Check villagers bucket and set villagerState:
        ///   empty bucket      -> move to well
        ///   full bucket       -> move to nearest tree with least amount of water
        /// </summary>
        /// <param name="villager">villager for decision making</param>
        /// <returns>target the villager moves to</returns>
        public Interactable DecideWhereToGoAndMove(Villager villager)
        {
            if (villager.GetVillagerState() != VillagerState.THINKING)
                return null;

            if (!villager.GetHaswaterInBucket())
            {
                Interactable well = FindNearestInteractableOfType(InteractableType.WELL);
                if (well == null)
                    return null;
                villager.MoveToTarget(well);
                villager.SetVillagerState(VillagerState.GOTOWELL);
                return well;
            }
            else
            {
                Interactable tree = FindNearestTreeWithLeastAmountOfWater();
                if (tree == null)
                    return null;
                villager.MoveToTarget(tree);
                villager.SetVillagerState(VillagerState.GOTOTREE);
                return tree;
            }
        }

        /// <summary>
        /// Find nearest Interactable Object like wells or trees.
        /// Searches in dynamically generated list of GameObjects from spawner.
        /// Checks InteractableType and distance to villager.
        /// </summary>
        /// <param name="interactableType">InteractableType to filter list of possible GameObjects</param>
        /// <returns>nearest interactable obeject of given type</returns>
        public Interactable FindNearestInteractableOfType(InteractableType interactableType)
        {
            List<GameObject> possibleTargets = new List<GameObject>();

            if (interactableType == InteractableType.TREE)
                possibleTargets = FindObjectOfType<Spawner>()
                .GetEntitiesPosition()
                .Values
                .Where(e => e.GetComponent<WorldEntities.Tree>() != null && e.GetComponent<WorldEntities.Tree>().GetInteractableType() == InteractableType.TREE)
                .ToList();

            else if (interactableType == InteractableType.SPROUT)
                possibleTargets = FindObjectOfType<Spawner>()
                .GetEntitiesPosition()
                .Values
                .Where(e => e.GetComponent<WorldEntities.Tree>() != null && e.GetComponent<WorldEntities.Tree>().GetInteractableType() == InteractableType.SPROUT)
                .ToList();

            else if (interactableType == InteractableType.WELL)
                possibleTargets = FindObjectOfType<Spawner>()
                .GetEntitiesPosition()
                .Values
                .Where(e => e.GetComponent<Well>() != null)
                .ToList();

            else
                return null;

            if (possibleTargets.Count() == 0)
                return null;

            // Find nearest object (https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/#post-3129260) as Interactable
            Interactable nearestInteractable = possibleTargets
                .OrderBy(t => (t.transform.position - this.transform.position).sqrMagnitude)
                .FirstOrDefault()
                .GetComponent<Interactable>();

            return nearestInteractable;
        }

        /// <summary>
        /// Find nearest Interactable trees.
        /// Searches in dynamically generated list of GameObjects from spawner.
        /// Checks the amount of water and distance to villager and returns the nearest with least water.
        /// </summary>
        /// <returns></returns>
        public Interactable FindNearestTreeWithLeastAmountOfWater()
        {
            List<GameObject> possibleTargets = new List<GameObject>();

            possibleTargets = FindObjectOfType<Spawner>()
                .GetEntitiesPosition()
                .Values
                .Where(e => e.GetComponent<WorldEntities.Tree>() != null)
                .ToList();

            if (possibleTargets.Count() == 0)
                return null;

            Interactable treeWithLeastAmountOfWater = possibleTargets
                .OrderBy(t => t.GetComponent<WorldEntities.Tree>().GetAmountWater())
                .ThenBy(t => (t.transform.position - this.transform.position).sqrMagnitude)
                .FirstOrDefault()
                .GetComponent<Interactable>();

            return treeWithLeastAmountOfWater;
        }

        /// <summary>
        /// Ability to interact with a given target.
        /// Check if villager is in the radius of the target.
        /// Set the villagerState to THINKING to limit the interaction to one.
        /// </summary>
        /// <param name="villager">villager who interacts</param>
        /// <param name="target">target to interact with</param>
        public void InteractWithTarget(Villager villager, Interactable target)
        {
            if (IsInTargetRadius(villager, target))
            {
                target.Interact(villager);
                villager.SetVillagerState(VillagerState.THINKING);
            }
        }

        /// <summary>
        /// Check if villager is in target radius for interacting.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true if in radius</returns>
        public bool IsInTargetRadius(Villager villager, Interactable target)
        {
            float distance = Vector3.Distance(target.transform.position, villager.transform.position);
            if (distance <= target.GetRadius())
                return true;
            return false;
        }
    }
}
