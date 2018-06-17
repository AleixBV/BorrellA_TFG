using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Only_Yellow_Light : MonoBehaviour {

    float timer = 0.0f;
    Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= Public_Vars.traffic_light_yellow_time / 4.0f)
        {
            light.enabled = !light.enabled;
            timer = 0.0f;
        }
    }
}
