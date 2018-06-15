using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outside_Trigger : MonoBehaviour {

    [SerializeField] private ScriptGUIController_Main Gui_Cotroller;
    [SerializeField] private bool Final_Cinematic = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (Final_Cinematic)
                Gui_Cotroller.SetRespawnTimer(0.35f, true);

            Gui_Cotroller.SetRespawn();
        }
    }
}
