﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoSceneManager : AbstractSceneManager {
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
		button.onClick.Invoke ();
	}
	public override void BoxOneButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		button.CancelInvoke ();
	}

	public override void BoxTwoButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		button.onClick.Invoke ();
	}
	public override void BoxTwoButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		button.CancelInvoke ();
	}

	public override void BoxThreeButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		button.onClick.Invoke ();
	}
	public override void BoxThreeButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		button.CancelInvoke ();
	}

	public override void BoxOneStartMoving()
	{
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: Moving";
	}

	public override void BoxOneStopMoving()
	{
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: At rest";
	}

	public override void BoxTwoStartMoving()
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: Moving";
	}

	public override void BoxTwoStopMoving()
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: At rest";
	}

	public override void BoxThreeStartMoving()
	{
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: Moving";
	}

	public override void BoxThreeStopMoving()
	{
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: At rest";
	}
}
