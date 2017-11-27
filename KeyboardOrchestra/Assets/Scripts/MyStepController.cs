using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MyStepController : MonoBehaviour {

	public String otherIP;
	public String sendingPort;
	public String receivingPort;

	private TextMesh instructionMesh;
	private TextMesh inputMesh;
//	private string[,] specialWords = new string[,] { { "VOID", "void"},{ "RETURN", "return"},{ "GE", "ge"},{ "=>", "chuck"}, {"TEST", "0"} , {"OOOO", "100"}};
	private string[,] acceptableKeys = new string[,] { { "A", "a", "0"}, { "B", "b", "0"}, { "C", "c", "0"}, { "D", "d", "0"}, { "E", "e", "0"}, { "F", "f", "0"}, { "G", "g", "0"}, { "H", "h", "0"}, { "I", "i", "0"}, { "J", "j", "0"}, { "K", "k", "0"}, { "L", "l", "0"}, { "M", "m", "0"}, { "N", "n", "0"}, { "O", "o", "0"}, { "P", "p", "0"}, { "Q", "q", "0"}, { "R", "r", "0"}, { "S", "s", "0"}, { "T", "t", "0"}, { "U", "u", "0"}, { "V", "v", "0"}, { "W", "w", "0"}, { "X", "x", "0"}, { "Y", "y", "0"}, { "Z", "z", "0"}, { "0", "0", "0"}, { "1", "1", "0"}, { "2", "2", "0"}, { "3", "3", "0"}, { "4", "4", "0"}, { "5", "5", "0"}, { "6", "6", "0"}, { "7", "7", "0"}, { "8", "8", "0"}, { "9", "9", "0"}, { "0", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", " ", "1"}};
//	public int presetButton;
	public GameObject instructionText;
	public GameObject inputText;
	public GameObject ticker;
	private int currLetter;

	private GameObject currKeysTop;
	private GameObject currKeysBottom;
	private bool pressTop;
	private bool pressBottom;
	public bool topDone;
	public bool bottomDone;

	private Color32 topColor;
	private Color32 bottomColor;
	private Color32 correctColor;

	private float initialTickerX;

	public string[] stepInstructions;
	public bool newRound;
	public float linePos;

	//created melody text
	public string melodyString;

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
		instructionMesh = (TextMesh)instructionText.GetComponent(typeof(TextMesh));
		inputMesh = (TextMesh)inputText.GetComponent(typeof(TextMesh));
		currLetter = 0;
		linePos = 0.0f;

		topColor = new Color32(48, 230, 169,255);
		bottomColor = new Color32(63, 56, 255,255);
		correctColor = new Color32 (56,224,101,255);

		myChuck = GetComponent<ChuckInstance> ();
		myGetIntCallback = Chuck.CreateGetIntCallback( GetIntCallback );
		myCallback = Chuck.CreateVoidCallback( NewMessageReceived );
		message = -1;
		newMessage = false;

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

		myChuck.RunCode(
			@"
			0 => external int messageToSend;
			0 => external int messageReceived;
			external Event sendMessage;
			external Event notifier;

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

			// infinite time loop
			while( true )
			{
    			sendMessage => now;
    			spork ~ sendIntMessage(messageToSend);
			}
			"
		);


		myChuck.StartListeningForChuckEvent( "notifier", myCallback );
	}

	// Update is called once per frame
	void Update () {

		if (newMessage) {
			if (message >= 100) {
				doKeyDown (message - 100, false);
			} else {
				doKeyUp (message, false);
			}
			newMessage = false;
		}

		//change steps to have correct initial state
		if (newRound) {
			currLetter = 0;
			//initialize empty melody array
			melodyString = "[]";
			volumeCount = 0;
			if (stepInstructions.GetLength (0) != 0) {
				//instructionMesh.text = stepInstructions [0];
				instructionMesh.text = ResolveTextSize (stepInstructions [0], 45);
				//inputMesh.text = stepInstructions [1];

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

		if (stepInstructions.Length > 0 && currLetter >= stepInstructions [1].Length && currLetter >= stepInstructions [2].Length) {
			//Debug.Log ("FINISHED BOTH WORDS");
			topDone = true;
			bottomDone = true;
		}

	}

	void addGraphicKeys(string inputWord, float yPos, GameObject currKeys, string whichComputer){
		//add all new key instructions
		foreach (Transform child in currKeys.transform) {
			GameObject.Destroy(child.gameObject);
		}

		float startX = -12f;
		foreach (char c in inputWord)
		{
			GameObject newKey = (GameObject)Instantiate(Resources.Load("Default Key"));
			TextMesh newKeyText = (TextMesh)newKey.transform.GetChild (0).GetComponent (typeof(TextMesh));
			newKeyText.text = c.ToString();
			//Debug.Log("new key created with text: ", newKeyText);
			//Debug.Log (c);
			newKey.transform.position = new Vector3 (startX, yPos, -7f);
			//make sure layering of objects is correct
			newKey.transform.GetChild (0).transform.position = new Vector3 (newKey.transform.GetChild (0).transform.position.x, newKey.transform.GetChild (0).transform.position.y, -2f);
			newKey.transform.GetChild (1).transform.position = new Vector3 (newKey.transform.GetChild (1).transform.position.x, newKey.transform.GetChild (1).transform.position.y, -1f);
			newKey.transform.GetChild (2).transform.position = new Vector3 (newKey.transform.GetChild (2).transform.position.x, newKey.transform.GetChild (2).transform.position.y, 0f);

			startX += 7f;
			newKey.transform.parent = currKeys.transform;
			if (whichComputer == "top") {
				newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = topColor;
			} else {
				newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = bottomColor;
			}
		}

	}

	void doKeyDown(int it, bool thisKeyboard) {
		string letterToAdd = acceptableKeys [it, 1];
		if (currLetter < stepInstructions [1].Length && !thisKeyboard) {
			//Debug.Log ("should be first char (h):" + letterToAdd);
			if (letterToAdd [0] == stepInstructions [1] [currLetter]) {
				pressTop = true;
				//new code to handle physical keys turning grey instead of turning text green
				//backgorund of key
				currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.gray;
			}
		}

		if (currLetter < stepInstructions [2].Length && thisKeyboard) {
			if (letterToAdd [0] == stepInstructions [2] [currLetter]) {
				pressBottom = true;
				currKeysBottom.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.gray;

			}
		}

		if (pressTop && pressBottom) {
			pressBottom = false;
			pressTop = false;
			currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = correctColor;
			currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			currLetter++;
		} else if (pressTop && currLetter >= stepInstructions [2].Length) {
			pressBottom = false;
			pressTop = false;
			currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = correctColor;
			currLetter++;
		} else if (pressBottom && currLetter >= stepInstructions [1].Length) {
			pressBottom = false;
			pressTop = false;
			currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = correctColor;
			currLetter++;
		}
	}

	void doKeyUp(int it, bool thisKeyboard) {
		string letterToAdd = acceptableKeys [it, 1];
		if (currLetter < stepInstructions [1].Length && letterToAdd [0] == stepInstructions [1] [currLetter] && !thisKeyboard) {
			pressTop = false;
			currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = topColor;

		}
		if (currLetter < stepInstructions [2].Length && letterToAdd [0] == stepInstructions [2] [currLetter] && thisKeyboard) {
			pressBottom = false;
			currKeysBottom.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = bottomColor;

		}
	}

	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						doKeyDown (it, true);
						myChuck.SetInt ("messageToSend", it + 100);
						myChuck.BroadcastEvent ("sendMessage");
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

	void GetIntCallback( System.Int64 messageReceived )
	{
		message = (int) messageReceived;
		newMessage = true;
	}


	void NewMessageReceived()
	{
		myChuck.GetInt ("messageReceived", myGetIntCallback);
	}

	// Wrap text  function
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
