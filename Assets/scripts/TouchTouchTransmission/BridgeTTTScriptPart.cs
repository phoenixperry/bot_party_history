using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTTTScriptPart : AbstractTTTScriptPart {

	int currentPart;
	float nextTime = 0;
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			currentPart = 2;
			partTwo ();
		}
	}
	public override void startPart() {
		Debug.Log ("BRIDGE BEGIN");
		currentPart = 1;
		partOne ();
	}

	void partOne() {
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-bridge-instructions") as AudioClip);
		nextTime = Time.time + 6;
	}
	void partTwo() {
		SendEndScriptPart ();
	}
}
