using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
public class InfiniteTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.LogWarning ("Infinite TTT Test");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = false;
	}
	void Update() {

	}
	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Success 2") as AudioClip);
		SendNewTarget (TouchState.None, 40, 0.2f);
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
		SendNewTarget (TouchState.None, 40, 0.2f);
	}
	void partOne() {
		nextTime = Time.time + 35;
		SendNewTarget (TouchState.None, 50, 1f);
	}
}