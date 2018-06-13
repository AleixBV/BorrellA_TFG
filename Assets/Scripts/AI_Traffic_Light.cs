using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Traffic_Light : MonoBehaviour {

    [SerializeField] GameObject BlueLight;
    [SerializeField] GameObject YellowLight;
    [SerializeField] GameObject RedLight;

    public enum TrafficLightColor
    {
        BLUE = 0,
        YELLOW,
        RED
    }

    public bool isRed = false;

	public void ChangeColor(TrafficLightColor color)
    {
        BlueLight.SetActive(false);
        YellowLight.SetActive(false);
        RedLight.SetActive(false);

        switch (color)
        {
            case TrafficLightColor.BLUE:
                BlueLight.SetActive(true);
                break;
            case TrafficLightColor.YELLOW:
                YellowLight.SetActive(true);
                break;
            case TrafficLightColor.RED:
                RedLight.SetActive(true);
                break;
            default:
                break;
        }
    }
}
