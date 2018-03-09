using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class TestBuildupTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find("Resol").Find ("ResolBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Resol").Find ("ResolLead").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Resol").Find ("ResolDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Test buildup start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Resol").Find ("ResolBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolDrum").GetComponent<SampleSequencer> ().enabled = false;

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
			SendNewTarget (TouchState.None, 30, 1);
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		}
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/screech") as AudioClip);
		SendNewTarget (TouchState.None,30, 1);
	}
	void partOne() {
		nextTime = Time.time + 60;
		SendPlayVoice(Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-start") as AudioClip);
		SendNewTarget (TouchState.None, 70, 1);
	}
	void partTwo() {
		SendClearTargets ();
		nextTime = Time.time + 5;
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-end") as AudioClip);
	}
	void partThree() {
		SendNewTarget (TouchState.AllConnected, 255, 0);
	}
	void partFour() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/test-transmission-end") as AudioClip);
		SendClearTargets ();
		SendEndScriptPart ();
	}
	// Copy pasted from bridge test script
	public override void BoxOneStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = true;
		Debug.Log ("Playing motion1");
	}
	public override void BoxOneStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = false;
	}

	public override void BoxTwoStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = true;
		Debug.Log ("Playing motion2");
	}
	public override void BoxTwoStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = false;
	}


	public override void BoxThreeStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Playing motion3");
	}
	public override void BoxThreeStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = false;
	}
}