using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.HueterDesWaldes.WorldEntities
{
    /// <summary>
    /// Interactable that is capable of correcting it's y-position autonomously in order to prevent the user to 'drown' entities with sand.
    /// </summary>
    public class StaticInteractable : Interactable
    {
    [SerializeField]
    private float tick = 1;
    private float staticInteractable_timer = 0;

        // Update is called once per frame
        protected virtual void Update()
        {
            staticInteractable_timer += Time.deltaTime;
            if(staticInteractable_timer > tick)
            {
                Debug.Log("Tick");
                staticInteractable_timer = 0;
                RepositionYValue();
            }
        }

        private void RepositionYValue()
        {
            RaycastHit hit;
            Camera orthoTopCam = Camera.main;
            Ray ray = orthoTopCam.ScreenPointToRay(orthoTopCam.WorldToScreenPoint(transform.position));

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("new pos");
                float newYValue = hit.point.y;
                transform.position = new Vector3(transform.position.x, newYValue, transform.position.z);
            }
            else
                Debug.Log("error: could not hit terrain with raycast");
        }
    }
}
