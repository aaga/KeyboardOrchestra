using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	//Instruciton Text, input Text 1, input Text 2 (* signals no key placed at that location), chuck code (LEVEL is a code to trigger a new level animation)
	private string[,,] specialWords = new string[,,] { 
		{ { "","*","*","SETUP"}, { "", "*", "*", "SETUP"} },//new level
		{ { "Welcome to the Keyboard Orchestra. Type start to begin.", "", "start", "" }, {"Welcome to the Keyboard Orchestra. Type start to begin.", "", "start", "" } },

		{ { "You are player...", "", "one", ""}, { "You are player...", "", "two", ""} },
		{ { "You can also play your partner's keyboard!", "two", "", ""}, { "You can also play your partner's keyboard!", "one", "", ""} },
		{ { "Sometimes you have to press two keys at once", "a", "a", ""}, { "Sometimes you have to press two keys at once", "b", "team", ""} },
		{ { "Ready to get Started?", "", "ready", ""}, {"Ready to get Started?", "", "ready", ""} },

		{ { "","","","LEVEL"}, { "", "", "", "LEVEL"} },//new level
		{ { "You must type to the beat of the music!", "", "ok", ""}, {"You must type to the beat of the music!", "", "ok", ""} },

		{ { "","","","LEVEL"}, { "", "", "", "LEVEL"} },//new level
		{ { "", "", "test", ""}, {"Waiting for next instruction...", "", "", ""} },
		{ { "","","hello",""}, { "", "", "wow", ""} },
		{ { "", "", "sup", ""}, {"", "", "; ;", ""} },
		{ { "","","word",""}, { "", "", "roof", ""} },
		{ { "","","lol",""}, {"","","key",""} },

		{ { "","","","LEVEL"}, { "", "", "", "LEVEL"} },//new level
		{ { "","","gg", ""}, { "", "", "135",  ""} },
		{ { "","**a","hip", ""}, { "", "*0", "10",  ""} },
		{ { "","*u*","fun", ""}, { "", "**o", "go",  ""} },
		{ { "","","wow", ""}, { "", "ge", "ge",  ""} },
		{ { "","0","good", ""}, { "", "we", "water",  ""} },

		{ { "","","","LEVEL"}, { "", "", "", "LEVEL"} },//new level
		{ { "","0","holy", ""}, { "", "the", "art",  ""} },
		{ { "","*aa","jeez", ""}, { "", "**jk", "hello",  ""} },
		{ { "","*nn","fun", ""}, { "", "1357", "1357",  ""} },
		{ { "","fun","fun", ""}, { "", "7531", "1357",  ""} },
		{ { "","**l","ok", ""}, { "", "1111", "once",  ""} },

		{ { "","","","LEVEL"}, { "", "", "", "LEVEL"} },//new level
		{ { "","fun","holy", ""}, { "", "the", "art",  ""} },
		{ { "","a*b*c","de", ""}, { "", "", "1357",  ""} },
		{ { "","*nn","fun", ""}, { "", "a*b*c", "*d*e*",  ""} },
		{ { "","nuf","fun", ""}, { "", "qwe", "asdf",  ""} },
		{ { "","ifl","ok", ""}, { "", "****y", "qwert",  ""} },

		{ { "", "", "rest", ""}, { "", "", "rest", ""} },
		{ { "", "", "end", "0.0 => Global.longSynthGain;0.0 => Global.bassGain;"}, { "", "", "end", ""} }
	};

	public GameObject step1;
	public GameObject mainBackground;
	public GameObject setupScreen;

	//Level Animation Objects and scripts
	public GameObject leftLevel;
	private LevelController leftLevelScript;
	public GameObject rightLevel;
	private LevelController rightLevelScript;
	public GameObject levelText;
	private TextMesh levelMesh;
	private bool levelAnimationDone;

	private bool setupDone;

	public int playerNumber;

	public int timestep;

	ChuckInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;
	Chuck.FloatCallback myGetBeatCallback;

	private MyStepController step1Script;

	//Ticker
	public int currRound;
	public float myPos;
	public float previousPos;
	private bool updatedRound;
	private bool tickerStarted;
	private bool alreadyCorrect;
	public int staticLevel;
	public float myBeat;

	private int currKey = 41;
	private int currStepInLevel;

	//ALL colors
	private Color32 correctColor;
	private Color32 normalBackgroundColor;
	private Color32 failBackgroundColor;


	// Initialization
	void Start () {
		updatedRound = false;
		tickerStarted = false;
		alreadyCorrect = false;
		myPos = 0.0f;
		previousPos = 0.0f;
		step1Script = step1.GetComponent<MyStepController> ();
		myChuck = GetComponent<ChuckInstance> ();
		myGetPosCallback = Chuck.CreateGetFloatCallback( GetPosCallback );
		myGetBeatCallback = Chuck.CreateGetFloatCallback( GetBeatCallback );

		currRound = 0;
		staticLevel = 0;

		leftLevelScript = leftLevel.GetComponent<LevelController> ();
		rightLevelScript = rightLevel.GetComponent<LevelController> ();

		levelMesh = (TextMesh)levelText.GetComponent(typeof(TextMesh));

		//set colors
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
						external Event gotCorrect;
						external Event startTicker;

						external Event keyFailTrigger;
						external Event endIntroMusic;

					 	" + timestep + @" => external int timeStep;
						external float pos;
						external float count;

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

						Gain localIntroGain;
						" + initialIntroGain + @" => localIntroGain.gain;

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

						class Chord {
						    
						    5 => int size;
						    
						    BlitSquare osc[5];
						    ADSR adsr[5];
						    
						    Gain g => dac;
						    
						    50::ms => dur attack;
						    25::ms => dur decay;
						    0.5 => float sustain;
						    25::ms => dur release;
						    
						    0.03 => g.gain;
						    
						    for (0 => int i; i < size; i++) {
						        osc[i] => adsr[i];
						        adsr[i] => g;
						        adsr[i].set(attack, decay, sustain, release);
						    }
						    
						    public void play(int num, int note) {
						        Std.mtof(note) => osc[num].freq;
						        1 => adsr[num].keyOn;
						    }
						    
						    public void softOff() {
						        for (0 => int i; i < size; i++) {
						            1 => adsr[i].keyOff;
									100::ms => now;
						        }
						    }
						    
						    public void hardOff() {
						        for (0 => int i; i < size; i++) {
						            0::ms => adsr[i].releaseTime;
						        }
						        softOff();
						        for (0 => int i; i < size; i++) {
						            release => adsr[i].releaseTime;
						        }
						    }
						    
						}

						class DrumSet {
						    // define hihat
						    Shakers hhs => JCRev r;
						    .025 => r.mix;
						    Std.mtof( 76 ) => hhs.freq;
						    
						    // Define Bassdrum
						    SinOsc s => ADSR bda;
						    80 => s.freq;
						    (0::ms, 10::ms, 0.0, 0::ms ) => bda.set;
						    
						    // define snare drum
						    Noise n => ADSR sna => Gain g;
						    0.15 => g.gain;
						    (0::ms, 25::ms, 0.0, 0::ms) => sna.set;
						    
						    
						    public void connect( UGen ugen ) {
						        r => ugen;
						        bda => ugen;
						        g => ugen;
						    }
						    
						    public void hh() {
						        1 => hhs.noteOn;
						    }
						    
						    public void bd() {
						        1 => bda.keyOn;
						    }
						    
						    public void sn() {
						        1 => sna.keyOn;
						    }
						}

						class Bass {
						    // BASS
						    SawOsc sb => LPF filt => ADSR a => Gain g2;
						    440 => filt.freq;
						    0.3 => filt.Q;
						    0.0 => g2.gain;
						    (10::ms, 45::ms, 0.5, 40::ms) => a.set; // Set ADSR envelope
						    
						    public void connect( UGen u ) {
						        g2 => u;
						    }
						    
						    public void bass( int tone ) {
						        Std.mtof( tone ) =>  sb.freq;
						        0.3 => g2.gain;
						        1 => a.keyOn;
						        125::ms => now;
						        1=> a.keyOff;
						    }
						}

						class BassDrumLoop {
						    
							Gain bdlGain => dac;
							bdlGain.gain(1);
						    DrumSet drm;
						    drm.connect( bdlGain );
						    
						    Bass bass;
						    bass.connect( bdlGain );
						    
						    [ 41, 41, 44, 46] @=> int bline[];
						    0 => int pos;
						    0 => count;
						    
						    250::ms => dur length;
						    
						    public void setLength(dur l) {
						        l => length;
						    }
						    
						    public void reset() {
						        0 => pos;
						        0 => count;
						    }
						    
						    public void setKey(int key) {
						        [ key, key, key + 3, key + 5] @=> bline;
						    }

							public void setGain(float gainToSet) {
							        bdlGain.gain(gainToSet);
							}

						    public void stop() {
								<<< ""trying to stop"" >>>;
						        setGain(0);
						    }
						    
						    public void play() {
						        while ( true ) {
						            drm.hh();
						            if ( count % 2 == 0 ) { drm.bd(); }
						            if ( count % 4 == 2 ) { drm.sn(); }
						            
						            if ( count % 2 == 0 ) { spork ~ bass.bass( bline[ pos % 4 ]); }
						            if ( count % 2 == 1 ) { spork ~ bass.bass( 12 + bline[ pos % 4 ]); }
						            
						            
						            1 + count => count;
						            if ( count % 4 == 0 ) { 1 + pos => pos; }
						            length => now;
						        }
						    }
						    
						}
						public class Global {
							static BassDrumLoop @ bdl;
							static Chord @ chord;
						}

						new BassDrumLoop @=> Global.bdl;
						new Chord @=> Global.chord;

						startTicker => now;
						while( true )
						{
							spork ~ updatePos();
							// spork ~ playCorrect();
							// 50::ms => now; //delay to make playCorrect not trigger twice
							timeStep::second => now;
						}						
					");
	}
	
	//RUN EVERY FRAME
	void Update () {

		//UPDATE SCREEN EACH ROUND
		if (!updatedRound) {
			step1Script.stepInstructions = oneD(currRound,playerNumber);
			Debug.Log ("step 1 instruction set" + step1Script.stepInstructions[0]);
			step1Script.newRound = true;
			updatedRound = true;
		}

		//START TICKER
		if (step1Script.startTheTicker && !tickerStarted) {
			myChuck.BroadcastEvent ("startTicker");
			tickerStarted = true;
		}

		myChuck.GetFloat ("pos", myGetPosCallback);
		myChuck.GetFloat ("count", myGetBeatCallback);


		//TRIGGER NEW LEVEL ANIMATION
		if (specialWords [currRound, playerNumber, 3] == "LEVEL") {

			if (!leftLevelScript.doneAnimation && !rightLevelScript.doneAnimation) {
				leftLevelScript.startAnimation = true;
				rightLevelScript.startAnimation = true;
			} else {
				//up the key of the music
				if (currRound > 6) {
					currKey++;
					setKey (currKey);
					currStepInLevel = 1;
				}

				//up the speed not on the first time
				if (currRound > 7) {
					offChord ();
					timestep--;
					myChuck.SetInt ("timeStep", timestep);
				}

				//RESET ALL VARIABLES TO INITIAL POSITION
				leftLevelScript.doneAnimation = false;
				rightLevelScript.doneAnimation = false;
				leftLevelScript.startAnimation = false;
				rightLevelScript.startAnimation = false;
				leftLevelScript.closeDone = false;
				rightLevelScript.closeDone = false;
				leftLevelScript.openDone = false;
				rightLevelScript.openDone = false;

				currRound++;
				updatedRound = false;
				Debug.Log ("done with level animation");
				levelAnimationDone = true;				//weird bool to make level after the animation not skip
			}
		} else if (specialWords [currRound, playerNumber, 3] == "SETUP") {
			setupScreen.SetActive (true);
			if (setupScreen.GetComponent<SetupController> ().setupDone) {
				currRound++;
				updatedRound = false;
				Debug.Log ("done with setup");
				setupScreen.SetActive (false);
				setupDone = true;				//weird bool to make level after the animation not skip
			}
		} else {

			//USER IS DONE WITH STEP BEFORE END OF TRIGGER
			if (myPos >= previousPos && step1Script.bottomDone == true && step1Script.topDone == true && (!alreadyCorrect || step1Script.goToNextStep)) {

				Debug.Log ("alreadyCorrect: " + alreadyCorrect + " goToNextStep: " + step1Script.goToNextStep);
				//REWARD BY DECREASING STATIC
				if (staticLevel > 0) {
					staticLevel--;
				}
				step1Script.updateStaticBar (staticLevel);
				alreadyCorrect = true;

				//immediacy (brings to end of round)
				if (step1Script.goToNextStep || currRound == 1) {	//BUG: remove hacky currRound Check
					Debug.Log ("go to next round");
					previousPos = myPos - 1;
					step1Script.goToNextStep = false;
				}
			}

			//IF ticker gets to end of screen
			if (myPos >= previousPos + 1.0f) {
				alreadyCorrect = false;

				//turn off intro music
				if (currRound == 3) {
					if (playerNumber == 0) {
						myChuck.BroadcastEvent ("endIntroMusic");
					}
				}

				resetBassline ();
				Debug.Log ("currRound in Main: " + currRound);

				//user got it wrong
				if (step1Script.bottomDone != true || step1Script.topDone != true) {
					myChuck.BroadcastEvent ("keyFchrodailTrigger");
					staticLevel++;
				}
				step1Script.updateStaticBar (staticLevel);

				previousPos = previousPos + 1.0f;

				//move on to next round
				if (currRound < specialWords.GetLength (0) && !levelAnimationDone && !setupDone) {
					currRound++;
					int currLevelText = (currRound - 2) / 5;
					if (currRound == 6) {
						levelMesh.text = "SURPRISE";
					} else {
						levelMesh.text = "Level " + currLevelText.ToString ();

					}
					updatedRound = false;
					playSelectChord ();

				}
				levelAnimationDone = false;
				setupDone = false;
				Debug.Log ("Current Round: " + currRound);

				if (currRound == 7) {
					startBassline ();
				}
			}
			float distanceMultiplier = 1.5f;
			step1Script.linePos = (myPos - previousPos) * distanceMultiplier;

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
					
					//mainBackground.GetComponent<Renderer> ().material.color = normalBackgroundColor;
				}
			}
		}
	}

	public void startBassline() {
		Debug.Log ("starting bassline");
		myChuck.RunCode ("Global.bdl.play();");
	}

	public void resetBassline() {
		myChuck.RunCode ("Global.bdl.reset();");
	}

	public void stopBassline() {
		myChuck.RunCode ("Global.bdl.stop();");
	}

	public void setLength(float milliseconds) {
		milliseconds /= 16.0f;
		myChuck.RunCode ("Global.bdl.setLength(" + milliseconds + "::ms);");
	}

	public void setKey(int key) {
		myChuck.RunCode ("Global.bdl.setKey(" + key + ");");
	}

	public void playChordNote(int keyNum, int note) {
		myChuck.RunCode ("Global.chord.play(" + keyNum + "," + note + ");");
	}

	public void offChord() {
		myChuck.RunCode ("Global.chord.softOff();");
	}
	
	public void playSelectChord() {
		if (currRound >= 9 && step1Script.bottomDone == true && step1Script.topDone == true) {
			int chordNote = 0;
			if (currStepInLevel == 1) {
				playChordNote (0, currKey);
			} else if (currStepInLevel == 2) {
				playChordNote (1, currKey + 3);
			} else if (currStepInLevel == 3) {
				playChordNote (2, currKey + 7);
			} else if (currStepInLevel == 4) {
				playChordNote (3, currKey + 12);
			} else if (currStepInLevel == 5) {
				playChordNote (4, currKey + 24);
			}
			currStepInLevel++;
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

	void GetBeatCallback( System.Double beat )
	{
		myBeat = (float) beat;
	}
}

