using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

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
		gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Bridge").Find ("BridgeDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("BRIDGE BEGIN");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Bridge").Find ("BridgeDrum").GetComponent<SampleSequencer> ().enabled = false;
	}

	void partOne() {
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-bridge-instructions") as AudioClip);
		nextTime = Time.time + 15;
	}
	void partTwo() {
		SendEndScriptPart ();
	}
}
