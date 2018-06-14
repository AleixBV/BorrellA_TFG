using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Front_Trigger : MonoBehaviour {

    [SerializeField] MSVehicleController Player;
    [SerializeField] AI_Car AI_Car;
    [SerializeField] Collider AI_Collider;

    bool colliding_player = false;
    List<Collider> colliding = new List<Collider>();
    bool avoiding_obstacle = false;
    float braking_timer = Public_Vars.braking_timer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AICarsCollider")
        {
            AI_Car.SetMaxSpeed(other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f));

            colliding.Add(other);
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
            if (!avoiding_obstacle && other.transform.parent.GetComponentInChildren<AI_Front_Trigger>().GetColliding().Contains(AI_Collider))
            {
                AI_Car.ResetMaxSpeed();
                colliding.Remove(other);
                colliding_player = false;
                braking_timer = Public_Vars.braking_timer;

                StartCoroutine(EnableAvoidingObstacles());
            }
            else
            {
                float new_speed = 0.0f;
                if (Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position)) > 4.0f)
                    new_speed = other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f);

                if (AI_Car.GetMaxSpeed() > new_speed)
                    AI_Car.SetMaxSpeed(new_speed);
            }
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

            colliding.Remove(other);
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

    private IEnumerator EnableAvoidingObstacles()
    {
        AI_Car.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        avoiding_obstacle = true;
        GetComponent<BoxCollider>().enabled = false;

        Debug.Log(AI_Car.name + " avoiding enabled");

        yield return new WaitForSeconds(3.0f);

        AI_Car.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        avoiding_obstacle = false;
        GetComponent<BoxCollider>().enabled = true;
    }

    public List<Collider> GetColliding()
    {
        return colliding;
    }
}
