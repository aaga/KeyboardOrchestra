using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TextController : MonoBehaviour {

	private TextMesh t;
	private ChuckInstance myChuck;
	//private string[] acceptableKeys = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Space", "Semicolon", "Return", "Backspace", "Equals"};
	private string[,] specialWords = new string[,] { { "VOID", "void"},{ "RETURN", "return"},{ "GE", "ge"},{ "=>", "chuck"}, {"TEST", "0"} , {"OOOO", "100"}};
	private string[,] acceptableKeys = new string[,] { { "A", "A", "0"}, { "B", "B", "0"}, { "C", "C", "0"}, { "D", "D", "0"}, { "E", "E", "0"}, { "F", "F", "0"}, { "G", "G", "0"}, { "H", "H", "0"}, { "I", "I", "0"}, { "J", "J", "0"}, { "K", "K", "0"}, { "L", "L", "0"}, { "M", "M", "0"}, { "N", "N", "0"}, { "O", "O", "0"}, { "P", "P", "0"}, { "Q", "Q", "0"}, { "R", "R", "0"}, { "S", "S", "0"}, { "T", "T", "0"}, { "U", "U", "0"}, { "V", "V", "0"}, { "W", "W", "0"}, { "X", "X", "0"}, { "Y", "Y", "0"}, { "Z", "Z", "0"}, { "0", "0", "0"}, { "1", "1", "0"}, { "2", "2", "0"}, { "3", "3", "0"}, { "4", "4", "0"}, { "5", "5", "0"}, { "6", "6", "0"}, { "7", "7", "0"}, { "8", "8", "0"}, { "9", "9", "0"}, { "0", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Return", "", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", " ", "1"}};
	public int presetButton;
	public GameObject codeText;

	// Use this for initialization
	void Start () {
		t = (TextMesh)codeText.GetComponent(typeof(TextMesh));

		myChuck = GetComponent<ChuckInstance>();

		for (int i = 0; i < acceptableKeys.GetLength(0); i++) {
			acceptableKeys[i,2] = (i+72).ToString();
		}
		//set space to none
		acceptableKeys[acceptableKeys.GetLength(0) -1,2] = "0";
	}
		
	void OnMouseDown(){
		if (presetButton == 1) {
			Debug.Log ("button 1 clicked");
			t.text = "R E A D M E . M D";
			t.text += System.Environment.NewLine;
			t.text += " R E A D M E . M  ";
		}
		if (presetButton == 2) {
			Debug.Log ("button 1 clicked");
			t.text = "GE ";
		}
		if (presetButton == 3) {
			Debug.Log ("button 1 clicked");
			t.text = "DDDD DDD DD D";
			t.text += System.Environment.NewLine;
			t.text += "    G   G  G  ";
		}
		if (presetButton == 4) {
			Debug.Log ("button 1 clicked");
			t.text = "C C E  C  H  G  C C E  C J H";
		}
	
	}
		
	// Update is called once per frame
	void Update () {
		//hacked way of knowing that only the code text object with script should add text to the mesh
		if (presetButton == 0) {
			getKey ();
		}
	}

	void getKey(){
		bool found = false;
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
			string myTone = "20";
			if (Input.GetKeyDown (vKey)) {
				//find total lines of code to check runoff
				int numLines = t.text.Split('\n').Length;

				found = true;

				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == vKey.ToString ()) {
						string letterToAdd = acceptableKeys [it, 1];
						if (vKey.ToString () == "Space") {
							//addColorToWords();
						}else if(vKey.ToString () == "Return" && numLines <= 15){
							t.text += System.Environment.NewLine;	//add new line
							//addColorToWords();
						}else if(vKey.ToString () == "Backspace"){
							t.text = t.text.Substring(0,(t.text.Length - 1));
						}
						t.text += letterToAdd;
//						myChuck.SetInt ("myExternalInt", Int32.Parse(acceptableKeys [it, 2]));
						myTone = acceptableKeys [it, 2];
						//break
						it = acceptableKeys.GetLength (0);
					}

				}
					
				if (t.text.Length % 40 == 39 && numLines <= 15) {
					t.text += System.Environment.NewLine;	//add new line
				}
				if (numLines >= 16) {
					t.text = t.text.Substring(0,(t.text.Length - 1));
				}
				myChuck.RunCode(@"
					SinOsc foo => dac;" + myTone + 
					@" => Std.mtof => foo.freq;
					10::ms => now;
					0 => foo.freq;
					10::ms => now;

				");
				
			}
		}
//		if (!found) {
//			myChuck.SetInt ("myExternalInt", 0);
//		}

	}

	void addColorToWords(){
		string word = "";
		int wordStart = 0;
		for (int letter = 0; letter < t.text.Length; letter++) {
			Debug.Log ("searching for a special word" + word);
			if (t.text.Substring (letter, 1) != " ") {
				word += t.text.Substring (letter, 1);
			} 
			if(t.text.Substring (letter, 1) == " " || letter == t.text.Length - 1){
				for (int special = 0; special < specialWords.GetLength (0) - 1; special++) {
					if (specialWords [special, 0] == word) {
						Debug.Log ("found a special word" + word);
						t.text = t.text.Substring(0, letter + 1) + "</color>" + t.text.Substring(letter + 1);
						//do this second becuase otherwise will mess up element position for adding second tag
						t.text = t.text.Substring(0, wordStart) + "<color=white>" + t.text.Substring(wordStart);
						Debug.Log ("markedup text" + t.text);

					}
				}
				word = "";
				wordStart = letter + 1;
			}
		}
	}
		
}
