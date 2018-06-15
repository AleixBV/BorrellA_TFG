using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Cinematic : MonoBehaviour {

    [SerializeField] List<Transform> destination = new List<Transform>();

    private NavMeshAgent agent;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private int nextDestination = 0;

    private void Awake()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
         agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(destination[nextDestination++].position);
    }

    private void FixedUpdate()
    {
        if(!agent.pathPending && agent.remainingDistance < 5.0f)
        {
            if (nextDestination >= destination.Count)
            {
                nextDestination = 0;
                agent.enabled = false;
                transform.localPosition = initialPos;
                transform.localRotation = initialRot;
                agent.enabled = true;
            }
            else
            {
                agent.SetDestination(destination[nextDestination++].position);
            }
        }
    }
}
