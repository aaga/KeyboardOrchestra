using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MyStepController : MonoBehaviour {

	private TextMesh instructionMesh;
	private string[,] acceptableKeys = new string[,] { { "A", "a", "0"}, { "B", "b", "0"}, { "C", "c", "0"}, { "D", "d", "0"}, { "E", "e", "0"}, { "F", "f", "0"}, { "G", "g", "0"}, { "H", "h", "0"}, { "I", "i", "0"}, { "J", "j", "0"}, { "K", "k", "0"}, { "L", "l", "0"}, { "M", "m", "0"}, { "N", "n", "0"}, { "O", "o", "0"}, { "P", "p", "0"}, { "Q", "q", "0"}, { "R", "r", "0"}, { "S", "s", "0"}, { "T", "t", "0"}, { "U", "u", "0"}, { "V", "v", "0"}, { "W", "w", "0"}, { "X", "x", "0"}, { "Y", "y", "0"}, { "Z", "z", "0"}, { "Alpha0", "0", "0"}, { "Alpha1", "1", "0"}, { "Alpha2", "2", "0"}, { "Alpha3", "3", "0"}, { "Alpha4", "4", "0"}, { "Alpha5", "5", "0"}, { "Alpha6", "6", "0"}, { "Alpha7", "7", "0"}, { "Alpha8", "8", "0"}, { "Alpha9", "9", "0"}, { "Alpha00", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", " ", "1"}};
	public GameObject instructionText;
	public GameObject inputText;
	public GameObject mainBackground;

	public GameObject main;
	private MainController mainScript;

	public GameObject ticker;
	private int currLetter;

	private GameObject currKeysTop;
	private GameObject currKeysBottom;
	private bool pressTop;
	private bool pressBottom;
	private int doubleWhammy; // 0 is off, 1 is top already pressed, 2 is bottom already pressed
	public bool topDone;
	public bool bottomDone;
	public bool otherReady;
	public bool startTheTicker;
	private bool sent1000;
	public bool goToNextStep;

	private Color32 topColor;
	private Color32 bottomColor;
	private Color32 correctColor;

	private float initialTickerX;

	public string[] stepInstructions;
	public bool newRound;
	public float linePos;

	public float volumeCount;

	// ChucK instance
	ChuckInstance myChuck;
	Chuck.IntCallback myGetIntCallback;
	Chuck.VoidCallback myCallback;

	private int message;
	private bool newMessage;

	// Use this for initialization
	void Start () {
		newRound = true;
		otherReady = false;
		startTheTicker = false;
		sent1000 = false;
		goToNextStep = false;
		instructionMesh = (TextMesh)instructionText.GetComponent(typeof(TextMesh));

		currLetter = 0;
		linePos = 0.0f;

		topColor = new Color32(255, 56, 129,255);
		bottomColor = new Color32(63, 56, 255,255);
		correctColor = new Color32 (56,224,101,255);

		myChuck = GetComponent<ChuckInstance> ();
		myGetIntCallback = Chuck.CreateGetIntCallback( GetIntCallback );
		myCallback = Chuck.CreateVoidCallback( NewMessageReceived );
		message = -1;
		newMessage = false;

		mainScript = main.GetComponent<MainController> ();

		initialTickerX = ticker.transform.position.x;

		for (int i = 0; i < acceptableKeys.GetLength(0); i++) {
			acceptableKeys[i,2] = (i+72).ToString();
		}
		//set space to none
		acceptableKeys[acceptableKeys.GetLength(0) -1,2] = "0";

		//create container to have new keys added
		currKeysTop = new GameObject("currKeysTop");
		currKeysTop.AddComponent<MeshFilter>();
		currKeysTop.AddComponent<MeshRenderer>();
		currKeysBottom = new GameObject("currKeysBottom");
		currKeysBottom.AddComponent<MeshFilter>();
		currKeysBottom.AddComponent<MeshRenderer>();
	}

	//Change the amount of Red (punishment) on ticker
	public void updateStaticBar(int level) {
		if (level >= ticker.transform.childCount) {
			level = ticker.transform.childCount;
		}
		for (int i = 0; i < level; i++) {
			ticker.transform.GetChild (i).gameObject.SetActive (true);
		}

		for (int i = level; i < ticker.transform.childCount; i++) {
			ticker.transform.GetChild (i).gameObject.SetActive (false);
		}

		myChuck.SetFloat ("staticGain", (float)level / (float) 250);
		myChuck.BroadcastEvent ("staticLevelUpdated");
	}

	public void runTheChuck(string otherIP, string receivingPort, string sendingPort) {
		Debug.Log (otherIP + receivingPort + sendingPort);
		myChuck.RunCode(
			@"

			0 => external int messageToSend;
			0 => external int messageReceived;
			0.0 => external float staticGain;
			external Event sendMessage;
			external Event notifier;

			//event for sound of key down and up
			external Event keyDownTrigger;
			external Event keyUpTrigger;
			external Event keyCorrectTrigger;

			//event for static updated
			external Event staticLevelUpdated;

			// address of other computer
			""" + otherIP + @""" => string hostname;
			// sending on port
			" + sendingPort + @" => int port;

			// check command line
			//if( me.args() ) me.arg(0) => hostname;
			//if( me.args() > 1 ) me.arg(1) => Std.atoi => port;

			// send object
			OscSend xmit;

			// aim the transmitter
			xmit.setHost( hostname, port );

			fun void sendIntMessage(int messageInt) {
    			// start the message...
    			// the type string 'i' expects an int
    			xmit.startMsg( ""/keyboardOrchestra/keyPressInfo"", ""i"" );
    
    			// a message is kicked as soon as it is complete 
    			// - type string is satisfied and bundles are closed
    			messageInt => xmit.addInt;
			}

			// create our OSC receiver
			OscRecv recv;
			// receiving on port
			" + receivingPort + @" => recv.port;
			// start listening (launch thread)
			recv.listen();

			// create an address in the receiver, store in new variable
			recv.event( ""/keyboardOrchestra/keyPressInfo, i"" ) @=> OscEvent @ oe;

			fun void receiveIntMessage () {
    			// infinite event loop
    			while( true )
    			{
        			// wait for event to arrive
        			oe => now;
        
        			// grab the next message from the queue. 
        			while( oe.nextMsg() )
        			{        
            			// getInt fetches the expected int (as indicated by ""i"")
            			oe.getInt() => messageReceived;
            			notifier.broadcast();
            
            			// print
            			<<< ""got (via OSC):"", messageReceived >>>;
        			}
    			}
			}

			spork ~ receiveIntMessage();

			fun void handleOSC(){
				while( true )
				{
	    			sendMessage => now;
	    			spork ~ sendIntMessage(messageToSend);

				}
			}

			fun void keyDownSound(){
				while( true )
				{
					keyDownTrigger => now;
					me.sourceDir() + ""keyDown.wav"" => string filename;
					if( me.args() ) me.arg(0) => filename;						
					SndBuf buf => dac;
					0 => buf.pos;
					filename => buf.read;
					buf.length() => now;
				}
			}

			fun void keyUpSound(){
				while( true )
				{
					keyUpTrigger => now;
					me.sourceDir() + ""keyUp.wav"" => string filename;
					if( me.args() ) me.arg(0) => filename;						
					SndBuf buf => dac;
					0 => buf.pos;
					filename => buf.read;
					buf.length() => now;
				}
			}

			fun void keyCorrectSound(){
				while( true )
				{
					keyCorrectTrigger => now;
					me.sourceDir() + ""keyCorrect.wav"" => string filename;
					if( me.args() ) me.arg(0) => filename;						
					SndBuf buf => dac;
					0 => buf.pos;
					filename => buf.read;
					buf.length() => now;
				}
			}
	
			Noise n => Gain localStaticGain => dac;
			0.0 => localStaticGain.gain;

			fun void playStatic() {
				while( true ) {
					staticLevelUpdated => now;
					staticGain => localStaticGain.gain;
				}
			}

			spork ~ handleOSC();
			spork ~ keyDownSound();
			spork ~ keyUpSound();
			spork ~ keyCorrectSound();
			spork ~ playStatic();

			while(true){
				1::second => now;
			}

			"
		);
		myChuck.StartListeningForChuckEvent( "notifier", myCallback );
	}

	//RUN ONCE EVERY FRAME
	void Update () {

		if (newMessage) {
			if (message == 1000) {
				otherReady = true;
			} else if (message >= 100) {
				doKeyDown (message - 100, false);
			} else {
				doKeyUp (message, false);
			}
			newMessage = false;
		}

		//show new display instructions each round
		if (newRound) {
			sent1000 = false;
			goToNextStep = false;
			currLetter = 0;

			volumeCount = 0;
			if (stepInstructions.GetLength (0) != 0) {
				instructionMesh.text = ResolveTextSize (stepInstructions [0], 35);
				addGraphicKeys (stepInstructions [1], 6f, currKeysTop,"top");
				addGraphicKeys (stepInstructions [2], -1f, currKeysBottom,"bottom");
			}
			newRound = false;

			//no keys pressed
			pressTop = false;
			pressBottom = false;
			topDone = false;
			bottomDone = false;
		}

		Vector3 temp = ticker.transform.position;
		temp.x = initialTickerX + linePos * 25f;
		ticker.transform.position = temp;

		//do the interaction
		getKey();

		//if received instructions, and both are completed, and both existed in the first place
		if (stepInstructions.Length > 0 && currLetter >= stepInstructions [1].Length  && currLetter >= stepInstructions [2].Length) {
			topDone = true;
			bottomDone = true;

			// send message if havent sent before
			if (!sent1000) {
				myChuck.SetInt ("messageToSend", 1000);
				myChuck.BroadcastEvent ("sendMessage");
				sent1000 = true;
			}

			//both people done
			if (otherReady) {
				otherReady = false;
				startTheTicker = true;
				goToNextStep = true;
			}
		}

	}

	//Create a visual key for each letter in the instructions
	void addGraphicKeys(string inputWord, float yPos, GameObject currKeys, string whichComputer){
		//add all new key instructions
		foreach (Transform child in currKeys.transform) {
			GameObject.Destroy(child.gameObject);
		}

		float startX = -12f;
		foreach (char c in inputWord)
		{
			//"*" is code word to use for no actual key in a place
			if (c != '*') {
				GameObject newKey = (GameObject)Instantiate (Resources.Load ("Default Key"));
				TextMesh newKeyText = (TextMesh)newKey.transform.GetChild (0).GetComponent (typeof(TextMesh));
				newKeyText.text = c.ToString ();

				newKey.transform.position = new Vector3 (startX, yPos, -7f);
				newKey.transform.GetChild (0).transform.position = new Vector3 (newKey.transform.GetChild (0).transform.position.x, newKey.transform.GetChild (0).transform.position.y, -2f);
				newKey.transform.GetChild (1).transform.position = new Vector3 (newKey.transform.GetChild (1).transform.position.x, newKey.transform.GetChild (1).transform.position.y, -1f);
				newKey.transform.GetChild (2).transform.position = new Vector3 (newKey.transform.GetChild (2).transform.position.x, newKey.transform.GetChild (2).transform.position.y, 0f);

				newKey.transform.parent = currKeys.transform;
			
				if (whichComputer == "top") {
					newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = topColor;
				} else {
					newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = bottomColor;
				}
			} 
			//need to add a fake gameobject to the parent to keep the numbering consistent
			else {
				GameObject newKey = new GameObject("NOTHING");
				newKey.transform.parent = currKeys.transform;
			}

			startX += 7f;
		}

	}

	//GET ACTUAL KEY PRESSED
	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						doKeyDown (it, true);
						myChuck.SetInt ("messageToSend", it + 100);
						myChuck.BroadcastEvent ("sendMessage");

						mainBackground.GetComponent<Renderer> ().material.color = new Color32 ((byte)UnityEngine.Random.Range(0f, 255f), (byte)UnityEngine.Random.Range(0f, 255f), (byte)UnityEngine.Random.Range(0f, 255f), 255);
					}
				}
			}
			//change bool when no longer pressed down
			if (Input.GetKeyUp (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						doKeyUp (it, true);
						myChuck.SetInt ("messageToSend", it);
						myChuck.BroadcastEvent ("sendMessage");
					}
				}
			}
		}

	}

	//HANDLE KEY DOWNS
	void doKeyDown(int it, bool thisKeyboard) {
		string letterToAdd = acceptableKeys [it, 1];

		if (thisKeyboard) {
			myChuck.BroadcastEvent ("keyDownTrigger");
		}

		if (currLetter < stepInstructions [1].Length && !thisKeyboard) {
			if (letterToAdd [0] == stepInstructions [1] [currLetter]) {
				pressTop = true;
				currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.gray;
				currKeysTop.transform.GetChild (currLetter).GetComponent<KeyWiggle> ().stopRotations ();
			}
		}

		if (currLetter < stepInstructions [2].Length && thisKeyboard) {
			if (letterToAdd [0] == stepInstructions [2] [currLetter] || stepInstructions [2] [currLetter] == '*') {
				pressBottom = true;
				currKeysBottom.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.gray;
				currKeysBottom.transform.GetChild (currLetter).GetComponent<KeyWiggle> ().stopRotations ();
			}
		}
	}

	//HANDLE KEY UPS
	void doKeyUp(int it, bool thisKeyboard) {
		string pressedLetter = acceptableKeys [it, 1];

		if (pressTop && (currLetter >= stepInstructions [2].Length || stepInstructions [2] [currLetter] == '*')) {
			pressBottom = false;
			pressTop = false;
			currKeysTop.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			currLetter++;
			if (thisKeyboard) {
				myChuck.BroadcastEvent ("keyCorrectTrigger");
			}
		} else if (pressBottom && (currLetter >= stepInstructions [1].Length || stepInstructions [1] [currLetter] == '*')) {
			pressBottom = false;
			pressTop = false;
			currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			currLetter++;
			if (thisKeyboard) {
				myChuck.BroadcastEvent ("keyCorrectTrigger");
			}
		} else if ((doubleWhammy == 1 && pressedLetter[0] == stepInstructions[2][currLetter]) ||
				(doubleWhammy == 2 && pressedLetter[0] == stepInstructions[1][currLetter])) {
			currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			currKeysTop.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			pressBottom = false;
			pressTop = false;
			doubleWhammy = 0;
			currLetter++;
			myChuck.BroadcastEvent ("keyCorrectTrigger");

		} else if (pressTop && pressBottom) {
			if (pressedLetter [0] == stepInstructions [1] [currLetter]) {
				doubleWhammy = 1;
			} else {
				doubleWhammy = 2;
			}
		} else {
			if (thisKeyboard) {
				myChuck.BroadcastEvent ("keyUpTrigger");
			}
			if (currLetter < stepInstructions [1].Length && pressedLetter [0] == stepInstructions [1] [currLetter] && !thisKeyboard) {
				pressTop = false;
				currKeysTop.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = topColor;
				currKeysTop.transform.GetChild (currLetter).GetComponent<KeyWiggle> ().startRotations ();

			} else if (currLetter < stepInstructions [2].Length && pressedLetter [0] == stepInstructions [2] [currLetter] && thisKeyboard) {
				pressBottom = false;
				currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = bottomColor;
				currKeysBottom.transform.GetChild (currLetter).GetComponent<KeyWiggle> ().startRotations ();

			}
		}
	}

	void GetIntCallback( System.Int64 messageReceived )
	{
		message = (int) messageReceived;
		newMessage = true;
	}


	void NewMessageReceived()
	{
		myChuck.GetInt ("messageReceived", myGetIntCallback);
	}

	// Wrap instruction text
	string ResolveTextSize(string input, int lineLength){

		// Split string by char " "         
		string[] words = input.Split(" "[0]);

		// Prepare result
		string result = "";

		// Temp line string
		string line = "";

		// for each all words        
		foreach(string s in words){
			// Append current word into line
			string temp = line + " " + s;

			// If line length is bigger than lineLength
			if(temp.Length > lineLength){

				// Append current line into result
				result += line + "\n";
				// Remain word append into new line
				line = s;
			}
			// Append current word into current line
			else {
				line = temp;
			}
		}

		// Append last line into result        
		result += line;

		// Remove first " " char
		return result.Substring(1,result.Length-1);
	}

}
