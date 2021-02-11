using System;
using UnityEngine;
using Assets.HueterDesWaldes.AreaCalculation;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Spawner spawner;  
    [SerializeField]
    private GameObject areaCalculatorGO;
    private IAreaCalculator areaCalculator;

    // Start is called before the first frame update
    void Start()
    {
        areaCalculator = areaCalculatorGO.GetComponent<IAreaCalculator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
