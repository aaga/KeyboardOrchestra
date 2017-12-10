using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	public bool startAnimation;
	public bool doneAnimation;

	public bool right;
	public bool left;

	public bool open;
	public bool close;

	public bool openDone;
	public bool closeDone;

	public GameObject partner;
	public GameObject you;
	public GameObject levelText;

	// Use this for initialization
	void Start () {
		startAnimation = false;
		open = false;
		close = false;
		openDone = false;
		closeDone = false;
		doneAnimation = false;
	}

	void closeAnimation(){
		if (right) {
			transform.position = new Vector3 (transform.position.x-.5f, transform.position.y, transform.position.z);
		} else if (left) {
			transform.position = new Vector3 (transform.position.x+.5f, transform.position.y, transform.position.z);
		}
	}

	void openAnimation(){
		if (right) {
			transform.position = new Vector3 (transform.position.x+.5f, transform.position.y, transform.position.z);
		} else if (left) {
			transform.position = new Vector3 (transform.position.x-.5f, transform.position.y, transform.position.z);
		}
	}

	// Update is called once per frame
	void Update () {

		if (startAnimation) {
			if (!closeDone) {
				close = true;
			}else if (!openDone) {
				open = true;
			}
			if (openDone && closeDone) {
				openDone = false;
				closeDone = false;
				doneAnimation = true;
			} else {
				doneAnimation = false;
			}
		}

		if (close) {
			closeDone = false;
			partner.SetActive (false);
			you.SetActive (false);
			float magicNum = 30; //super janky way to get the two screens to move farther past the middle to give us more "time" to show the level num
			float goalRightPosition = 51.5f;
			float goalLeftPosition = -50.0f;
			if (transform.position.x > goalRightPosition-magicNum && right) {
				closeAnimation ();
			} else if (transform.position.x < goalLeftPosition+magicNum && left) {
				closeAnimation ();
			} else {
				close = false; // turn off
				closeDone = true;
				openDone = false;
				levelText.SetActive (true);
			}
			//turn on Level Text!
			if(transform.position.x <= goalRightPosition && right){
				levelText.SetActive (true);
			}
		}
		if (open) {
			openDone = false;
			float goalRightPosition = 86.2f;
			float goalLeftPosition = -86.0f;
			if (transform.position.x < goalRightPosition && right) {
				openAnimation ();
			} else if (transform.position.x > goalLeftPosition && left) {
				openAnimation ();
			} else {
				open = false; // turn off
				openDone = true;
				partner.SetActive (true);
				you.SetActive (true);
			}
			//turn off Level Text!
			if(transform.position.x >= 51.5f && right){
				levelText.SetActive (false);
			}
		}
	}

}
