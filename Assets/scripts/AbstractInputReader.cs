using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Bot {
	public Bot(string name_, string compass_, string xpos_, string ypos_, string zpos_, string btn_) {
		name = name_; compass = compass_; xpos = xpos_; ypos = ypos_; zpos = zpos_; btn = btn_;
	}

	public void printBotData() {
		Debug.Log ("Bot: " + name + ", compass: " + compass + " x,y,z = " + xpos + "," + ypos + "," + zpos + " btn = " + btn);
	}
	public string name;
	public string compass;
	public string xpos;
	public string ypos;
	public string zpos;
	public string btn;
}

public struct TouchedBots {
	public TouchedBots(string touch1, string touch2) {
		botsTouched = touch1;
		touch = touch2;
	}

	public void printTouchData() {
		Debug.Log ("Touching: " + touch + " and " + botsTouched);
	}
	public string botsTouched;
	public string touch; 
}

public class AbstractInputReader : MonoBehaviour {
	protected Bot b;
	protected TouchedBots touchedBots;
	public delegate void BotDataReceived(Bot b_);
	public static event BotDataReceived OnBotDataReceived;

	public delegate void TouchManagerReceived(TouchedBots t_);
	public static event TouchManagerReceived OnTouch;

	protected void passOnBotDataReceived(Bot b_) {
		if (OnBotDataReceived != null) OnBotDataReceived (b_);
	}

	protected void passOnTouch(TouchedBots t_) {
		if (OnTouch != null) OnTouch (t_);
	}
		

	// Update is called once per frame
	void Update () {
		
	}
}
