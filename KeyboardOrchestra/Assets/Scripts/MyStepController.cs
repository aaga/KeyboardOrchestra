using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MyStepController : MonoBehaviour {

	private TextMesh instructionMesh;
	private TextMesh inputMesh;
//	private string[,] specialWords = new string[,] { { "VOID", "void"},{ "RETURN", "return"},{ "GE", "ge"},{ "=>", "chuck"}, {"TEST", "0"} , {"OOOO", "100"}};
	private string[,] acceptableKeys = new string[,] { { "A", "a", "0"}, { "B", "b", "0"}, { "C", "c", "0"}, { "D", "d", "0"}, { "E", "e", "0"}, { "F", "f", "0"}, { "G", "g", "0"}, { "H", "h", "0"}, { "I", "i", "0"}, { "J", "j", "0"}, { "K", "k", "0"}, { "L", "l", "0"}, { "M", "m", "0"}, { "N", "n", "0"}, { "O", "o", "0"}, { "P", "p", "0"}, { "Q", "q", "0"}, { "R", "r", "0"}, { "S", "s", "0"}, { "T", "t", "0"}, { "U", "u", "0"}, { "V", "v", "0"}, { "W", "w", "0"}, { "X", "x", "0"}, { "Y", "y", "0"}, { "Z", "z", "0"}, { "0", "0", "0"}, { "1", "1", "0"}, { "2", "2", "0"}, { "3", "3", "0"}, { "4", "4", "0"}, { "5", "5", "0"}, { "6", "6", "0"}, { "7", "7", "0"}, { "8", "8", "0"}, { "9", "9", "0"}, { "0", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Return", "", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", " ", "1"}};
//	public int presetButton;
	public GameObject instructionText;
	public GameObject inputText;
	public GameObject ticker;
	private int currLetter;

	private float initialTickerX;

	public string[] stepInstructions;
	public bool newRound;
	public float linePos;

	//created melody text
	public string melodyString;

	// Use this for initialization
	void Start () {
		newRound = true;
		instructionMesh = (TextMesh)instructionText.GetComponent(typeof(TextMesh));
		inputMesh = (TextMesh)inputText.GetComponent(typeof(TextMesh));
		currLetter = 0;
		linePos = 0.0f;

		initialTickerX = ticker.transform.position.x;

		for (int i = 0; i < acceptableKeys.GetLength(0); i++) {
			acceptableKeys[i,2] = (i+72).ToString();
		}
		//set space to none
		acceptableKeys[acceptableKeys.GetLength(0) -1,2] = "0";
	}

	// Update is called once per frame
	void Update () {

		//change steps to have correct initial state
		if (newRound) {
			currLetter = 0;
			//initialize empty melody array
			melodyString = "[]";
			if (stepInstructions.GetLength (0) != 0) {
				instructionMesh.text = stepInstructions [0];
				inputMesh.text = stepInstructions [1];
			}
			newRound = false;
		}

		Vector3 temp = ticker.transform.position;
		temp.x = initialTickerX + linePos * 25f;
		ticker.transform.position = temp;

		//do the interaction
		getKey();

	}

	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						string letterToAdd = acceptableKeys [it, 1];
						if (vKey.ToString () == "Space") {
							//addColorToWords();
						}else if(vKey.ToString () == "Return"){
							//t.text += System.Environment.NewLine;	//add new line
							//addColorToWords();
							//proceed to next step

						}else if(vKey.ToString () == "Backspace"){
							//don't allow backspace 
							//t.text = t.text.Substring(0,(t.text.Length - 1));
						}
						//inputMesh.text += letterToAdd;
						//Debug.Log("key pressed, currletter is: " + stepInstructions [1][currLetter]);
						if (stepInstructions [2] == "greyOut" && currLetter < stepInstructions[1].Length) {
							Debug.Log ("should be first char (h):" + letterToAdd);
							if(letterToAdd[0] == stepInstructions [1][currLetter]){
								Debug.Log("made it" + stepInstructions [1][currLetter]);
								//inputMesh.text = inputMesh.text.Replace (inputMesh.text [23*currLetter)].ToString (), "<color=#00FF00>" + inputMesh.text [currLetter].ToString () + "</color>");

								inputMesh.text = inputMesh.text.Substring(0, 24*currLetter + 1) + "</color>" + inputMesh.text.Substring(24*currLetter + 1);
								inputMesh.text = inputMesh.text.Substring(0, 24*currLetter) + "<color=#00FF00>" + inputMesh.text.Substring(24	*currLetter);
								Debug.Log ("markedup text" + inputMesh.text);

								currLetter++;

							}
						}
						if (stepInstructions [2] == "melody") {
							//remove the "]" from the previous array
							melodyString = melodyString.Substring (0, (melodyString.Length - 1));

							//dont add comma before first entry
							if (melodyString.Length > 1) {
								melodyString += ",";
							}
							melodyString += acceptableKeys [it, 2];;
							melodyString += "]";
							inputMesh.text += letterToAdd;

						}
							
					}
				}
			}
		}

	}

//	void addColorToWords(){
//		string word = "";
//		int wordStart = 0;
//		for (int letter = 0; letter < t.text.Length; letter++) {
//			Debug.Log ("searching for a special word" + word);
//			if (t.text.Substring (letter, 1) != " ") {
//				word += t.text.Substring (letter, 1);
//			} 
//			if(t.text.Substring (letter, 1) == " " || letter == t.text.Length - 1){
//				for (int special = 0; special < specialWords.GetLength (0) - 1; special++) {
//					if (specialWords [special, 0] == word) {
//						Debug.Log ("found a special word" + word);
//						t.text = t.text.Substring(0, letter + 1) + "</color>" + t.text.Substring(letter + 1);
//						//do this second becuase otherwise will mess up element position for adding second tag
//						t.text = t.text.Substring(0, wordStart) + "<color=white>" + t.text.Substring(wordStart);
//						Debug.Log ("markedup text" + t.text);
//
//					}
//				}
//				word = "";
//				wordStart = letter + 1;
//			}
//		}
//	}

}
