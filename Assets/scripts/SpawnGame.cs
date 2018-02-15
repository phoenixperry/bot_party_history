using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGame : MonoBehaviour {
    public GameObject serialDataManager;
	public GameObject keyboardDataManager;
    public GameObject botDataManager;
    public GameObject touchManager;

	public bool serial;

    // Use this for initialization
    void Start () {
		serialDataManager = Instantiate (serialDataManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		//serialDataManager.SetActive (false);
		keyboardDataManager = Instantiate (keyboardDataManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		//keyboardDataManager.SetActive (false);
		botDataManager = Instantiate(botDataManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		touchManager = Instantiate (touchManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		serial = true;
		switchControls (serial);
	}

	void switchControls(bool serial) {
		serialDataManager.SetActive (serial);
		keyboardDataManager.SetActive(!serial);
		if (serial) {
			Debug.Log ("Switched to Serial Control");
		} else {
			Debug.Log ("Switched to Keyboard Control");
		}
	} 
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Semicolon)) {
			serial = !serial;
			switchControls (serial);
		}
	}
}
