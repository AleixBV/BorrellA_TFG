using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GPS_Path : MonoBehaviour {

    [SerializeField] private Transform position;
    [SerializeField] private Transform destination;
    [SerializeField] private LineRenderer line;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float timer_recalculate = 1.0f;

    private float timer_recalculate_progress = 0.0f;

    // Use this for initialization
    void Start () {
        //timer_recalculate_progress = timer_recalculate;
    }
	
	// Update is called once per frame
	void Update () {
        timer_recalculate_progress += Time.deltaTime;

        if(timer_recalculate_progress >= timer_recalculate)
        {
            timer_recalculate_progress = 0.0f;

            SetPath();
        }
	}

    void SetPath()
    {
        agent.isStopped = true;
        agent.enabled = false;
        transform.SetPositionAndRotation(position.position, position.rotation);
        agent.enabled = true;

        if (agent.hasPath)
            agent.ResetPath();

        agent.SetDestination(destination.position);
        //Debug.Log("recalcule path gps");

        StopAllCoroutines();
        StartCoroutine(DrawPath());
    }

    IEnumerator DrawPath()
    {
        yield return new WaitUntil(() => !agent.pathPending);

        if (agent.path.corners.Length < 2)
        {
            Debug.Log("gps path length too small");
            yield break;
        }

        line.positionCount = agent.path.corners.Length;

        for (int x = 0; x < agent.path.corners.Length; x++)
            line.SetPosition(x, agent.path.corners[x]);
    }
}
