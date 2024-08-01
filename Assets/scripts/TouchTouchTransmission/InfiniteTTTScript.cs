using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTTTScript : AbstractTTTScript {


	void OnEnable() {
		scriptParts = new List<AbstractTTTScriptPart> ();
		scriptParts.Add (gameObject.AddComponent<InfiniteTTTScriptPart>());
	}
		
}
