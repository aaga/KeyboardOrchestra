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

	private GameObject currKeysTop;
	private GameObject currKeysBottom;
	private bool pressTop;
	private bool pressBottom;

	private float initialTickerX;

	public string[] stepInstructions;
	public bool newRound;
	public float linePos;

	//created melody text
	public string melodyString;

	//created melody text
	public float volumeCount;

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

		//create container to have new keys added
		currKeysTop = new GameObject("currKeysTop");
		currKeysTop.AddComponent<MeshFilter>();
		currKeysTop.AddComponent<MeshRenderer>();
		currKeysBottom = new GameObject("currKeysBottom");
		currKeysBottom.AddComponent<MeshFilter>();
		currKeysBottom.AddComponent<MeshRenderer>();

	}

	// Update is called once per frame
	void Update () {

		//change steps to have correct initial state
		if (newRound) {
			currLetter = 0;
			//initialize empty melody array
			melodyString = "[]";
			volumeCount = 0;
			if (stepInstructions.GetLength (0) != 0) {
				instructionMesh.text = stepInstructions [0];
				//inputMesh.text = stepInstructions [1];

				addGraphicKeys (stepInstructions [1], 0f, currKeysTop);
				addGraphicKeys (stepInstructions [2], -4f, currKeysBottom);

			}
			newRound = false;

			//no keys pressed
			pressTop = false;
			pressBottom = false;
		}

		Vector3 temp = ticker.transform.position;
		temp.x = initialTickerX + linePos * 25f;
		ticker.transform.position = temp;

		//do the interaction
		getKey();

	}

	void addGraphicKeys(String inputWord, float yPos, GameObject currKeys){
		//add all new key instructions
		foreach (Transform child in currKeys.transform) {
			GameObject.Destroy(child.gameObject);
		}

		float startX = -14f;
		foreach (char c in inputWord)
		{
			GameObject newKey = (GameObject)Instantiate(Resources.Load("Default Key"));
			TextMesh newKeyText = (TextMesh)newKey.transform.GetChild (0).GetComponent (typeof(TextMesh));
			newKeyText.text = c.ToString();
			Debug.Log("new key created with text: ", newKeyText);
			Debug.Log (c);
			newKey.transform.position = new Vector3 (startX, yPos, 3f);
			startX += 3f;
			newKey.transform.parent = currKeys.transform;
			if (stepInstructions [4] == "1") {
				newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = Color.blue;
			} else {
				newKey.transform.GetChild (1).GetComponent<Renderer> ().material.color = Color.cyan;

			}
		}

	}

	void getKey(){
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			if (Input.GetKeyDown (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						string letterToAdd = acceptableKeys [it, 1];
						Debug.Log ("MOST IMPORTANT: " + letterToAdd);
						if (currLetter < stepInstructions [1].Length) {
							Debug.Log ("should be first char (h):" + letterToAdd);
							if (letterToAdd [0] == stepInstructions [1] [currLetter]) {
								pressTop = true;
								//new code to handle physical keys turning grey instead of turning text green
								//backgorund of key
							}
						}

						if (currLetter < stepInstructions [2].Length) {
							if (letterToAdd [0] == stepInstructions [2] [currLetter]) {
								pressBottom = true;
							}
						}

						if (pressTop && pressBottom) {
							pressBottom = false;
							pressTop = false;
							currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.green;
							currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = Color.green;
							currLetter++;
						}else if (pressTop && currLetter >= stepInstructions [2].Length) {
							pressBottom = false;
							pressTop = false;
							currKeysTop.transform.GetChild(currLetter).GetChild(1).GetComponent<Renderer> ().material.color = Color.green;
							currLetter++;
						}else if (pressBottom && currLetter >= stepInstructions [1].Length) {
							pressBottom = false;
							pressTop = false;
							currKeysBottom.transform.GetChild (currLetter).GetChild (1).GetComponent<Renderer> ().material.color = Color.green;
							currLetter++;
						}
					}
				}
			}
			//change bool when no longer pressed down
			if (Input.GetKeyUp (vKey)) {
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						string letterToAdd = acceptableKeys [it, 1];
						if (currLetter < stepInstructions [1].Length && letterToAdd [0] == stepInstructions [1] [currLetter]) {
							pressTop = false;
						}
						if (currLetter < stepInstructions [2].Length && letterToAdd [0] == stepInstructions [2] [currLetter]) {
							pressBottom = false;
						}
					}
				}
			}
		}

	}
}
