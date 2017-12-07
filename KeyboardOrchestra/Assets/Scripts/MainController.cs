using System.Collections;
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
		{ { "", "", "bass", "0.5 => Global.bassGain;"}, {"Waiting for next instruction...", "", "", ""} },
		{ { "","in","in",""}, { "", "d*a", "acec", "0.7 => Global.synthGain;"} },
		{ { "", "2", "cefe", @"0.4 => Global.synthGain2;"}, {"", ";;;", "; ;", "0.3 => Global.tripletGain;0.4 => Global.synthGain;"} },
		{ { "","7","key",@"[70,72,74,72] @=> Global.synthMelody2;[51,51,58,58] @=> Global.bassMelody;"}, { "", "99", "roof", "[67,68,70,68] @=> Global.synthMelody;"} },
		{ { ""," b ","b b",@"[69,71,73,71] @=> Global.synthMelody2;[50,50,57,57] @=> Global.bassMelody;"}, {"","3","key",@"[66,67,69,67] @=> Global.synthMelody;0.0 => Global.tripletGain;"} },
		{ { "","","aaddg",@"0.0 => Global.synthGain2;0.6 => Global.longSynthGain;"}, { "", "---", "1357", "0.0 => Global.synthGain;"} },
		{ { "", "", "rest", ""}, { "", "", "rest", ""} },
		{ { "", "", "end", "0.0 => Global.longSynthGain;0.0 => Global.bassGain;"}, { "", "", "end", ""} }
	};

	public GameObject step1;
	public GameObject mainBackground;

	public int playerNumber;

	public int timestep;

	ChuckInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;

	private MyStepController step1Script;

	private int currRound;
	private float myPos;
	private float previousPos;
	private bool updatedRound;
	private bool tickerStarted;
	private bool alreadyCorrect;
	private int staticLevel;

	private Color32 correctColor;
	private Color32 normalBackgroundColor;
	private Color32 failBackgroundColor;


	// Use this for initialization
	void Start () {
		updatedRound = false;
		tickerStarted = false;
		alreadyCorrect = false;
		myPos = 0.0f;
		previousPos = 0.0f;
		step1Script = step1.GetComponent<MyStepController> ();
		myChuck = GetComponent<ChuckInstance> ();
		myGetPosCallback = Chuck.CreateGetFloatCallback( GetPosCallback );
		currRound = 0;
		staticLevel = 0;

		timestep = 8;

		correctColor = new Color32 (56,224,101,255);
		normalBackgroundColor = new Color32 (63, 56, 255, 255);
		failBackgroundColor = new Color32 (255, 64, 89, 255);
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
			    			static float tripletGain;

							static float introGain;

			    			static int synthMelody[];
			    			static int longSynthMelody[];

			    			static int synthMelody2[];
			    			static int bassMelody[];

						}
						external Event gotCorrect;
						external Event startTicker;

						external Event keyFailTrigger;
						external Event endIntroMusic;

					 	8 => external int timeStep;
						8.0 => float masterTimer;
						external float pos;

						fun void updatePos() {
							timeStep::second => dur currentTimeStep;
							currentTimeStep / 1000 => dur deltaTime;
							now => time startTime;
							
							pos => float originalPos;
							<<<timeStep>>>;				
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
						0 => Global.tripletGain;

						SinOsc synth => ADSR e => Gain localSynthGain => dac;
						SinOsc synth2 => Gain localSynthGain2 => dac;
						SinOsc longSynth => Gain localLongSynthGain => dac;

						SinOsc bass => Gain localBassGain => dac;
						SinOsc triplet => Gain localTripletGain => dac;

						0 => synth.freq;
						0 => longSynth.freq;

						0 => synth2.freq;
						0 => bass.freq;
						0 => triplet.freq;	

						200::ms => e.attackTime;
						100::ms => e.decayTime;
						.5 => e.sustainLevel;
						200::ms => e.releaseTime;
						1 => e.keyOn;

						Gain localIntroGain;
						" + initialIntroGain + @" => Global.introGain;
						" + initialIntroGain + @" => localIntroGain.gain;
						//1 => int firstTime;

						fun void fadeIntro(){
							endIntroMusic => now;
							.5 => float tempGain;
							while(tempGain >= 0.0){
								tempGain - .01 => tempGain;
								tempGain => localIntroGain.gain;
								150::ms => now;
							}
						}

						fun void playIntroMelody(){
							// sound file
							me.sourceDir() + ""IntroMusicShort.wav"" => string filename;
							<<< filename >>>;
							if( me.args() ) me.arg(0) => filename;						
							// the patch 
							SndBuf buf => localIntroGain => dac;
							0 => buf.pos;

							filename => buf.read;
							spork ~ fadeIntro();
							buf.length() => now;	
						}
	
						fun void playMelody() {
							for (0 => int i; i < masterTimer; i++) {
							    for (0 => int x; x < Global.synthMelody.cap(); x++)
							    {
									Global.synthGain => localSynthGain.gain;
							        Global.synthMelody[x] => Std.mtof => synth.freq;
							        ((timeStep/masterTimer)*1000/Global.synthMelody.cap()/2)::ms => now;
							        0 => synth.freq;
							        ((timeStep/masterTimer)*1000/Global.synthMelody.cap()/2)::ms => now;
							    }
							}
						}

						fun void playMelody2() {
						    for (0 => int i; i < masterTimer; i++) {
						        for (0 => int x; x < Global.synthMelody2.cap(); x++)
						        {
									Global.synthGain2 => localSynthGain2.gain;
						            Global.synthMelody2[x] => Std.mtof => synth2.freq;
							        ((timeStep/masterTimer)*1000/Global.synthMelody2.cap()/2)::ms => now;
						            0 => synth2.freq;
							        ((timeStep/masterTimer)*1000/Global.synthMelody2.cap()/2)::ms => now;
						        }
						    }
						}

						fun void playLongMelody() {
							for (0 => int i; i < masterTimer; i++) {
							    for (0 => int x; x < Global.longSynthMelody.cap(); x++)
							    {
									Global.longSynthGain => localLongSynthGain.gain;
							        Global.longSynthMelody[x] => Std.mtof => longSynth.freq;
							        ((timeStep/masterTimer)*1000/Global.longSynthMelody.cap())::ms => now;
							    }
							}
						}
						
						fun void playBass() {
							for (0 => int i; i < masterTimer; i++) {
							    for (0 => int x; x < Global.bassMelody.cap(); x++)
							    {
									Global.bassGain => localBassGain.gain;
							        Global.bassMelody[x] => Std.mtof => bass.freq;
							        ((timeStep/masterTimer)*1000/Global.bassMelody.cap())::ms => now;
							    }
							}
						}

						fun void playTriplet() {
							for (0 => int i; i < masterTimer; i++) {
								for(0 => int x; x < 3 ; x++){
									Global.tripletGain => localTripletGain.gain;
							        69 => Std.mtof => triplet.freq;
							        ((timeStep/masterTimer)*1000/3/2)::ms => now;
							        0 => triplet.freq;
							        ((timeStep/masterTimer)*1000/3/2)::ms => now;
								}
							}
						}

						//play if they get a step correct
						fun void playCorrect() {
							gotCorrect => now;
							me.sourceDir() + ""keyDown.wav"" => string filename;
							if( me.args() ) me.arg(0) => filename;						
							SndBuf buf => localIntroGain => dac;
							0 => buf.pos;
							filename => buf.read;
							buf.length() => now;	
						}

						fun void keyFailSound(){
							while( true )
							{
								keyFailTrigger => now;
								me.sourceDir() + ""fail.wav"" => string filename;
								if( me.args() ) me.arg(0) => filename;						
								SndBuf buf => dac;
								0 => buf.pos;
								filename => buf.read;
								buf.length() => now;
							}
						}

						spork ~ keyFailSound();
						spork ~ playIntroMelody();
						startTicker => now;
								
						while( true )
						{
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

							spork ~ playTriplet();
							spork ~ playCorrect();
							50::ms => now; //delay to make playCorrect not trigger twice
							timeStep::second => now;
						}
					");
		myChuck.SetInt ("timeStep", timestep);

	}
	
	// Update is called once per frame
	void Update () {
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

		//USER IS DONE WITH STEP
		if (myPos >= previousPos + 0.05f && step1Script.bottomDone == true && step1Script.topDone == true && !alreadyCorrect) {

			myChuck.RunCode (specialWords [currRound, playerNumber, 3]);
			if (staticLevel > 0) {
				staticLevel--;
			}
			step1Script.updateStaticBar (staticLevel);
			alreadyCorrect = true;

			previousPos = myPos - 1;
		}

		if (myPos >= previousPos + 1.0f) {
			alreadyCorrect = false;

			//turn off intro music
			if (currRound == 3) {
				if (playerNumber == 0) {
					myChuck.BroadcastEvent ("endIntroMusic");
				}
//				myChuck.RunCode ("0 => Global.introGain;");
			}
			//make faster
			if (currRound >= 5) {
				if (currRound % 2 == 0) {
					timestep--;
					myChuck.SetInt ("timeStep", timestep);
				}
			}

			if (step1Script.bottomDone != true || step1Script.topDone != true) {
				myChuck.BroadcastEvent ("keyFailTrigger");
				staticLevel++;
			}
			step1Script.updateStaticBar (staticLevel);

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
			//flash screen red if incorrect at end!
			if (myPos >= previousPos + 0.96f) {
				if (step1Script.bottomDone != true || step1Script.topDone != true) {
					mainBackground.GetComponent<Renderer> ().material.color = failBackgroundColor;
				}
			} else {
				
				mainBackground.GetComponent<Renderer> ().material.color = normalBackgroundColor;
			}
					
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

