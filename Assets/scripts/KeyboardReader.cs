using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardReader : AbstractReader {

	// Use this for initialization
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Q)) {
			passOnTouch(new TouchedBots("BoxOneTwo", "1")); 
		}

		if (Input.GetKey (KeyCode.W)) {
			passOnTouch(new TouchedBots("BoxTwoThree", "1")); 		
		}

		if (Input.GetKey (KeyCode.E)) {
			passOnTouch(new TouchedBots("BoxOneThree", "1")); 		}

	}
}
