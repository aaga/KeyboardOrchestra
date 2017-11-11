using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UIButtonController : MonoBehaviour {

	public GameObject text;
	private TextMesh t;
	private ChuckInstance myChuck;
	public Slider mainSlider;
	private int currSliderValue = 100;
	public bool loop = false;
	private int currentlyLooping = 0;
	//2 lines, 10 ints
	private string[] data = new string[15];

	private string currTone = "SinOsc";

	private string[,] specialWords = new string[,] { { "VOID", "-1"},{ "RETURN", "-1"},{ "GE", "-1"},{ "=>", "-1"}, {"TEST", "-1"} , {"OOOO", "-1"}};
	//private string[] acceptableKeys = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Space", "Semicolon", "Return", "Backspace", "Equals"};
	private string[,] acceptableKeys = new string[,] { { "A", "A", "0"}, { "B", "B", "0"}, { "C", "C", "0"}, { "D", "D", "0"}, { "E", "E", "0"}, { "F", "F", "0"}, { "G", "G", "0"}, { "H", "H", "0"}, { "I", "I", "0"}, { "J", "J", "0"}, { "K", "K", "0"}, { "L", "L", "0"}, { "M", "M", "0"}, { "N", "N", "0"}, { "O", "O", "0"}, { "P", "P", "0"}, { "Q", "Q", "0"}, { "R", "R", "0"}, { "S", "S", "0"}, { "T", "T", "0"}, { "U", "U", "0"}, { "V", "V", "0"}, { "W", "W", "0"}, { "X", "X", "0"}, { "Y", "Y", "0"}, { "Z", "Z", "0"}, { "0", "0", "0"}, { "1", "1", "0"}, { "2", "2", "0"}, { "3", "3", "0"}, { "4", "4", "0"}, { "5", "5", "0"}, { "6", "6", "0"}, { "7", "7", "0"}, { "8", "8", "0"}, { "9", "9", "0"}, { "0", "0", "0"}, { "Semicolon", ";", "0"}, { "Equals", "=", "0"}, { "Return", "", "0"}, { "Backspace", "", "0"}, { "LeftParen", "(", "0"}, { "RightParen", ")", "0"},{ "DoubleQuote", @"""", "0"},{ "Plus", "+", "0"},{ "Minus", "-", "0"},{ "Period", ".", "0"},{ "Colon", ":", "0"},{ "Greater", ">", "0"},{ "Less", "<", "0"},{ "Slash", "/", "0"},{ "Space", " ", "1"}};

	// Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
		Debug.Log("checking slider: " + mainSlider.value);
		currSliderValue = Math.Abs(300 - (int)mainSlider.value);
		Debug.Log("new slider value: " + currSliderValue);

	}

	//change tone of compile and run based on clicks of the three theme buttons
	public void changeTone(string newTone){
		currTone = newTone;
		//SqrOsc, SinOsc,PulseOsc
	}

	void Start()
	{
		myChuck = GetComponent<ChuckInstance>();

		for (int i = 0; i < acceptableKeys.GetLength(0); i++) {
//			acceptableKeys[i,2] = ((float)((i+1)*40.001)).ToString();
			acceptableKeys[i,2] = (i+72).ToString();
		}

		//slider change value callback
		mainSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });

		t = (TextMesh)text.GetComponent(typeof(TextMesh));
	}

	void OnMouseDown()
	{
		t = (TextMesh)text.GetComponent(typeof(TextMesh));
		Debug.Log("all text: " + t.text);

		clearData ();

//		string[] data = new string[10];
		//array to send to chuck
		if (t.text.Length > 0) {
//			data = "[";
			int numNotes = t.text.Length;
			string foundHiddenWord = "";
			string searchWord = "";

			bool newLine = true;
			int lineCounter = 0;

			for (int i = 0; i < numNotes; i++) {
				if (newLine) {
					data [lineCounter] = "[";
					newLine = false;
				}
				string myTone = "0";
				if (t.text.Substring (i, 1) != " ") {
					searchWord += t.text.Substring (i, 1);
				} 
				//check end of (special) word
				if(t.text.Substring (i, 1) == " " || i == t.text.Length - 1 || t.text.Substring (i, 1) == System.Environment.NewLine){
					for (int special = 0; special < specialWords.GetLength (0) - 1; special++) {
						if (specialWords [special, 0] == searchWord) {
							Debug.Log ("found a hidden word" + searchWord);
							foundHiddenWord = specialWords [special, 1];
						}
					}
					searchWord = "";
				}
				for (int it = 0; it < acceptableKeys.GetLength (0); it++) {
					if (acceptableKeys [it, 0] == t.text.Substring (i, 1)) {
						myTone = acceptableKeys [it, 2];
					}
				}
				data [lineCounter] += myTone;
				data [lineCounter] += ",";
				if(foundHiddenWord != ""){
					data [lineCounter] += foundHiddenWord;
					data [lineCounter] += ",";
				}
				//hit new line or end of curr line
				if( t.text.Substring (i, 1) == System.Environment.NewLine || i == numNotes - 1){
					newLine = true;
					data [lineCounter] = data[lineCounter].Substring (0, (data[lineCounter].Length - 1));
					data [lineCounter] += "]";
					lineCounter++;
					Debug.Log ("FOUND A NEW LINE");
				}
				
			}

			//cut off last comma
		}
			
		//run button
		if (loop) {
			currentlyLooping++;
			currentlyLooping = currentlyLooping%2;
			myChuck.SetInt ("loopTrigger", currentlyLooping);

			for (int currLine = 0; currLine < data.Length; currLine++) {
				if (data [currLine].Length >= 2) {
					myChuck.RunCode (currTone + @" foo => dac;
						external int loopTrigger;
						while(loopTrigger == 1){"
					+ data [currLine] + @" @=> int myNotes[];
							//if(loopTrigger == 1){
								for(0 => int x; x < myNotes.cap(); x++)
								{
									if(myNotes[x] == -1){
										//2000 => foo.freq;
										//1::second => now;
									}else{
										myNotes[x] => Std.mtof => foo.freq;
										" + currSliderValue +
										@"::ms => now;
										0 => foo.freq;
										20::ms => now;
									}
								}
							//}
						}"		
					);
				}
			}
			//compile button
		} else {
			for (int currLine = 0; currLine < data.Length; currLine++) {
				Debug.Log ("data sending to chuck: " + data[currLine]);
				if (data [currLine].Length >= 2) {
					myChuck.RunCode (currTone + @" foo => dac;
					1 => external int myTone;"
					+ data [currLine] + @" @=> int myNotes[];
						for(0 => int x; x < myNotes.cap(); x++)
						{
							if(myNotes[x] == -1){
								// sound file
								me.sourceDir() + ""/../StreamingAssets/ge.wav"" => string filename;
								if( me.args() ) me.arg(0) => filename;						
								// the patch 
								SndBuf buf => dac;
   								0 => buf.pos;

								// load the file
								filename => buf.read;

								buf.length() => now;	

							}else{

//								Bowed foo => JCRev r => dac;
//								if(myTone == 1){
//									Bowed foo => JCRev r => dac;
//								}
//								if(myTone == 2){
//									FM foo => JCRev r => dac;
//								}
//								if(myTone == 3){
//									VoicForm foo => JCRev r => dac;
//								}
								//.75 => r.gain;
								//.05 => r.mix;
								myNotes[x] => Std.mtof => foo.freq;
//				    			.1 => foo.noteOn;
								" + currSliderValue +
								@"::ms => now;
								0 => foo.freq;

							}
						}"		
					);
				}
			}

		}
		lookForWords ();
	}

	void clearData(){
		for (int currLine = 0; currLine < data.Length; currLine++) {
			data [currLine] = "";
		}
	}

	void lookForWords(){
		string word = "";
		for (int letter = 0; letter < t.text.Length; letter++) {
			Debug.Log ("searching for a hidden word" + word);
			if (t.text.Substring (letter, 1) != " ") {
				word += t.text.Substring (letter, 1);
			} 
			if(t.text.Substring (letter, 1) == " " || letter == t.text.Length - 1){
				for (int special = 0; special < specialWords.GetLength (0) - 1; special++) {
					if (specialWords [special, 0] == word) {
						Debug.Log ("found a hidden word" + word);
					}
				}
				word = "";
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

}
