﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuildupTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		Debug.Log ("Test buildup start");
		currentPart = 1;
		partOne ();
	}
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			partTwo ();
			currentPart = 2;
		} else if (currentPart == 2 && nextTime <= Time.time) {
			currentPart = 3;
			partThree ();
		}
	}
	public override void targetSuccess() {
		if (currentPart == 1 || currentPart == 2) {
			SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/success-ping") as AudioClip);
			SendNewTarget (TouchState.None, 30);
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		}
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/screech") as AudioClip);
		SendNewTarget (TouchState.None,30);
	}
	void partOne() {
		nextTime = Time.time + 45;
		SendPlayVoice(Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-start") as AudioClip);
		SendNewTarget (TouchState.None, 30);
	}
	void partTwo() {
		SendClearTargets ();
		nextTime = Time.time + 6;
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-end") as AudioClip);
	}
	void partThree() {
		SendNewTarget (TouchState.AllConnected, 255);
	}
	void partFour() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/test-transmission-end") as AudioClip);
		SendClearTargets ();
		SendEndScriptPart ();
	}
}