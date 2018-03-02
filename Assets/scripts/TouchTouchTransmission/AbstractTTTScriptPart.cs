using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractTTTScriptPart : MonoBehaviour {
	public delegate void EndScriptPart();
	public static event EndScriptPart OnEndScriptPart;

	public delegate void ClearTargets();
	public static event ClearTargets OnClearTargets;

	public delegate void NewTarget(TouchState target, int duration);
	public static event NewTarget OnNewTarget;

	public delegate void PlayVoice(AudioClip clip);
	public static event PlayVoice OnPlayVoice;

	public delegate void PlayGameSound(AudioClip clip);
	public static event PlayGameSound OnPlayGameSound;

	protected void OnDisable() {
		SendClearTargets ();
	}

	protected void SendPlayGameSound(AudioClip clip) {
		if (OnPlayGameSound != null) {
			OnPlayGameSound (clip);
		}
	}

	protected void SendPlayVoice(AudioClip clip) {
		if (OnPlayVoice != null) {
			OnPlayVoice (clip);
		}
	}

	protected void SendEndScriptPart() {
		if (OnEndScriptPart != null) {
			OnEndScriptPart ();
		}
	}
	protected void SendClearTargets() {
		if (OnClearTargets != null) {
			OnClearTargets();
		}
	}
	protected void SendNewTarget(TouchState target, int duration) {
		if (OnNewTarget != null) {
			OnNewTarget (target, duration);
		}
	}

	public virtual void startPart() {

	}
	public virtual void stopPart() {

	}
	public virtual void targetSuccess() {

	}
	public virtual void targetFailure() {

	}
	public virtual void targetCleared() {

	}
}
