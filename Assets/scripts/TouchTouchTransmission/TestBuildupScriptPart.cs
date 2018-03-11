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
		} else if (currentPart == 4 && nextTime <= Time.time) {
			partFive ();
		}
	}
	public override void targetSuccess() {
		if (currentPart == 1) {
			SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/success-ping") as AudioClip);
			SendNewTarget (TouchState.None, 30, 1);
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		}
	}
	public override void targetFailure() {
		if (currentPart == 1) {
			SendPlayGameSound (Resources.Load ("TouchTouchTransmission/capage-drafts/screech") as AudioClip);
			SendNewTarget (TouchState.None, 30, 1);
		}	
		// Deal with Just One More Time Human
	}
	void partOne() {
		nextTime = Time.time + 60;
		//SendPlayVoice(Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-start") as AudioClip);
		SendNewTarget (TouchState.None, 70, 1);
	}
	void partTwo() {
		SendClearTargets ();
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Transmiss Compl") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/Just One More TIme HUm") as AudioClip
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partThree() {
		SendNewTarget (TouchState.AllConnected, 255, 0);
	}
	void partFour() {
		List<AudioClip> clips = new List<AudioClip> () {
			Resources.Load ("TouchTouchTransmission/capage-drafts/test-transmission-end") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/You Broadcast") as AudioClip,
		};
		clips.AddRange(TouchTouchTransmission.numberToClips (getScore()));
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Gigabytes 1") as AudioClip);
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Translation 1") as AudioClip);
		clips.AddRange(TouchTouchTransmission.numberToClips (getScore()));
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Points") as AudioClip);
		if (getScore () < getScoreWin ()) {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Bitrate Low 1") as AudioClip);
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Recon Hum Int Train Proto") as AudioClip);

		} else if (getScore () > getScoreWin () * 1.5) {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Hu Op Efficiency") as AudioClip);
		} else {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Hu Perf Adeq 1") as AudioClip);
		}
		// Resetting system...
		SendPlayVoices(clips);
		SendClearTargets ();
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partFive() {
		SendTerminate();
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