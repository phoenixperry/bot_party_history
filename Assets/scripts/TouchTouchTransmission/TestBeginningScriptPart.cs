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
	bool readyToMoveOn = false;
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			readyToMoveOn = true;
		} else if (currentPart == 2 && nextTime <= Time.time) {
			readyToMoveOn = true;
		}
	}

	bool checkMoveOn() {
		if (readyToMoveOn && currentPart == 1) { 
			partTwo ();
			currentPart = 2;
			return true;
		} else if (readyToMoveOn && currentPart == 2) {
			SendClearTargets ();
			SendEndScriptPart ();
			return true;
		}
		return false;
	}
	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Success 2") as AudioClip);
		if (checkMoveOn()) { return; }
		SendNewTarget (TouchState.None, 80, 0.5f);
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
		if (checkMoveOn()) { return; }
		SendNewTarget (TouchState.None,100, 0.5f);
	}
	void partOne() {
		nextTime = Time.time + 40;
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Beginning Transmiss 1") as AudioClip 
		};
		SendPlayVoices (clips);
		SendNewTarget (TouchState.None, 100, 0.8f);
	}
	void partTwo() {
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Tranmiss Insuff") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/Engage Proto Inc Through") as AudioClip
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
}