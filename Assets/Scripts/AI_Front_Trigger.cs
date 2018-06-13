using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Front_Trigger : MonoBehaviour {

    [SerializeField] MSVehicleController Player;
    [SerializeField] AI_Car AI_Car;

    bool colliding_player = false;
    bool colliding = false;
    float braking_timer = Public_Vars.braking_timer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AICarsCollider")
        {
            AI_Car.SetMaxSpeed(other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f));

            colliding = true;
        }
        else if (other.tag == "Player")
        {
            AI_Car.SetMaxSpeed(Player.GetVelocity() - (Player.GetVelocity() / 50.0f));

            colliding_player = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "AICarsCollider")
        {
            float new_speed = 0.0f;
            if(Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position)) > 4.0f)
                new_speed = other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f);

            if(AI_Car.GetMaxSpeed() > new_speed)
                AI_Car.SetMaxSpeed(new_speed);
        }
        else if (other.tag == "Player")
        {
            float test = Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position));

            float new_speed = 0.0f;
            if (Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position)) > 4.0f)
                new_speed = Player.GetVelocity() - (Player.GetVelocity() / 50.0f);

            if (AI_Car.GetMaxSpeed() > new_speed)
                AI_Car.SetMaxSpeed(new_speed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "AICarsCollider")
        {
            AI_Car.ResetMaxSpeed();

            colliding = false;
        }
        else if(other.tag == "Player")
        {
            AI_Car.ResetMaxSpeed();

            colliding_player = false;
            braking_timer = Public_Vars.braking_timer;
        }
    }

    private void FixedUpdate()
    {
        if(colliding_player)
        {
            braking_timer -= Time.fixedDeltaTime;

            if(braking_timer <= 0.0f)
            {
                //TODO
                Debug.Log("BEEP");

                braking_timer = Public_Vars.braking_timer / 2;
            }
        }
    }
}
