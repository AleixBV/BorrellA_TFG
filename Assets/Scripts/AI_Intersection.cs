using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Intersection : MonoBehaviour {

    [System.Serializable]
    public struct Next_Destination
    {
        public AI_Intersection nextDestination;
        public Transform stopPlace;
    }

    public bool isExit = false;
    [SerializeField] float radius = 0.1f;
    [SerializeField] Next_Destination[] possibleNextDestinations;
    [SerializeField] bool haveTrafficLight = false;
    [SerializeField] AI_Traffic_Light[] trafficLights;
    [SerializeField] bool trafficRedLightOn = false;

    float trafficLightsTimer = Public_Vars.traffic_light_time;

    private void Start()
    {
        if (haveTrafficLight)
        {
            if (trafficRedLightOn)
                SetTrafficLight(AI_Traffic_Light.TrafficLightColor.RED);
            else
                SetTrafficLight(AI_Traffic_Light.TrafficLightColor.BLUE);
        }
    }

    private void FixedUpdate()
    {
        if (haveTrafficLight)
        {
            trafficLightsTimer -= Time.fixedDeltaTime;

            if (trafficRedLightOn)
            {
                if (trafficLightsTimer <= 0.0f)
                {
                    SetTrafficLight(AI_Traffic_Light.TrafficLightColor.BLUE);
                    trafficRedLightOn = false;
                    trafficLightsTimer = Public_Vars.traffic_light_time;
                }
            }
            else
            {
                if (trafficLightsTimer <= 0.0f)
                {
                    SetTrafficLight(AI_Traffic_Light.TrafficLightColor.RED);
                    trafficRedLightOn = true;
                    trafficLightsTimer = Public_Vars.traffic_light_time + (Public_Vars.traffic_light_yellow_time / 2);
                }
                else if (trafficLightsTimer <= Public_Vars.traffic_light_yellow_time)
                    SetTrafficLight(AI_Traffic_Light.TrafficLightColor.YELLOW);
            }
        }
    }

    private void SetTrafficLight(AI_Traffic_Light.TrafficLightColor color)
    {
        foreach(AI_Traffic_Light light in trafficLights)
            light.ChangeColor(color);
    }

    private int PossibleDestinationContains(AI_Intersection contains)
    {
        int ret = 0;
        foreach (Next_Destination dest in possibleNextDestinations)
        {
            if (dest.nextDestination == contains || dest.stopPlace == contains)
                return ret;
            ret++;
        }
        return -1;
    }

    public Next_Destination GetNextDestination(AI_Intersection origin)
    {
        int originId = PossibleDestinationContains(origin);
        if (originId >= 0)
        {
            int nextDest = Random.Range(0, possibleNextDestinations.Length - 1);
            if (nextDest >= originId)
                nextDest++;
            return possibleNextDestinations[nextDest];
        }
        else
        {
            return possibleNextDestinations[Random.Range(0, possibleNextDestinations.Length)];
        }
    }

    public float GetRadius()
    {
        return radius;
    }

    public bool GetRedLightOn()
    {
        return trafficRedLightOn;
    }

    public float RemainingRedLightTime()
    {
        return trafficLightsTimer;
    }
}
