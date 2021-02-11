using Assets.HueterDesWaldes.WorldEntities;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.HueterDesWaldes.AI
{
    public class VillagerMovement : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera cam;
        public ThirdPersonCharacter character;
        private float radius = 0;

        // Start is called before the first frame update
        void Start()
        {
            agent.updateRotation = false; // the character animation will handle the rotation
        }

        // Update is called once per frame
        void Update()
        {
            // this.moveViaMouse();
            if (agent.remainingDistance - radius > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                character.Move(Vector3.zero, false, false);
        }

        public void StopMoving(Villager villager)
        {
            agent.SetDestination(villager.transform.position);
        }

        /**
         * Public method that can be called from other scripts, accepts a target as param and then moves the Villager to that target.
         * Returns something (lol) when the Villager has reached the destination.
         * @param {Interactable} target - the object we want to move to.
         */
        public void MoveToTarget(Interactable target)
        {
            radius = target.GetRadius(); // this is inherited from Interactable -> Well, or any other object, we can access it
            agent.SetDestination(target.transform.position);
        }

        /**
         * This function is used for debugging purposes. It will make the Villager move to the position the developer clicks onto.
         * (Only on the NavMesh), if you want to use it uncomment the method in update.
         */
        private void moveViaMouse()
        {
            if (leftMouseButtonDown() && mouseIsOnScreenOfTargetingCamera())
            {
                RaycastHit hitInfo;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo))
                    agent.SetDestination(hitInfo.point);
            }
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        private bool mouseIsOnScreenOfTargetingCamera()
        {
            return Display.RelativeMouseAt(Input.mousePosition).z == cam.targetDisplay;
        }

        private bool leftMouseButtonDown()
        {
            return Input.GetMouseButtonDown(0);
        }
    }
}