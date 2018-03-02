using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTTTScript : AbstractTTTScript {


	void OnEnable() {
		scriptParts = new List<AbstractTTTScriptPart> ();
		scriptParts.Add (gameObject.AddComponent<TrainingTTTScriptPart>());
	}
		
}
