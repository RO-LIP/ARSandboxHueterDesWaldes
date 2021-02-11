using UnityEngine;
using UnityEngine.UI;

public class AirQuality : MonoBehaviour
{
    private float minQuality = 0;
    private float maxQuality = 1;

    public static float airQuality;
    
    private Slider slider;
    public Image fillImage;
    public Gradient gradient;

    void Awake()
    {
        SetAirQuality();
    }

    private void Start()
    {
    }

    void Update()
    {
        UpdateAirQualityBar();
    }

    // Set initial value of AirQuality Bar at 50%
    private void SetAirQuality()
    {
        slider = GetComponent<Slider>();
        airQuality = maxQuality / 2;
        slider.value = airQuality / maxQuality;
        fillImage.color = gradient.Evaluate(slider.value);
    }

    private void UpdateAirQualityBar()
    {
        // currentQuality must allways be in-between min and max Quality
        if (airQuality < minQuality)
        {
            airQuality = minQuality;
        }
        else if (airQuality > maxQuality)
        {
            airQuality = maxQuality;
        }
        slider.value = airQuality / maxQuality;
        fillImage.color = gradient.Evaluate(slider.value);
    }
}
