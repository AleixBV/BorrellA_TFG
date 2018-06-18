using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Front_Trigger : MonoBehaviour {

    [SerializeField] MSVehicleController Player;
    [SerializeField] AI_Car AI_Car;
    [SerializeField] Player_Front_Trigger Player_Front_Trigger;
    [SerializeField] Collider AI_Collider;

    bool colliding_player = false;
    List<Collider> colliding = new List<Collider>();
    bool avoiding_obstacle = false;
    float braking_timer = Public_Vars.braking_timer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AICarsCollider")
        {
            if (!avoiding_obstacle)
                AI_Car.SetMaxSpeed(other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f));

            colliding.Add(other);
        }
        else if (other.tag == "Player")
        {
            if (!avoiding_obstacle)
                AI_Car.SetMaxSpeed(Player.GetVelocity() - (Player.GetVelocity() / 50.0f));

            colliding_player = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!avoiding_obstacle && other.tag == "AICarsCollider")
        {
            if (other.transform.parent.GetComponentInChildren<AI_Front_Trigger>().GetColliding().Contains(AI_Collider))
            {
                AI_Car.ResetMaxSpeed();
                colliding.Remove(other);
                braking_timer = Public_Vars.braking_timer;

                StartCoroutine(EnableAvoidingObstacles());
            }
            else
            {
                float new_speed = 0.0f;
                if (Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position)) > 4.0f)
                    new_speed = other.GetComponentInParent<AI_Car>().GetAgent().speed - (other.GetComponentInParent<AI_Car>().GetAgent().speed / 50.0f);

                if (!avoiding_obstacle && AI_Car.GetMaxSpeed() > new_speed)
                    AI_Car.SetMaxSpeed(new_speed);
            }
        }
        else if (other.tag == "Player")
        {
            float test = Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position));

            float new_speed = 0.0f;
            if (Vector3.Distance(AI_Car.GetCollider().ClosestPoint(other.transform.position), other.ClosestPoint(AI_Car.transform.position)) > 4.0f)
                new_speed = Player.GetVelocity() - (Player.GetVelocity() / 50.0f);

            if (!avoiding_obstacle && AI_Car.GetMaxSpeed() > new_speed)
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

            if (AI_Car.GetMaxSpeed() <= 2.5f)
            {
                if (braking_timer <= 0.0f)
                {
                    braking_timer = Public_Vars.braking_timer;

                    if (Player_Front_Trigger.GetColliding().Count > 0)
                    {
                        foreach (Collider col in Player_Front_Trigger.GetColliding())
                        {
                            if (IsCarStoppedRed(col))
                        {
                            braking_timer = col.transform.parent.GetComponent<AI_Car>().GetDestination().RemainingRedLightTime() + Public_Vars.braking_timer;
                            return;
                            }
                        }
                    }

                    if (AI_Car.GetDestination().GetRedLightOn() && Vector3.Distance(Player.transform.position, AI_Car.GetDestination().transform.position) < 15.0f)
                    {
                        braking_timer = AI_Car.GetDestination().RemainingRedLightTime() + Public_Vars.braking_timer;
                        return;
                    }
                    else if (Vector3.Distance(Player.transform.position, AI_Car.GetDestination().transform.position) < 15.0f)
                    {
                        AI_Car.SetDestinationRadius(Vector3.Distance(AI_Car.transform.position, AI_Car.GetDestination().transform.position) + 1.0f);
                    }


                    AI_Car.PlayHorn();

                    StartCoroutine(EnableAvoidingObstacles());
                }
            }
            else
                braking_timer = Public_Vars.braking_timer;
        }
    }

    private bool IsCarStoppedRed(Collider col)
    {
        bool ret = false;

        if (col.transform.parent.GetComponent<AI_Car>().IsCarStoppedRed())
        {
            ret = true;
        }
        else
        {
            foreach (Collider collider in col.transform.parent.GetComponentInChildren<AI_Front_Trigger>().GetColliding())
            {
                if (IsCarStoppedRed(collider))
                    return true;
            }
        }

        return ret;
    }

    private IEnumerator EnableAvoidingObstacles()
    {
        if (avoiding_obstacle)
            yield break;

        AI_Car.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        avoiding_obstacle = true;
        
        AI_Car.ResetMaxSpeed();

        Debug.Log(AI_Car.name + " avoiding enabled");

        yield return new WaitForSeconds(5.0f);

        AI_Car.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        avoiding_obstacle = false;
    }

    public List<Collider> GetColliding()
    {
        return colliding;
    }

    public bool IsCollidingWithPlayer()
    {
        return colliding_player;
    }
}
