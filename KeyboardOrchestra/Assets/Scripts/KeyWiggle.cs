using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWiggle : MonoBehaviour {

	public float minRotation;
	public float maxRotation;
	public float speed;

	public GameObject main;
	public MainController mainScript;

	private float scale;
	private float prevBeat;
	private bool newBeat;
	private float destruction;

	private int clockwise;
	private Vector3 rotationPosition;
	private  float thisRotation;
	private bool doRotations;

	// Use this for initialization
	void Start () {
		scale = 1;
		prevBeat = 0;
		newBeat = true;
		mainScript = main.GetComponent<MainController> ();
		destruction = 1f;

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


		//start pulsing with music
		if (mainScript.currRound >= 7) {
			pulseKeys ();
		}

		//if they got something wrong, change speed
		if (mainScript.staticLevel > 0) {
			speed = 70f + mainScript.staticLevel * 20f;
		} else {
			speed = 70f;
		}
		if (mainScript.staticLevel == 3) {
			destruction += .001f;
			transform.localScale = new Vector3(destruction, destruction, 1);
		}

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

	void pulseKeys(){

		//SOLUTION WITH POS DOESNT ALWAYS SYNC WITH BEAT
		float fraction = 1f / mainScript.timestep; //change to timestep later
		float chuckBeat = ((mainScript.myPos - mainScript.previousPos) % fraction) / fraction;	

		//flip chuckBeat around to decrease size
		if (chuckBeat >= .5) {
			chuckBeat = 1 - chuckBeat;
		}
		scale = Mathf.Pow (chuckBeat, 2);
		scale = .9f + ((.2f) / (1.1f)) * scale*2;
		transform.localScale = new Vector3 (scale, scale, scale);

//		if (mainScript.myBeat%2 == 0 && prevBeat != mainScript.myBeat) {
//			prevBeat = mainScript.myBeat;
//			newBeat = !newBeat;
//		}
//		if (newBeat) {
//			scale -= .009f;
//		} else {
//			scale += .009f;
//		}
//		transform.localScale = new Vector3(scale, scale, scale);

	}
}