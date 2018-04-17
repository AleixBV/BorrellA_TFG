using UnityEngine;
using System.Collections;

public class AICarMove : MonoBehaviour {

	[SerializeField]
	GameObject targetAICar;
	[SerializeField]
	GameObject[] targetNavMeshObjects;
	int targetNavMeshObjectCounts;
	int targetNavMeshObjectNow;

	Vector3 startPos;
	Vector3 startRot;

	UnityEngine.AI.NavMeshAgent navMeshAgentComponent;
	const float CAR_SPEED_MAX = 1.0f;

	// Use this for initialization
	void Start () {

		navMeshAgentComponent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
		startPos = targetNavMeshObjects[0].transform.localPosition;
		startRot = targetNavMeshObjects[0].transform.localEulerAngles;
		targetNavMeshObjectCounts = targetNavMeshObjects.Length -1;

	}

	public void InitAICar () {

		navMeshAgentComponent.speed = 0.0f;
		targetAICar.GetComponent<Animation>().Play("00_Stop");
		StartCoroutine(startCar(3.0f));

	}

	IEnumerator startCar (float startDelayTime) {

		navMeshAgentComponent.speed = 0.0f;
		targetAICar.GetComponent<Animation>().Play("00_Stop");
		yield return new WaitForSeconds(startDelayTime);

		// Set destination
		targetNavMeshObjectNow = 1;
		navMeshAgentComponent.SetDestination(targetNavMeshObjects[targetNavMeshObjectNow].transform.localPosition);
		this.transform.localPosition = startPos;
		this.transform.localEulerAngles = startRot;

		yield return new WaitForSeconds(0.5f);
		navMeshAgentComponent.speed = CAR_SPEED_MAX;
		targetAICar.GetComponent<Animation>().Play("01_Run");

	}

	
	// Update is called once per frame
	void Update () {

		if (navMeshAgentComponent.remainingDistance < 0.1f)
		{
			targetNavMeshObjectNow ++;
			if (targetNavMeshObjectNow <= targetNavMeshObjectCounts)
			{
				navMeshAgentComponent.SetDestination(targetNavMeshObjects[targetNavMeshObjectNow].transform.localPosition);
			}
			else if (targetNavMeshObjectNow >  targetNavMeshObjectCounts)
			{
				targetNavMeshObjectNow = 1;
				navMeshAgentComponent.SetDestination(targetNavMeshObjects[targetNavMeshObjectNow].transform.localPosition);
			}
		}
	
	}
}
