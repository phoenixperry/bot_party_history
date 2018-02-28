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

public enum LED_CHANGES {None=0, On, Off, Set, FadeOn, FadeOff};
public class AbstractInputReader : MonoBehaviour {
	public delegate void WriteToSerial(byte[] wri); //all methods that subscribe to this delegate must be void and pass in no data 
	public static event WriteToSerial OnWriteToSerial; //this is the event to register your functions to 

	public void HandleLEDChange(int led, LED_CHANGES type, int parameter) {
		byte first = (byte) (((byte) led) << 6);
		if (type == LED_CHANGES.On) {
			first += 32;
		} else if (type == LED_CHANGES.Off) {
			first += 16;
		} else if (type == LED_CHANGES.Set) {
			first += 8;
		} else if (type == LED_CHANGES.FadeOn) {
			first += 4;
		} else if (type == LED_CHANGES.FadeOff) {
			first += 2;
		}

		passWrite(new byte[] { first, (byte)parameter});
	}

	public static void passWrite(byte[] wri) {
		if (OnWriteToSerial != null) {
			OnWriteToSerial (wri);
		}
	}

	public void OnEnable() {
		AbstractManager.DoLEDChange += HandleLEDChange;
	}
	public void OnDisable() {
		AbstractManager.DoLEDChange -= HandleLEDChange;
	}
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
