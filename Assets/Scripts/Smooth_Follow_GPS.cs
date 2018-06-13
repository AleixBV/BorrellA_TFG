using UnityEngine;

public class Smooth_Follow_GPS : MonoBehaviour
{

    // The target we are following
    [SerializeField] private Transform target;
    // the height we want the camera to be above the target
    [SerializeField] private float height = 175.0f;

    [SerializeField] private float posDamping;
    [SerializeField] private float rotDamping;

    // Use this for initialization
    void Start()
    {
        if (!target)
            return;

        //initialize with target position plus desired height
        var wantedPos = target.position;
        wantedPos.y += height;

        transform.position = wantedPos;

        var wantedRot = transform.eulerAngles;
        wantedRot.y = target.eulerAngles.y;
        transform.eulerAngles = wantedRot;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current position
        var wantedPos = target.position;
        wantedPos.y += height;

        var currentPos = transform.position;

        var finalPos = transform.position;

        // Damp the position
        finalPos.x = Mathf.Lerp(currentPos.x, wantedPos.x, posDamping * Time.deltaTime);
        finalPos.z = Mathf.Lerp(currentPos.z, wantedPos.z, posDamping * Time.deltaTime);

        //Calculate the current rotation
        var wantedRot = transform.eulerAngles;

        //Damp the rotation
        wantedRot.y = Mathf.LerpAngle(transform.eulerAngles.y, target.eulerAngles.y, rotDamping * Time.deltaTime);
        
        // Set the position of the camera on the x-z plane
        transform.position = finalPos;

        // Set the rotation of the camera on the y plane
        transform.eulerAngles = wantedRot;
    }
}