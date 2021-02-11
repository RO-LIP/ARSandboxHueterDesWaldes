using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandMovement : MonoBehaviour
{
    private int currentHandIndex = 0;
    public List<GameObject> hands = null;
    public GameObject upperRange = null;
    public GameObject lowerRange = null;
    public float waitForMin = 3;
    public float waitForMax = 5;
    float waitFor = 5;
    float waitForTimer = 0;
    bool move = false;
    Transform dest = null;
    public float speedChangeInterval = 2;
    public float speedChangeTimer = 5;
    public float stoppingDistance = 1;
    public float noise = 3;
    public float speedMin = -100;
    public float speedMax = 200;
    float speed = -50;

    // Start is called before the first frame update
    void Start()
    {
        dest = upperRange.transform;
        waitForTimer = waitFor;
        ChangeHand();
    }

    // Update is called once per frame
    void Update()
    {
        speedChangeTimer -= Time.deltaTime;
        if (speedChangeTimer <= 0)
        {
            speedChangeTimer = speedChangeInterval;
            speed = Random.Range(speedMin, speedMax);
            if (speed < 0) speed = noise;
        }

        if (!move)
        {
            waitForTimer -= Time.deltaTime;
            if (waitForTimer <= 0)
            {
                float waitFor = Random.Range(waitForMin, waitForMax);
                waitForTimer = waitFor;
                move = !move;
            }
        }

        if (Vector3.Distance(transform.position, dest.position) <= stoppingDistance)
        {
            if (dest == upperRange.transform)
                dest = lowerRange.transform;
            else
                dest = upperRange.transform;
            move = false;
            ChangeHand();
        }

        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest.position, Time.deltaTime * speed);
        }
    }

    private void ChangeHand()
    {
        currentHandIndex = (currentHandIndex + 1) % hands.Count;
        for (int i = 0; i < hands.Count; i++)
        {
            if (i == currentHandIndex)
                hands[i].SetActive(true);
            else
                hands[i].SetActive(false);
        }
    }
}
