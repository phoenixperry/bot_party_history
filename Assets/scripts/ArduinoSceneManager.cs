using System.Collections;
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
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
	}
	public override void BoxOneButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;	
	}

	public override void BoxTwoButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
	}
	public override void BoxTwoButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;	
	}

	public override void BoxThreeButtonDown ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
	}
	public override void BoxThreeButtonUp ()
	{
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;	
	}
}
