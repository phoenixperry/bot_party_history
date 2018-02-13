using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoSceneManager : AbstractSceneManager {
	public ArrayList touchsound = new ArrayList();
	public AudioClip onetwo, twothree, onethree, allthree;
	public void OnEnable()
	{
		TouchManager.OnBoxOneTwoTouched += BoxOneTwoConnected;
		TouchManager.OnBoxOneTwoReleased += BoxOneTwoReleased;

		TouchManager.OnBoxOneThreeTouched += BoxOneThreeConnected;
		TouchManager.OnBoxOneThreeReleased += BoxOneThreeReleased;

		TouchManager.OnBoxTwoThreeTouched += BoxTwoThreeConnected;
		TouchManager.OnBoxTwoThreeReleased += BoxTwoThreeReleased;

		TouchManager.OnAllBoxesConnected += AllConnected;
		TouchManager.OnAllBoxesReleased += AllReleased;
	}

	void Start() {
		touchsound.Add (onetwo);
		touchsound.Add (twothree);
		touchsound.Add (onethree);
		touchsound.Add (allthree);
	}
	public void BoxOneTwoConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [0] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}
	public void BoxTwoThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [1] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public void BoxOneThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [2] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public void AllConnected() {
		Debug.Log ("AllConnected!");
		//if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [3] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		//}
	}
}
