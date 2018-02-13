﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanGame : MonoBehaviour {
    public GameObject serialDataManager;
	public GameObject keyboardDataManager;
    //public GameObject botDataManager;
    public GameObject touchManager;

    // Use this for initialization
    void Start () {

        serialDataManager = Instantiate(serialDataManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		//serialDataManager.SetActive (false);
		keyboardDataManager = Instantiate (keyboardDataManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		keyboardDataManager.SetActive (false);
		// botDataManager   = Instantiate(botDataManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        touchManager = Instantiate(touchManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
