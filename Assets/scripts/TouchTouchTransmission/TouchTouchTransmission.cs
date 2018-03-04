using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchState { None=0, OneTwo, TwoThree, OneThree, AllConnected };
public class TouchTouchTransmission : AbstractManager {
	public AbstractTTTScript script;
	int TO_WIN = 10;
	TouchState target;
	int score = 0;
	float nextTime = 0;
	public AudioClip Sound_Win, Sound_Success, Sound_Fail;
	void OnEnable() {
		base.OnEnable ();
		AbstractTTTScriptPart.OnPlayVoice += playBotVoice;
		AbstractTTTScriptPart.OnNewTarget += addNewTarget;
		AbstractTTTScriptPart.OnClearTargets += clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound += playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart += endScriptPart;
	}
	void OnDisable() {
		base.OnDisable();
		AbstractTTTScriptPart.OnPlayVoice -= playBotVoice;
		AbstractTTTScriptPart.OnNewTarget -= addNewTarget;
		AbstractTTTScriptPart.OnClearTargets -= clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound -= playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart -= endScriptPart;

	}
	void Update() {
		if (target != TouchState.None && nextTime <= Time.time) {
			fail ();
		}
	}
	public void clearTargets() {
		target = TouchState.None;
		gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Target: "+target;
		lightUp (target, 0);
	}
	public void endScriptPart() {
		script.startNextScript ();
	}
	public void playBotVoice(AudioClip clip) {
		AudioSource botSpeaker = gameObject.transform.Find ("BotSpeaker").GetComponent<AudioSource> ();
		botSpeaker.clip = clip;
		botSpeaker.Play ();
	}
	public void playGameSound(AudioClip clip) {
		AudioSource gameSpeaker = gameObject.transform.Find ("GameSpeaker").GetComponent<AudioSource> ();
		gameSpeaker.clip = clip;
		gameSpeaker.Play ();
	}
	public void addNewTarget(TouchState new_target, int duration) {
		if (new_target == TouchState.None) {
			while (new_target == target || new_target == TouchState.None) {
				new_target = (TouchState) (Random.Range (1, 5));
			}
		}
		target = new_target;
		nextTime = Time.time + (duration / 10);
		Debug.Log (target);
		lightUp (target, duration);
		gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Target: "+target;
	}

	void Start() {
		score = 0;
		script = gameObject.AddComponent<TestTTTScript>();
		script.startCurrentScript();
	}
		
	void lightUp(TouchState newTarget, int time) {
		TurnOffLEDOne ();
		TurnOffLEDTwo ();
		TurnOffLEDThree ();
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.OneThree || newTarget == TouchState.OneTwo) {
			TurnOnLEDOne ();
			FadeLEDOneOff (time);
		}
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.TwoThree || newTarget == TouchState.OneTwo) {
			TurnOnLEDTwo ();
			FadeLEDTwoOff (time);
		}
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.OneThree || newTarget == TouchState.TwoThree) {
			TurnOnLEDThree ();
			FadeLEDThreeOff (time);
		}
	}

	void success() {
		script.targetSuccess ();
	}

	void fail() {
		script.targetFailure ();
	}

	void checkMatch(TouchState input) {
		if (!enabled)
			return;

		Debug.Log ("Test match: " + input);
		if (input == target) {
			success ();
			//newTarget ();

		} else if (target == TouchState.AllConnected) {
			// pass. Reason why is because otherwise getting to a three-touch would cause a fail noise.
		} else {
			//fail ();
		}

		if (score >= TO_WIN) {
			enabled = false;
		}
	}

	public override void BoxOneTwoConnected()
	{
		checkMatch (TouchState.OneTwo);
	}

	public override void BoxOneThreeConnected() {
		checkMatch (TouchState.OneThree);
	}

	public override void BoxTwoThreeConnected() {
		checkMatch (TouchState.TwoThree);
	}

	public override void AllConnected() {
		checkMatch (TouchState.AllConnected);
	}
}
