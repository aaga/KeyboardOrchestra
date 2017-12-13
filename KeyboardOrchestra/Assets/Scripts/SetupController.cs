using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupController : MonoBehaviour {

	public GameObject partner;
	public GameObject you;
	private GameObject ipInput;
	private TextMesh ipInputMesh;

	public GameObject main;
	private MainController mainScript;

	public GameObject Step1;
	private MyStepController stepScript;

	private bool firstKeyPress;
	public bool setupDone;

	private string[,] acceptableKeys = new string[,] { { "A", "a", "0"}, { "B", "b", "0"}, { "C", "c", "0"}, { "D", "d", "0"}, { "E", "e", "0"}, { "F", "f", "0"}, { "G", "g", "0"}, { "H", "h", "0"}, { "I", "i", "0"}, { "J", "j", "0"}, { "K", "k", "0"}, { "L", "l", "0"}, { "M", "m", "0"}, { "N", "n", "0"}, { "O", "o", "0"}, { "P", "p", "0"}, { "Q", "q", "0"}, { "R", "r", "0"}, { "S", "s", "0"}, { "T", "t", "0"}, { "U", "u", "0"}, { "V", "v", "0"}, { "W", "w", "0"}, { "X", "x", "0"}, { "Y", "y", "0"}, { "Z", "z", "0"}, { "Alpha0", "0", "0"}, { "Alpha1", "1", "0"}, { "Alpha2", "2", "0"}, { "Alpha3", "3", "0"}, { "Alpha4", "4", "0"}, { "Alpha5", "5", "0"}, { "Alpha6", "6", "0"}, { "Alpha7", "7", "0"}, { "Alpha8", "8", "0"}, { "Alpha9", "9", "0"}, { "Alpha00", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", "", "1"},{ "Return", "", "0"}};

	//NOTE: using the same instruciton text mesh as we use later to start the game
	private TextMesh instructionMesh;
	public GameObject instructionText;

	private string setupText;
	// Use this for initialization
	void Start () {
		partner.SetActive (false);
		you.SetActive (false);
		instructionMesh = (TextMesh)instructionText.GetComponent(typeof(TextMesh));
		ipInput = transform.GetChild (0).gameObject;
		ipInputMesh = (TextMesh)ipInput.GetComponent (typeof(TextMesh));
	
		stepScript = Step1.GetComponent<MyStepController> ();
		firstKeyPress = true;
		setupDone = false;

		mainScript = main.GetComponent<MainController> ();

		setupText = "Join the same WiFi network and find that magical number called the IP address in Settings.";

	}
	
	// Update is called once per frame
	void Update () {
		instructionMesh.text = ResolveTextSize (setupText, 30);
		instructionMesh.text += "\n\nLeave blank if playing alone. \nPress Enter when done.";
		getKey ();
	}

	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				if (firstKeyPress) {
					firstKeyPress = false;
					ipInputMesh.text = "";
				}
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						string letterToAdd = acceptableKeys [it, 1];
						if (vKey.ToString () == "Backspace") {
							ipInputMesh.text = ipInputMesh.text.Substring (0, (ipInputMesh.text.Length - 1));
						}
						//user done
						if (vKey.ToString () == "Return") {
							setupDone = true;
							string otherIP;
							string receivingPort;
							string sendingPort;

							if (ipInputMesh.text == "") {
								otherIP = "localhost";
								sendingPort = "6449";
								receivingPort = "6449";
							} else {
								otherIP = ipInputMesh.text;
							}
							//enable for two player mode - not testing
							if (mainScript.playerNumber == 0) {
								sendingPort = "6449";
								receivingPort = "6439";
							} else if (mainScript.playerNumber == 1) {
								sendingPort = "6439";
								receivingPort = "6449";
							}
							stepScript.runTheChuck (otherIP, receivingPort, sendingPort);

						}
						ipInputMesh.text += letterToAdd;
						it = acceptableKeys.GetLength (0);
					}
				}
			}
		}
	}

	// Wrap instruction text
	string ResolveTextSize(string input, int lineLength){

		// Split string by char " "         
		string[] words = input.Split(" "[0]);
		string result = "";
		string line = "";

		foreach(string s in words){
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
