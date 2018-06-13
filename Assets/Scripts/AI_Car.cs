﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Car : MonoBehaviour {

    [SerializeField] MSSceneController sceneController;
    [SerializeField] GameObject targetAICar;
    [SerializeField] AI_Intersection navMeshDest;
    [SerializeField] AI_Spawn_Container spawnContainer;

    UnityEngine.AI.NavMeshAgent navMeshAgentComponent;
    [SerializeField] private float CAR_SPEED = 10.0f;

    private bool nextDestinationIsStop = false;
    private float destinationRadius;

    private bool first_start = true;

    // Use this for initialization
    void Start()
    {
        navMeshAgentComponent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        destinationRadius = navMeshDest.GetRadius();
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
        navMeshAgentComponent.speed = CAR_SPEED;
        targetAICar.GetComponent<Animation>().Play("01_Run");

    }

    void stopCar()
    {
        navMeshAgentComponent.speed = 0.0f;
        targetAICar.GetComponent<Animation>().Play("00_Stop");
    }

    private void FixedUpdate()
    {
        if (!navMeshAgentComponent.pathPending && navMeshAgentComponent.hasPath && navMeshAgentComponent.remainingDistance < destinationRadius)
        {
            if (!nextDestinationIsStop && navMeshDest.name.Contains("before"))
                StartCoroutine(EnableAutoBrake(2.0f));

            if (navMeshDest.GetRedLightOn())
            {
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
                    destinationRadius = 0.1f;
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
}
