using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Public_Vars : MonoBehaviour {

    public static bool is_controller_enabled;
    public static bool forced_controller_disabled = false;
    public static bool forced_VR_disabled = false;
    public static bool game_paused = false;

    public static GameObject Public_Vars_instance = null;

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
