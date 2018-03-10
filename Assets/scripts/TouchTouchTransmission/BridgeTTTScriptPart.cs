using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class BridgeTTTScriptPart : AbstractTTTScriptPart {

	int currentPart;
	float nextTime = 0;
	int times_failed = 0;
	bool one_moved = false;
	bool two_moved = false;
	bool three_moved = false;
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			currentPart = 2;
			partTwo ();
		} else if (currentPart == 2 && nextTime <= Time.time) {
			currentPart = 3;
			partThree ();
		}
	}
	public override void startPart() {
		gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = true;
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
		markov_motion1.fillSequencer(seq1);
		markov_motion2.fillSequencer(seq2);

	}
	public override void stopPart() {
		gameObject.transform.Find("Bridge").Find ("BridgeBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Bridge").Find ("BridgeDrum").GetComponent<SampleSequencer> ().enabled = false;
	}

	void partOne() {
		partOne (30, false);
	}
	void partOne(float time, bool nudge) {
		
		List<AudioClip> clips;
		if (!nudge) {
			clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Humans Move Twist Inc Bit") as AudioClip 
			};
		} else {
			clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Move and Twist 1") as AudioClip };
		}
		SendPlayVoices (clips);
		nextTime = Time.time + time;
	}
	// TODO: Nudge line
	void partTwo() {
		Debug.Log (times_failed);
		if (one_moved && two_moved && three_moved) {
			List<AudioClip> clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Reengage Tr Proto") as AudioClip }; 
			SendPlayVoices (clips);
			nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
		} else if (times_failed < 2) {
			times_failed += 1;
			currentPart = 1;
			partOne (15f, true);
		} else {
			List<AudioClip> clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Reengage Tr Proto") as AudioClip }; 
			SendPlayVoices (clips);
			nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
		}
	}

	void partThree() {
		SendEndScriptPart ();
	}
	public override void BoxOneStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = true;
		Debug.Log ("Playing motion1");
		one_moved = true;
	}
	public override void BoxOneStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = false;
	}

	public override void BoxTwoStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = true;
		Debug.Log ("Playing motion2");
		two_moved = true;
	}
	public override void BoxTwoStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = false;
	}


	public override void BoxThreeStartMoving(double speed) {
		gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Playing motion3");
		three_moved = true;
	}
	public override void BoxThreeStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = false;
	}
	// TODO: Handle continuemoving and use speed
}
