using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outside_Trigger : MonoBehaviour {

    [SerializeField] private ScriptGUIController_Main Gui_Cotroller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Gui_Cotroller.SetRespawn();
        }
    }
}
