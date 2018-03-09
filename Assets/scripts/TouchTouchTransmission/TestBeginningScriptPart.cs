using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class TestBeginningTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find ("Intro").Find ("IntroDrum").GetComponent<SampleSequencer> ().enabled = true;
		gameObject.transform.Find ("Intro").Find ("IntroLead").GetComponent<HelmSequencer> ().StartOnNextCycle();
		gameObject.transform.Find ("Intro").Find ("IntroBass").GetComponent<HelmSequencer> ().StartOnNextCycle();
		Debug.Log ("Test beginning start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Intro").Find("IntroLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Intro").Find ("IntroBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Intro").Find ("IntroDrum").GetComponent<SampleSequencer> ().enabled = false;
	}
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			partTwo ();
			currentPart = 2;
		} else if (currentPart == 2 && nextTime <= Time.time) {
			SendClearTargets ();
			SendEndScriptPart ();
		}
	}
	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/success-ping") as AudioClip);
		SendNewTarget (TouchState.None, 70, 1);
	}
	public override void targetFailure() {
		Debug.Log ("Target failure");
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/screech") as AudioClip);
		SendNewTarget (TouchState.None,70, 1);
	}
	void partOne() {
		nextTime = Time.time + 35;
		SendPlayVoice(Resources.Load ("TouchTouchTransmission/capage-drafts/test-beginning-start") as AudioClip);
		SendNewTarget (TouchState.None, 70, 1);
	}
	void partTwo() {
		nextTime = Time.time + 10;
		SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-beginning-end") as AudioClip);
	}
}