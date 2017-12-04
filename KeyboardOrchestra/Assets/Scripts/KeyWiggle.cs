using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWiggle : MonoBehaviour {

	public float minRotation;
	public float maxRotation;
	public float speed;

	private int clockwise;
	private Vector3 rotationPosition;
	private  float thisRotation;

	// Use this for initialization
	void Start () {
		minRotation = 3.0f;
		maxRotation = 5.0f;
		if (Random.Range (0.0f, 1.0f) > 0.5f) {
			clockwise = 1;
		} else {
			clockwise = -1;
		}
		thisRotation = Random.Range (minRotation, maxRotation);
		speed = 70.0f;
		rotationPosition = new Vector3 (-3, -6, 0);
		rotationPosition += transform.position;
	}

	// Update is called once per frame
	void Update () {
		//transform.Rotate(0,0,mySine*10, Space.Self)

		transform.RotateAround (rotationPosition, Vector3.forward, Time.deltaTime * speed * clockwise);


		if (clockwise == 1) {
			if (transform.rotation.eulerAngles.z > thisRotation && transform.rotation.eulerAngles.z < 180.0f) {
				clockwise = -1;
				thisRotation = Random.Range (minRotation, maxRotation) * -1;
			}
		} else if (clockwise == -1) {
			if (transform.rotation.eulerAngles.z < 360.0f + thisRotation && transform.rotation.eulerAngles.z > 180.0f) {
				clockwise = 1;
				thisRotation = Random.Range (minRotation, maxRotation);
			}
		}


		//transform.RotateAround (transform.position  - new Vector3(0,3,0), Vector3.forward, (-1 * Time.time % 1) + 2f);



//		Vector3 mySine = new Vector3(0,1,0);
//		transform.LookAt(transform.position + mySine);
	}
}
