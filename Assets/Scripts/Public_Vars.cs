using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Public_Vars : MonoBehaviour {

    public static bool is_controller_enabled = false;
    public static bool forced_controller_disabled = false;
    public static bool forced_VR_disabled = false;
    public static bool game_paused = false;
    public static bool instant_acceleration = false;

    public static GameObject Public_Vars_instance = null;

    public static float traffic_light_time = 15.0f;
    public static float traffic_light_yellow_time = 3.0f;
    public static float stop_time = 2.0f;
    public static float spawn_dist = 100.0f;
    public static float braking_timer = 7.0f;

    public static bool final_cinematic = false;

    // Use this for initialization
    void Start () {
        if (Public_Vars_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Public_Vars_instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
