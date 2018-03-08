using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioHelm;

public class FreePlayManager : AbstractManager {
	public ArrayList touchsound = new ArrayList();
	public AudioClip onetwo, twothree, onethree, allthree;
	public GameObject btnInterface;
	TestMarkovMusic markov_piano;

	void Start() {
		markov_piano = new TestMarkovMusic ();
		touchsound.Add (onetwo);
		touchsound.Add (twothree);
		touchsound.Add (onethree);
		touchsound.Add (allthree);
	}
	public override void BoxOneTwoConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [0] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}
	public override void BoxTwoThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [1] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public override void BoxOneThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [2] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public override void AllConnected() {
		//if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [3] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		//}
	}

	public override void BoxOneButtonDown ()
	{
		TurnOnLEDOne ();
		BoxOneStartMoving (10f);
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxOneButtonUp ()
	{
		TurnOffLEDOne ();
		BoxOneStopMoving ();
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxTwoButtonDown ()
	{
		TurnOnLEDTwo ();
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxTwoButtonUp ()
	{
		TurnOffLEDTwo ();
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxThreeButtonDown ()
	{
		TurnOnLEDThree ();
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxThreeButtonUp ()
	{
		TurnOffLEDThree ();
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxOneStartMoving(double speed)
	{
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: Moving";
		GameObject midibot = gameObject.transform.Find ("Bot1Midi").gameObject;
		midibot.GetComponent<HelmSequencer> ().enabled = true;
		//markov_piano.addNextBeats (1024, midibot.GetComponent<HelmSequencer>());
		midibot.GetComponent<AudioSource> ().Play ();
	}

	public override void BoxOneContinueMoving(double speed)
	{

	}

	public override void BoxOneStopMoving()
	{
		GameObject midibot = gameObject.transform.Find ("Bot1Midi").gameObject;
		midibot.GetComponent<AudioSource> ().Stop ();
		midibot.GetComponent<HelmSequencer> ().Clear ();
		midibot.GetComponent<HelmSequencer> ().currentIndex = -1;
		midibot.GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: At rest";
	}

	public override void BoxTwoStartMoving(double speed)
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: Moving";
		BoxTwoContinueMoving (speed);

	}

	public override void BoxTwoContinueMoving(double speed) {
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 60 = C3
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().NoteOn ((int)(60 + Mathf.Max(0f,(float)(speed-5))*2));
	}

	public override void BoxTwoStopMoving()
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: At rest";
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().AllNotesOff ();
	}

	public override void BoxThreeStartMoving(double speed)
	{
		BoxThreeContinueMoving (speed);
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: Moving";
	}

	public override void BoxThreeContinueMoving(double speed)
	{
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 72 = C5.
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().NoteOn ((int)(72 + Mathf.Max(0f,(float)(speed-5))*2));
	}
	public override void BoxThreeStopMoving()
	{
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: At rest";
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().AllNotesOff ();
	}

	public override void BoxOneStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate1").GetComponent<TextMesh> ().text = "1: Spinning!";
		BoxOneContinueRotating (angular_speed);
	}
	public override void BoxOneContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);
	}
	public override void BoxOneStopRotating() {
		gameObject.transform.Find ("Rotate1").GetComponent<TextMesh> ().text = "1: Not Spinning!";
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}

	public override void BoxTwoStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate2").GetComponent<TextMesh> ().text = "2: Spinning!";
	}
	public override void BoxTwoContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);

	}
	public override void BoxTwoStopRotating() {
		gameObject.transform.Find ("Rotate2").GetComponent<TextMesh> ().text = "2: Not Spinning!";
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}

	public override void BoxThreeStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate3").GetComponent<TextMesh> ().text = "3: Spinning!";
	}
	public override void BoxThreeContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);
	}
	public override void BoxThreeStopRotating() {
		gameObject.transform.Find ("Rotate3").GetComponent<TextMesh> ().text = "3: Not Spinning!";
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}
}
