﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioHelm;

public class FreePlayManager : AbstractManager {
	public ArrayList touchsound = new ArrayList();
	public AudioClip onetwo, twothree, onethree, allthree;
	public GameObject btnInterface;
	void Start() {
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
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxOneButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxTwoButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxTwoButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxThreeButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxThreeButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxOneStartMoving(double speed)
	{
		BoxOneContinueMoving (speed);
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: Moving";
	}

	public override void BoxOneContinueMoving(double speed)
	{
		// So this plays notes proportional to speed of box one from the octave C3 to C4.
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 48 = C3. 
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().NoteOn ((int)(48 + Mathf.Max(0f,(float)(speed-5))*2));
	}

	public override void BoxOneStopMoving()
	{
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().AllNotesOff();
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
}
