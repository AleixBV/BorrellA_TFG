using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	[SerializeField]
	GameObject cameraObject;
	[SerializeField]
	GameObject[] targetCameraPoints;
	public List<string> targetCameraNames = new List<string>();
	private int activeCameraNum;

	const float speedMoveStreet = 1.0f;
	const float speedRotateRotary =  5.0f;

	// Use this for initialization
	void Start () {
		foreach (GameObject targetCameraPoint in targetCameraPoints)
		{
			targetCameraNames.Add(targetCameraPoint.name);
		}
	}
	
	// Update is called once per frame
	void Update () {

		switch (activeCameraNum)
		{
		case 1:
			cameraObject.transform.Rotate(Vector3.forward * Time.deltaTime * speedRotateRotary);
			break;

		case 5:
			cameraObject.transform.Translate(Vector3.forward * Time.deltaTime * speedMoveStreet);
			if (cameraObject.transform.localPosition.z < -520.0f)
			{
				ChangeCamera(5);
			}
			break;

		case 6:
			cameraObject.transform.Translate(Vector3.forward * Time.deltaTime * speedMoveStreet);
			if (cameraObject.transform.localPosition.z > -150.0f)
			{
				ChangeCamera(6);
			}
			break;

		case 7:
			cameraObject.transform.Translate(Vector3.forward * Time.deltaTime * speedMoveStreet);
			if (cameraObject.transform.localPosition.z > -230.0f)
			{
				ChangeCamera(7);
			}
			break;

		case 8:
			cameraObject.transform.Rotate(Vector3.up * Time.deltaTime * speedRotateRotary);
			break;
		}

	}

	public void ChangeCamera (int targetCameraNumber) {

		activeCameraNum = targetCameraNumber;
		if (targetCameraNumber < 100)
		{
			cameraObject.transform.parent = null;
			cameraObject.transform.localPosition = targetCameraPoints[targetCameraNumber].transform.position;
			cameraObject.transform.localEulerAngles = targetCameraPoints[targetCameraNumber].transform.localEulerAngles;
		}
	}
}
