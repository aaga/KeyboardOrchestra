using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	//Instruciton Text, input Text, action
	private string[,,] specialWords = new string[,,] { {{ "Cue the horns", "horns", "greyOut"}, { "Waiting for next instruction...", "", "waiting"}}, {{ "Add Bass Line", "b", "greyOut"}, { "Play this Melody", "fff ce d ff g", "greyOut"}}};

	public GameObject step1;
	public GameObject step2;

	private MyStepController step1Script;
	private MyStepController step2Script;

//	public GameObject step3;
//	public GameObject step4;

	private int currRound = 0;
	private bool updatedRound;

	// Use this for initialization
	void Start () {
		updatedRound = false;
		step1Script = step1.GetComponent<MyStepController> ();
		step2Script = step2.GetComponent<MyStepController> ();
	}
	
	// Update is called once per frame
	void Update () {
		getKey ();
		if (!updatedRound) {
			//update
			step1Script.stepInstructions = oneD(currRound,0);
			step2Script.stepInstructions = oneD(currRound,1);
			Debug.Log ("step 2 instruction set" + step2Script.stepInstructions[0]);
			Debug.Log ("step 1 instruction set" + step1Script.stepInstructions[0]);
			step1Script.newRound = true;
			step2Script.newRound = true;
			updatedRound = true;
		}
	}

	string[] oneD(int index1, int index2) {
		string[] oneDArray = new string[3];
		oneDArray [0] = specialWords [index1, index2, 0];
		oneDArray [1] = specialWords [index1, index2, 1];
		oneDArray [2] = specialWords [index1, index2, 2];
		return oneDArray;
	}

	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				if ("Return" == vKey.ToString ()) {
					if (currRound < specialWords.GetLength(0)) {
						currRound++;
						updatedRound = false;
					}
					Debug.Log ("Current Round: " + currRound);
				}

			}
		}
	}
}

