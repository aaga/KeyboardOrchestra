using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWiggle : MonoBehaviour {

	public float minRotation;
	public float maxRotation;
	public float speed;

	private float scale;

	private int clockwise;
	private Vector3 rotationPosition;
	private  float thisRotation;
	private bool doRotations;

	// Use this for initialization
	void Start () {
		scale = 1;

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

		//ChucK sends a num 0-1 and 1 is when we want scale to be largest (on beat)

		/*
		float pulseSpeed = 40f; //todo: change to chuck value
		float pulseRange = .15f; // pulse from .8 to 1 times the scale
		//PingPongs the value t, so that it is never larger than length and never smaller than 0.
		scale = Mathf.PingPong(Time.time * pulseSpeed, 10);
		scale = Mathf.Pow (scale, 2);
		scale = (1 - pulseRange) + (scale / 100f) * (pulseRange);
		transform.localScale = new Vector3(scale, scale, scale);
		if (scale >= .99) {
			Debug.Log ("time: " + Time.time);
		}

		float pulseRange = .15f; // pulse from .8 to 1 times the scale
		float chuckBeat;
		chuckBeat * 20;	//0-20 (switch at 10)

		//flip chuckBeat around to decrease size
		if(chuckBeat >= 10){
			chuckBeat = 20 - chuckBeat;
		}
		chuckBeat = Mathf.Pow(chuckBeat,2);	//0-100 in quadratic
		scale = (1 - pulseRange) + (scale / 100f) * (pulseRange);
		transform.localScale = new Vector3(scale, scale, scale);
*/

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