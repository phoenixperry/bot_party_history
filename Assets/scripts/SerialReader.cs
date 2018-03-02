﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;



//public struct Colors {
 //   public static Color     
//} 

public class SerialReader : AbstractInputReader
{
	Queue<byte[]> writeQueue;
     //this is for the port you're on = it has to match what arduino is plugged into       

	SerialPort stream;

    string incommingData;
    // Use this for initialization

	string getSerialPort() {
		string[] ports = SerialPort.GetPortNames ();
		if (ports.Length == 0) {
			Debug.Log ("No serial port found.");
			return "";
		}
		return ports [0]; // TODO: At present this uses the first found serial input. --cap
		}

	void Update() {
		checkForWrites ();
	}
	void checkForWrites() {
		while (writeQueue.Count > 0) {
			stream.Write (writeQueue.Dequeue(),0,2);
		}
	}

	void OnEnable() {
		base.OnEnable ();
		OnWriteToSerial += queueWrite;
		writeQueue = new Queue<byte[]>();
		string port = getSerialPort ();
		if (port == "") {
		Debug.Log ("Terminating stream enable...\n (Hint: Hit semicolon (;) to switch to keyboard input.)");
				return; // TODO: The case of there not being input is handled a little inelegantly. --cap
		}
			stream = new SerialPort (port, 115200);
			stream.WriteTimeout = 1000;
			stream.ReadTimeout = 1000; // Need to nicely handle this.
			Debug.Log ("Opening stream...");
			stream.Open ();

		Debug.Log ("Starting stream coroutine...");
			startProcessCoroutine ();
		}

		void OnDisable() {
		base.OnDisable ();
		OnWriteToSerial -= queueWrite;
		if (stream != null) {
			Debug.Log ("Closing stream.");
			stream.Close ();
		}
		}

    void Start()
    {

    }

		public void startProcessCoroutine() {
		StartCoroutine
		(
		AsynchronousReadFromArduino
		(incommingData =>
		{
		//Debug.Log(incommingData);
		string [] sensors = incommingData.Split(' ');
		if (sensors.Length > 1 && sensors.Length < 4)
		{
		//Bot.name = sensors[0];
		//Bot.name = sensors[1];
		//if (OnTouch != null) {
		passOnTouch(new TouchedBots(sensors[0], sensors[1])); 
		//}

		}
		else if (sensors.Length == 6)
		{


		//if (OnBotDataReceived != null) {
		passOnBotDataReceived(new Bot(sensors[0], sensors[1], sensors[2], sensors[3], sensors[4], sensors[5])); 
		//} 


		}

		},     // Callback
		() => Debug.LogError("Error!"), // Error callback
		10000f                          // Timeout (milliseconds)
		)
		);
		}
		

	public void queueWrite(byte[] wri)
	{
		writeQueue.Enqueue (wri);
	}

    public IEnumerator AsynchronousReadFromArduino(System.Action<string> callback, System.Action fail = null, float timeout = float.PositiveInfinity)
    {
        System.DateTime initialTime = System.DateTime.Now;
        System.DateTime nowTime;
        System.TimeSpan diff = default(System.TimeSpan);

        string dataString = null;
        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (System.TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = System.DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }



}
