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
		//gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Bridge").Find ("BridgeDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("BRIDGE BEGIN");
		currentPart = 1;
		partOne ();
		TextAsset noteasy = Resources.Load("MarkovFiles/NotEasyBeingGreen") as TextAsset;
		AbstractMarkovMusic markov_motion1 = new TestMarkovMusic (noteasy.text);
		AbstractMarkovMusic markov_motion2 = new TestMarkovMusic (noteasy.text);
		Transform motion1 = gameObject.transform.Find ("Motion").Find ("Motion1");
		Transform motion2 = gameObject.transform.Find ("Motion").Find ("Motion2");
		HelmSequencer seq1 = motion1.GetComponent<HelmSequencer> ();
		HelmSequencer seq2 = motion2.GetComponent<HelmSequencer> ();
		markov_motion1.addNextBeats (seq1.length, seq1);
		markov_motion2.addNextBeats (seq2.length, seq2);

	}
	public override void stopPart() {
		gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Bridge").Find ("BridgeDrum").GetComponent<SampleSequencer> ().enabled = false;
	}

	void partOne() {
		//SendPlayVoice (Resources.Load ("TouchTouchTransmission/capage-drafts/test-bridge-instructions") as AudioClip);
		nextTime = Time.time + 30;
	}
	void partTwo() {
		SendEndScriptPart ();
	}

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
