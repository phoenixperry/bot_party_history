using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchState { None=0, OneTwo, TwoThree, OneThree, AllConnected };
public class TouchTouchTransmission : AbstractManager {
	public AbstractTTTScript script;
	TouchState target;
	static float DELAY_BETWEEN_CLIPS = 0.2f;
	int score = 0;
	int TO_WIN = 40;
	float nextTime = 0;
	public AudioClip Sound_Win, Sound_Success, Sound_Fail;
	public static float getTotalTimeToPlay(List<AudioClip> clips) {
		float time = -DELAY_BETWEEN_CLIPS;
		foreach (AudioClip c in clips) {
			time += c.length + DELAY_BETWEEN_CLIPS;
		}
		return time;
	}
	void OnEnable() {
		base.OnEnable ();
		AbstractTTTScriptPart.OnTerminate += terminate;
		AbstractTTTScriptPart.OnUpdateScore += provideScoreUpdate;
		AbstractTTTScriptPart.OnPlayVoices += playBotVoices;
		AbstractTTTScriptPart.OnPlayVoice += playBotVoice;
		AbstractTTTScriptPart.OnNewTarget += newTarget;
		AbstractTTTScriptPart.OnClearTargets += clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound += playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart += endScriptPart;
	}
	void OnDisable() {
		base.OnDisable();
		AbstractTTTScriptPart.OnTerminate += terminate;
		AbstractTTTScriptPart.OnUpdateScore += provideScoreUpdate;
		AbstractTTTScriptPart.OnPlayVoices -= playBotVoices;
		AbstractTTTScriptPart.OnPlayVoice -= playBotVoice;
		AbstractTTTScriptPart.OnNewTarget -= newTarget;
		AbstractTTTScriptPart.OnClearTargets -= clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound -= playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart -= endScriptPart;

	}
	void terminate() {
		Debug.Log ("TERMINATE HIT");
		MenuManager.Menu ();
	}
	void Update() {
		if (target != TouchState.None && nextTime <= Time.time) {
			fail ();
			target = TouchState.None;
		}
	}
	public void provideScoreUpdate() {
		script.updateScore (new QuickTuple<int, int> (score, TO_WIN));
	}
	public void clearTargets() {
		// This needs to clear ones that are in the addTarget queue too
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
	public void playBotVoices(List<AudioClip> clips) {
		GameObject botSpeaker = gameObject.transform.Find ("BotSpeaker").gameObject;
		float time = (float) AudioSettings.dspTime; 
		foreach (AudioClip c in clips) {
			Debug.Log ("Playing clip in " + time);
			AudioSource newSpeaker = botSpeaker.AddComponent<AudioSource> ();
			newSpeaker.clip = c;
			newSpeaker.PlayScheduled(time);
			time += c.length + DELAY_BETWEEN_CLIPS;
		}
	}
	public void playGameSound(AudioClip clip) {
		AudioSource gameSpeaker = gameObject.transform.Find ("GameSpeaker").GetComponent<AudioSource> ();
		gameSpeaker.clip = clip;
		gameSpeaker.Play ();
	}
	public void newTarget(TouchState new_target, int duration, float pause) {
		clearTargets ();
		//Pause goes here

		StartCoroutine(targetAddWrapper(new_target, duration,pause)); // I hate I have to do this and not invoke, but...
	}
	IEnumerator targetAddWrapper(TouchState new_target, int duration, float pause) {
		yield return new WaitForSeconds(pause);

		addNewTarget (new_target, duration);
	}
	public void addNewTarget(TouchState new_target, int duration) {
		if (new_target == TouchState.None) {
			while (new_target == target || new_target == TouchState.None) {
				new_target = (TouchState) (Random.Range (1, 5));
			}
		}
		target = new_target;
		nextTime = Time.time + (duration / 10);
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

		if (input == target) {
			success ();
			//newTarget ();

		} else if (target == TouchState.AllConnected) {
			// pass. Reason why is because otherwise getting to a three-touch would cause a fail noise.
		} else {
			//fail ();
		}

	//if (score >= TO_WIN) {
	//		enabled = false;
	//	}
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

	public override void BoxOneStartMoving(double speed) {
		script.BoxOneStartMoving (speed);
	}
	public override void BoxOneContinueMoving(double speed) {
		script.BoxOneContinueMoving (speed);
	}
	public override void BoxOneStopMoving() {
		script.BoxOneStopMoving ();
	}

	public override void BoxTwoStartMoving(double speed) {
		script.BoxTwoStartMoving (speed);
	}
	public override void BoxTwoContinueMoving(double speed) {
		script.BoxTwoContinueMoving (speed);
	}
	public override void BoxTwoStopMoving() {
		script.BoxTwoStopMoving ();
	}

	public override void BoxThreeStartMoving(double speed) {
		script.BoxThreeStartMoving (speed);
	}
	public override void BoxThreeContinueMoving(double speed) {
		script.BoxThreeContinueMoving (speed);
	}
	public override void BoxThreeStopMoving() {
		script.BoxThreeStopMoving ();
	}
}
