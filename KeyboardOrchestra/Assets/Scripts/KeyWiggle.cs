using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWiggle : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//transform.Rotate(0,0,mySine*10, Space.Self)
		if (Time.time % 2 > 1) {
			transform.RotateAround (transform.position - new Vector3(0,3,0), Vector3.forward, Time.time % 1 - 2f);
		} else 
			transform.RotateAround (transform.position  - new Vector3(0,3,0), Vector3.forward, (-1 * Time.time % 1) + 2f);
		}


//		Vector3 mySine = new Vector3(0,1,0);
//		transform.LookAt(transform.position + mySine);
	}
}
