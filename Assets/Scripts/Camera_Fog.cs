using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Fog : MonoBehaviour {

    private bool old_fog;
    [SerializeField] private bool fog = true;

    private void OnPreRender()
    {
        old_fog = RenderSettings.fog;
        RenderSettings.fog = fog;
    }

    private void OnPostRender()
    {
        RenderSettings.fog = old_fog;
    }
}
