using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final_Trigger : MonoBehaviour {

    [SerializeField] Transform destination;
    [SerializeField] Final_Cinematic Final_Cinematic;

    [SerializeField] AI_Traffic_Light[] light_red;
    [SerializeField] AI_Traffic_Light[] light_blue;

    [SerializeField] GameObject AI_Container;

    private void OnTriggerEnter(Collider other)
    {
        if (!Public_Vars.final_cinematic && other.tag == "Player")
        {
            Public_Vars.final_cinematic = true;
            Final_Cinematic.StartCinematic(destination);

            AI_Container.SetActive(true);

            foreach (AI_Traffic_Light light in light_red)
                light.ChangeColor(AI_Traffic_Light.TrafficLightColor.RED, true);

            foreach (AI_Traffic_Light light in light_blue)
                light.ChangeColor(AI_Traffic_Light.TrafficLightColor.BLUE, true);
        }
    }
}
