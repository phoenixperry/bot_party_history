using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractTTTScript : MonoBehaviour {

	public List<AbstractTTTScriptPart> scriptParts;
	int currentPart = 0;

	public void targetSuccess() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetSuccess ();
		}
	}
	public void targetFailure() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetFailure ();
		}
	}
	public void targetCleared() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetCleared ();
		}
	}

	public void startNextScript() {
		Debug.Log ("startNextScript");
		stopCurrentScript ();
		currentPart += 1;
		startCurrentScript ();
	}
	public void stopCurrentScript() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].stopPart ();
		}
	}
	public void startCurrentScript() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].startPart ();
		}
	}
}
