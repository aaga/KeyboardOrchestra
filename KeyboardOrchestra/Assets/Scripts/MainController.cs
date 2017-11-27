﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	//Instruciton Text, input Text 1, input Text 2, chuck code
	private string[,,] specialWords = new string[,,] { 
		{ { "Welcome to the Keyboard Orchestra. Type start to begin.", "", "start", "" }, {"Welcome to the Keyboard Orchestra. Type start to begin.", "", "start", "" } },
		{ { "You are player...", "", "one", ""}, { "You are player...", "", "two", ""} },
		{ { "You can also play your partner's keyboard!", "two", "", ""}, { "You can also play your partner's keyboard!", "one", "", ""} },
		{ { "Sometimes you have to press two keys at once", "a", "team", ""}, { "Sometimes you have to press two keys at once", "b", "team", ""} },
		{ { "Ready to get Started?", "", "ready", ""}, {"Ready to get Started?", "", "ready", ""} },
		{ { "Lay down the bass", "", "bass", "0.5 => Global.bassGain;"}, {"Waiting for next instruction...", "", "", ""} },
		{ { "Plug in the synth","in","in",""}, { "Set the Synth Melody", "", "acec", "0.7 => Global.synthGain;"} },
		{ { "Add a harmony", "2", "cefe", @"0.4 => Global.synthGain2;"}, {"Add a triplet!", ";;;", "; ;", "0.3 => Global.beatGain;0.4 => Global.synthGain;"} },
		{ { "Raise the key","7","key",@"[70,72,74,72] @=> Global.synthMelody2;[51,51,58,58] @=> Global.bassMelody;"}, { "Raise the roof", "99", "roof", "[67,68,70,68] @=> Global.synthMelody;"} },
		{ { "Rebalance the gain"," b ","b b",@"0.0 => Global.offbeatGain;[69,71,73,71] @=> Global.synthMelody2;[50,50,57,57] @=> Global.bassMelody;"}, {"Lower the key back down","3","key",@"[66,67,69,67] @=> Global.synthMelody;0.0 => Global.beatGain;"} },
		{ { "Set the Synth Melody","","aaddg",@"0.0 => Global.synthGain2;0.6 => Global.longSynthGain;"}, { "Rewire the setup", "-----", "14763", "0.0 => Global.synthGain;"} },
		{ { "Take a rest", "", "rest", ""}, { "Take a rest", "", "rest", ""} },
		{ { "End the piece", "", "end", "0.0 => Global.longSynthGain;0.0 => Global.bassGain;"}, { "End the piece", "", "end", ""} }
	};

	public GameObject step1;
	public GameObject mainBackground;

	public int playerNumber;

	ChuckInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;

	private MyStepController step1Script;

	private int currRound = 0;
	private float myPos;
	private float previousPos;
	private bool updatedRound;
	private bool tickerStarted;

	private Color32 correctColor;
	private Color32 normalBackgroundColor;

	// Use this for initialization
	void Start () {
		updatedRound = false;
		tickerStarted = false;
		myPos = 0.0f;
		previousPos = 0.0f;
		step1Script = step1.GetComponent<MyStepController> ();
		myChuck = GetComponent<ChuckInstance> ();
		myGetPosCallback = Chuck.CreateGetFloatCallback( GetPosCallback );

		correctColor = new Color32 (56,224,101,255);
		normalBackgroundColor = new Color32 (63, 56, 255, 255);
		string initialIntroGain;
		if (playerNumber == 0) {
			initialIntroGain = ".3";
		} else {
			initialIntroGain = "0.0";
		}
		myChuck.RunCode(@"
						public class Global {
							static float synthGain;
							static float longSynthGain;

							static float synthGain2;
			    			static float bassGain;
			    			static float beatGain;
			    			static float offbeatGain;

							static float introGain;

			    			static int synthMelody[];
			    			static int longSynthMelody[];

			    			static int synthMelody2[];
			    			static int bassMelody[];

						}
						external Event gotCorrect;
						external Event startTicker;

					 	8 => external float timeStep;
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

						[65,65,68,66] @=> Global.synthMelody;
						[65,70,68,67,67,72,70,69] @=> Global.longSynthMelody;

						[69,71,73,71] @=> Global.synthMelody2;
						[50,50,57,57] @=> Global.bassMelody;


						0 => Global.synthGain;
						0 => Global.longSynthGain;

						0 => Global.synthGain2;
						0 => Global.bassGain;
						0 => Global.beatGain;
						0 => Global.offbeatGain;

						SinOsc synth => ADSR e => Gain localSynthGain => dac;
						SinOsc synth2 => Gain localSynthGain2 => dac;
						SinOsc longSynth => Gain localLongSynthGain => dac;

						SinOsc bass => Gain localBassGain => dac;
						SinOsc beat => Gain localBeatGain => dac;
						SinOsc offbeat => Gain localOffbeatGain => dac;


						0 => synth.freq;
						0 => longSynth.freq;

						0 => synth2.freq;
						0 => bass.freq;
						0 => beat.freq;
						0 => offbeat.freq;	
	

						200::ms => e.attackTime;
						100::ms => e.decayTime;
						.5 => e.sustainLevel;
						200::ms => e.releaseTime;
						1 => e.keyOn;

						Gain localIntroGain;
						" + initialIntroGain + @" => Global.introGain;
						" + initialIntroGain + @" => localIntroGain.gain;
						//1 => int firstTime;

						fun void playIntroMelody(){
							// sound file
							me.sourceDir() + ""IntroMusicShort.wav"" => string filename;
							<<< filename >>>;
							if( me.args() ) me.arg(0) => filename;						
							// the patch 
							SndBuf buf => localIntroGain => dac;
							0 => buf.pos;

							// load the file
							filename => buf.read;

							buf.length() => now;	
						}
	
						fun void playMelody() {
							for (0 => int i; i < timeStep; i++) {
							    for (0 => int x; x < Global.synthMelody.cap(); x++)
							    {
							        Global.synthMelody[x] => Std.mtof => synth.freq;
							        125::ms => now;
							        0 => synth.freq;
							        125::ms => now;
							    }
							}
						}

						fun void playMelody2() {
						    for (0 => int i; i < timeStep; i++) {
						        for (0 => int x; x < Global.synthMelody2.cap(); x++)
						        {
						            Global.synthMelody2[x] => Std.mtof => synth2.freq;
						            125::ms => now;
						            0 => synth2.freq;
						            125::ms => now;
						        }
						    }
						}

						fun void playLongMelody() {
							for (0 => int i; i < timeStep/4; i++) {
							    for (0 => int x; x < Global.longSynthMelody.cap(); x++)
							    {
							        Global.longSynthMelody[x] => Std.mtof => longSynth.freq;
							        500::ms => now;
							    }
							}
						}
						
						fun void playBass() {
							for (0 => int i; i < timeStep; i++) {
							    for (0 => int x; x < Global.bassMelody.cap(); x++)
							    {
							        Global.bassMelody[x] => Std.mtof => bass.freq;
							        250::ms => now;
							    }
							}
						}

						fun void playBeat() {
							for (0 => int i; i < timeStep; i++) {
								for(0 => int x; x < 8 ; x++){
							        69 => Std.mtof => beat.freq;
							        (1000/6)::ms => now;
							        0 => beat.freq;
							        (1000/6)::ms => now;
								}
							}
						}

						fun void playOffbeat() {
							for (0 => int i; i < timeStep; i++) {
								for(0 => int x; x < 8 ; x++){
							        0 => offbeat.freq;
							        62.5::ms => now;
							        56 => Std.mtof => offbeat.freq;
							        62.5::ms => now;
								}	
							}
						}

					    TriOsc correct => Gain correctGain => dac;
						.07 => correctGain.gain;
						0 => correct.freq;

						//play if they get a step correct
						fun void playCorrect() {
							gotCorrect => now;
						    50 => Std.mtof => correct.freq;
						    100::ms => now;
						    53 => Std.mtof => correct.freq;
						    100::ms => now;
						    58 => Std.mtof => correct.freq;
						    100::ms => now;
							0 => correct.freq;
						}

						spork ~ playIntroMelody();
						startTicker => now;
								
						while( true )
						{
							Global.synthGain => localSynthGain.gain;
							Global.synthGain2 => localSynthGain2.gain;
							Global.longSynthGain => localLongSynthGain.gain;


							Global.bassGain => localBassGain.gain;
							Global.introGain => localIntroGain.gain;
							Global.beatGain => localBeatGain.gain;
							Global.offbeatGain => localOffbeatGain.gain;

							spork ~ updatePos();

							//ALL MUSIC PLAYS BELOW IN SEQUENCE

							//if(firstTime == 1){
							//	0 => firstTime;
							//	spork ~ playIntroMelody();
							//}

							spork ~ playMelody();
							spork ~ playBass();
							spork ~ playMelody2();
							spork ~ playLongMelody();

							spork ~ playBeat();
							spork ~ playOffbeat();
							spork ~ playCorrect();
							50::ms => now; //delay to make playCorrect not trigger twice
							timeStep::second => now;				
						}
					");
	}
	
	// Update is called once per frame
	void Update () {
		//getKey ();
		if (!updatedRound) {
			//update
			step1Script.stepInstructions = oneD(currRound,playerNumber);
			Debug.Log ("step 1 instruction set" + step1Script.stepInstructions[0]);
			step1Script.newRound = true;
			updatedRound = true;
		}

		if (step1Script.startTheTicker && !tickerStarted) {
			myChuck.BroadcastEvent ("startTicker");
			tickerStarted = true;
		}

		myChuck.GetFloat ("pos", myGetPosCallback);
		//full loop has passed!!!
		if (myPos >= previousPos + 0.95f) {
			
			//turn off elevator music after roudn 3 ALWAYS
			if (currRound == 3) {
				myChuck.RunCode ("0 => Global.introGain;");
			}
			//check if both steps done
			//if (step1Script.bottomDone == true && step1Script.topDone == true) {

				//check to only trigger new sounds in song if the current player actually had the step instructions to do so
				//if (specialWords [currRound, playerNumber, 1].Length != 0 || specialWords [currRound, playerNumber, 2].Length != 0) {
					myChuck.BroadcastEvent ("gotCorrect");
					myChuck.RunCode(specialWords [currRound, playerNumber, 3]);
				//}
			//}
		}

		if (myPos >= previousPos + 1.0f) {
			previousPos = previousPos + 1.0f;

			if (currRound < specialWords.GetLength(0)) {
				currRound++;
				updatedRound = false;
			}
			Debug.Log ("Current Round: " + currRound);
		}
		float distanceMultiplier = 1.5f;
		step1Script.linePos = (myPos - previousPos)*distanceMultiplier;

		// Background updates when instructions are complete
		if (step1Script.bottomDone == true && step1Script.topDone == true) {
			mainBackground.GetComponent<Renderer> ().material.color = correctColor;
		} else {
			mainBackground.GetComponent<Renderer> ().material.color = normalBackgroundColor;
		}

	}

	string[] oneD(int index1, int index2) {
		string[] oneDArray = new string[4];
		oneDArray [0] = specialWords [index1, index2, 0];
		oneDArray [1] = specialWords [index1, index2, 1];
		oneDArray [2] = specialWords [index1, index2, 2];
		oneDArray [3] = specialWords [index1, index2, 3];

		return oneDArray;
	}
		

	void GetPosCallback( System.Double pos )
	{
		myPos = (float) pos;
	}
}

