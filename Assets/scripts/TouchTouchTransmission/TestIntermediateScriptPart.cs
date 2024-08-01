using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
public class TestIntermediateTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Test intermediate start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = false;
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
		nextTime = Time.time + 35;
		SendNewTarget (TouchState.None, 50, 0.5f);
	}
	void partTwo() {
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Tranmiss Op Engage Accel") as AudioClip 
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
}