using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	//Instruciton Text, input Text, action
	private string[,,] specialWords = new string[,,] { 
		{{ "Cue the synth", "play synth", "greyOut"}, { "Waiting for next instruction...", "", "waiting"}},
		{{ "Add Bass Line", "b", "greyOut"}, { "Type a Melody", "", "melody"}},
		{{ "Type a Melody", "", "melody"}, { "Waiting for next instruction...", "", "waiting"}},
		{{ "Waiting for next instruction...", "", "waiting"}, { "Type a Melody", "", "melody"}},
		{{ "Waiting for next instruction...", "", "waiting"}, { "Waiting for next instruction...", "", "waiting"}}
	};

	public GameObject step1;
	public GameObject step2;

	ChuckInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;

	private MyStepController step1Script;
	private MyStepController step2Script;

//	public GameObject step3;
//	public GameObject step4;

	private int currRound = 0;
	private float myPos;
	private float previousPos;
	private bool updatedRound;

	// Use this for initialization
	void Start () {
		updatedRound = false;
		myPos = 0.0f;
		previousPos = 0.0f;
		step1Script = step1.GetComponent<MyStepController> ();
		step2Script = step2.GetComponent<MyStepController> ();
		myChuck = GetComponent<ChuckInstance> ();
		myGetPosCallback = Chuck.CreateGetFloatCallback( GetPosCallback );
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

		myChuck.GetFloat ("pos", myGetPosCallback);
		//full loop has passed!!!
		if (myPos >= previousPos + 0.95f) {

			if (currRound == 0) {
				myChuck.RunCode ("0.5 => Global.synthGain;");

			} else if (currRound >= 1) {
				myChuck.RunCode ("0.5 => Global.bassGain;");
				myChuck.RunCode (step2Script.melodyString + @" @=> Global.synthMelody;");
				myChuck.RunCode (step1Script.melodyString + @" @=> Global.synthMelody;");
				Debug.Log ("passed this melody to chuck: " + step2Script.melodyString );
			}
		}

		if (myPos >= previousPos + 1.0f) {
			previousPos = previousPos + 1.0f;

			if (currRound < specialWords.GetLength(0)) {
				currRound++;
				updatedRound = false;
			}
			Debug.Log ("Current Round: " + currRound);
		}

		step1Script.linePos = myPos - previousPos;
		step2Script.linePos = myPos - previousPos;
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
					myChuck.RunCode (@"
						public class Global {
							static float synthGain;
			    			static float bassGain;
			    			static int synthMelody[];
			    			static int bassMelody[];
						}
						4 => external float timeStep;
						external float pos;

						fun void updatePos() {
							timeStep::second => dur currentTimeStep;
							currentTimeStep / 1000 => dur deltaTime;
							now => time startTime;
							
							pos => float originalPos;
											
							while( now < startTime + currentTimeStep )
							{
								deltaTime / currentTimeStep +=> pos;
								deltaTime => now;
							}
						}

						[60,63,67,72] @=> Global.synthMelody;
						[55,55,60,60] @=> Global.bassMelody;



						0 => Global.synthGain;
						0 => Global.bassGain;

						SinOsc synth => Gain localSynthGain => dac;
						SinOsc bass => Gain localBassGain => dac;
						0 => synth.freq;
						0 => bass.freq;	

						fun void playMelody() {
							for (0 => int i; i < 4; i++) {
							    for (0 => int x; x < Global.synthMelody.cap(); x++)
							    {
							        Global.synthMelody[x] => Std.mtof => synth.freq;
							        125::ms => now;
							        0 => synth.freq;
							        125::ms => now;
							    }
							}
						}
						
						fun void playBass() {
							for (0 => int i; i < 4; i++) {
							    for (0 => int x; x < Global.bassMelody.cap(); x++)
							    {
							        Global.bassMelody[x] => Std.mtof => bass.freq;
							        250::ms => now;

							    }
							}
						}
								
						while( true )
						{
							Global.synthGain => localSynthGain.gain;
							Global.bassGain => localBassGain.gain;
							spork ~ updatePos();
							spork ~ playMelody();
							spork ~ playBass();
							timeStep::second => now;				
						}
					");
				}

			}
		}
	}

	void GetPosCallback( System.Double pos )
	{
		myPos = (float) pos;
	}
}

