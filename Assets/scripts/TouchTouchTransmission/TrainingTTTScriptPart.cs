using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTTTScriptPart : AbstractTTTScriptPart {

	int currentPart;
	float nextTime = 0;
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			currentPart = 2;
			partTwo ();
		} else if (currentPart == 5 && nextTime <= Time.time) {
			currentPart = 6;
			partSix ();
		} else if (currentPart == 7 && nextTime <= Time.time) {
			currentPart = 8;
			partEight ();
		}
	}
	public override void startPart() {
		Debug.Log ("TRAINING BEGIN");
		currentPart = 1;
		partOne ();
	}

	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/success-ping") as AudioClip);
		if (currentPart == 2) {
			currentPart = 3;
			partThree ();
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		} else if (currentPart == 4) {
			currentPart = 5;
			partFive ();
		} else if (currentPart == 6) {
			currentPart = 7;
			partSeven ();
		}
	}

	void partOne() {
		// Instructions, 4s wait.
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/instructions") as AudioClip);
		nextTime = Time.time + 4;
	}
	void partTwo() {
		SendNewTarget (TouchState.OneTwo,250);
	}
	void partThree() {
		SendNewTarget (TouchState.TwoThree,250);
	}
	void partFour() {
		SendNewTarget (TouchState.OneThree,250);
	}
	void partFive() {
		SendClearTargets ();
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/pre-end-training") as AudioClip);
		nextTime = Time.time + 2;
	}
	void partSix() {
		SendNewTarget (TouchState.AllConnected, 250);
	}
	void partSeven() {
		SendClearTargets ();
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/end-training") as AudioClip);
		nextTime = Time.time + 6;
	}
	void partEight() {
		SendEndScriptPart ();
	}
}
