using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final_Cinematic : MonoBehaviour {

    [SerializeField] MSSceneController sceneController;
    [SerializeField] MSVehicleController vehicleController;
    private Transform destination = null;

    bool colliding_active = false;
    bool colliding_active_delay = false;
    bool _x_dir = false;

    private void FixedUpdate()
    {
        if (Public_Vars.final_cinematic && destination != null)
        {
            Transform vehicle = sceneController.vehicles[0].transform;
            Vector3 direction;
            if (_x_dir)
            {
                direction = (vehicle.position - destination.position);
            }
            else
            {
                direction = ((destination.position.z) > (vehicle.position.z)) ?
                (destination.position - vehicle.position) : (vehicle.position - destination.position);
            }
            //Debug.Log(Vector3.Cross(sceneController.vehicles[0].transform.forward, direction).z);

            float angle = Vector3.Angle(sceneController.vehicles[0].transform.forward, direction);

            sceneController.cinematicVerticalInput = 1.0f;
            if (angle > 10.0f)
            {
                if (_x_dir)
                {
                    if (Vector3.Cross(sceneController.vehicles[0].transform.forward, direction).x > 0.0f)
                        sceneController.cinematicHorizontalInput = -Mathf.Lerp(1.0f, 0.05f, angle / 180.0f);
                    else
                        sceneController.cinematicHorizontalInput = Mathf.Lerp(1.0f, 0.05f, angle / 180.0f);
                }
                else
                {
                    if (Vector3.Cross(sceneController.vehicles[0].transform.forward, direction).z > 0.0f)
                        sceneController.cinematicHorizontalInput = Mathf.Lerp(1.0f, 0.05f, angle / 180.0f);
                    else
                        sceneController.cinematicHorizontalInput = -Mathf.Lerp(1.0f, 0.05f, angle / 180.0f);
                }
            }
            else
            {
                sceneController.cinematicHorizontalInput = 0.0f;
            }

            if (!_x_dir && Vector3.Cross(sceneController.vehicles[0].transform.forward, direction).x < 0.0f)
                sceneController.cinematicVerticalInput = -1.0f;
            else if (_x_dir && Vector3.Cross(sceneController.vehicles[0].transform.forward, direction).z < 0.0f)
            {
                sceneController.cinematicVerticalInput = -1.0f;
            }

            if (vehicleController.IsCollisioning())
            {
                if (!colliding_active_delay)
                    StartCoroutine(VehicleColliding());
            }

            if (colliding_active_delay)
            {
                sceneController.cinematicHorizontalInput = -sceneController.cinematicHorizontalInput;
                if (colliding_active)
                    sceneController.cinematicVerticalInput = -sceneController.cinematicVerticalInput;
            }
        }
    }

    private IEnumerator VehicleColliding()
    {
        colliding_active = true;
        colliding_active_delay = true;

        yield return new WaitForSeconds(1.0f);

        colliding_active = false;

        yield return new WaitForSeconds(0.5f);

        colliding_active_delay = false;
    }

    public void StartCinematic(Transform dest, bool x_dir = false)
    {
        destination = dest;
        _x_dir = x_dir;
    }
}
