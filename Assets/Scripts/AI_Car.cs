using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Car : MonoBehaviour {

    [SerializeField] MSSceneController sceneController;
    [SerializeField] GameObject targetAICar;
    [SerializeField] AI_Intersection navMeshDest;
    [SerializeField] AI_Spawn_Container spawnContainer;

    UnityEngine.AI.NavMeshAgent navMeshAgentComponent;
    [SerializeField] private float CAR_MAX_SPEED = 10.0f;
    private float max_speed;

    [SerializeField] AudioClip[] beepSounds;
    AudioSource beepSoundAUD;

    private bool nextDestinationIsStop = false;
    private float destinationRadius;

    private bool first_start = true;
    private bool _car_stoped_red = false;

    // Use this for initialization
    void Awake()
    {
        navMeshAgentComponent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        max_speed = CAR_MAX_SPEED;

        destinationRadius = navMeshDest.GetRadius();

        beepSoundAUD = GetComponent<AudioSource>();
        beepSoundAUD.clip = beepSounds[0];
    }

    public void InitAICar()
    {
        navMeshAgentComponent.speed = 0.0f;
        targetAICar.GetComponent<Animation>().Play("00_Stop");
        stopCar();
        StartCoroutine(startCar(Public_Vars.stop_time));
    }

    IEnumerator startCar(float startDelayTime)
    {
        yield return new WaitForSeconds(startDelayTime);

        // Set destination
        if (first_start)
        {
            navMeshAgentComponent.SetDestination(navMeshDest.transform.position);
            first_start = false;
        }

        yield return new WaitForSeconds(0.5f);
        navMeshAgentComponent.speed = max_speed;
        targetAICar.GetComponent<Animation>().Play("01_Run");
        _car_stoped_red = false;
    }

    void stopCar()
    {
        navMeshAgentComponent.speed = 0.0f;
        targetAICar.GetComponent<Animation>().Play("00_Stop");
    }

    private void FixedUpdate()
    {
        if (!_car_stoped_red && !navMeshAgentComponent.pathPending && navMeshAgentComponent.hasPath && navMeshAgentComponent.remainingDistance < destinationRadius)
        {
            if (!nextDestinationIsStop && navMeshDest.name.Contains("before"))
                StartCoroutine(EnableAutoBrake(2.0f));

            if (navMeshDest.GetRedLightOn())
            {
                _car_stoped_red = true;
                stopCar();
                StartCoroutine(startCar(0.5f + navMeshDest.RemainingRedLightTime()));
            }

            if (nextDestinationIsStop)
            {
                if (navMeshDest.name.Contains("before"))
                {
                    navMeshAgentComponent.SetDestination(navMeshDest.transform.position);
                    destinationRadius = navMeshDest.GetRadius();
                    navMeshAgentComponent.autoBraking = false;
                }
                stopCar();
                StartCoroutine(startCar(Public_Vars.stop_time));
                nextDestinationIsStop = false;
            }
            else if (navMeshDest.isExit)
            {
                AI_Intersection tmpNavMeshDest = spawnContainer.GetRandomSpawner();
                if (Vector3.Distance(tmpNavMeshDest.transform.position, sceneController.vehicles[0].transform.position) < Public_Vars.spawn_dist)
                    return;

                navMeshDest = tmpNavMeshDest;

                navMeshAgentComponent.enabled = false;
                transform.position = navMeshDest.transform.position;
                transform.rotation = navMeshDest.transform.rotation;
                navMeshAgentComponent.enabled = true;
                //navMeshAgentComponent.Warp(navMeshDest.transform.position);

                navMeshDest = navMeshDest.GetNextDestination(navMeshDest).nextDestination;
                navMeshAgentComponent.SetDestination(navMeshDest.transform.position);
                destinationRadius = navMeshDest.GetRadius();
            }
            else
            {
                AI_Intersection.Next_Destination nextDest = navMeshDest.GetNextDestination(navMeshDest);
                navMeshDest = nextDest.nextDestination;

                if (nextDest.stopPlace != null)
                {
                    nextDestinationIsStop = true;
                    navMeshAgentComponent.SetDestination(nextDest.stopPlace.position);
                    destinationRadius = 1.0f;
                }
                else
                {
                    if (navMeshDest.name.Contains("before"))
                        navMeshAgentComponent.autoBraking = false;

                    navMeshAgentComponent.SetDestination(navMeshDest.transform.position);
                    destinationRadius = navMeshDest.GetRadius();
                }
            }
        }
    }

    private IEnumerator EnableAutoBrake(float delay)
    {
        yield return new WaitForSeconds(delay);
        navMeshAgentComponent.autoBraking = true;
    }

    public UnityEngine.AI.NavMeshAgent GetAgent()
    {
        return navMeshAgentComponent;
    }

    public void SetMaxSpeed(float new_max_speed)
    {
        max_speed = new_max_speed;

        if (navMeshAgentComponent.speed > 0.0f)
        {
            navMeshAgentComponent.speed = new_max_speed;
            if (new_max_speed < 0.1f)
                stopCar();
        }
    }

    public void ResetMaxSpeed()
    {
        max_speed = CAR_MAX_SPEED;

        if (navMeshAgentComponent.speed > 0.0f)
            navMeshAgentComponent.speed = max_speed;
        else
            StartCoroutine(startCar(0.25f));
    }

    public float GetMaxSpeed()
    {
        return max_speed;
    }

    public Collider GetCollider()
    {
        return targetAICar.GetComponent<Collider>();
    }

    public void PlayHorn()
    {
        if(!beepSoundAUD.isPlaying)
            beepSoundAUD.PlayOneShot(beepSoundAUD.clip);
    }

    public AI_Intersection GetDestination()
    {
        return navMeshDest;
    }

    public void SetDestinationRadius(float radius)
    {
        destinationRadius = radius;
    }

    public float GetDestinationRadius()
    {
        return destinationRadius;
    }

    public UnityEngine.AI.NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgentComponent;
    }

    public bool IsCarStoppedRed()
    {
        return _car_stoped_red;
    }
}
