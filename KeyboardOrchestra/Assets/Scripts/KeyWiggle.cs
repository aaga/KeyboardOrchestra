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
	private bool doRotations;

	// Use this for initialization
	void Start () {
		doRotations = true;
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

	public void stopRotations() {
		doRotations = false;
	}

	public void startRotations() {
		doRotations = true;
	}

	// Update is called once per frame
	void Update () {
//		float scale = .1f + Mathf.PingPong(Time.time * 0.2f, 1f - .1f);
//		transform.localScale = new Vector3(scale, scale, scale);
		if (doRotations) {
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
		}
	}
}
