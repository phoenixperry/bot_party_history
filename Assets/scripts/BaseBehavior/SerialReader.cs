using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;



//public struct Colors {
 //   public static Color     
//} 

public class SerialReader : AbstractInputReader
{

    //opens a queue of bites, which is the opposite of a stack. It's first in first out. Just like a line, you deal with the data in the order it shows up. 
    Queue<byte[]> writeQueue;
    
	SerialPort stream; //serial port data 

	MenuButtonState menu_state;

    string incommingData; //data coming in through the port 

    //list the port names and return the first serial port in the stack of possible serial ports on your machine, this is usually your arduino
	string getSerialPort() {
		string[] ports = SerialPort.GetPortNames ();
		if (ports.Length == 0) {
			Debug.Log ("No serial port found.");
			return "";
		}
		return ports [0]; // TODO: At present this uses the first found serial input. --cap
		}

    //this update function simply checks if anything needs to be written. 
	void Update() {
		checkForWrites (); //makes sure if there's data in our writeQueue, it sends. 
	}
	void checkForWrites() {
		while (writeQueue.Count > 0 && stream != null) {
			stream.Write (writeQueue.Dequeue(),0,2); //this sends the first byte in the writeQueue, it starts with the first byte in the buffer and sends 2 bytes of data. we are sending only 2 bytes to arduino this way to save memory and increase speed. 
		}
	}

	void OnEnable() {
		base.OnEnable (); //calls the base class enable function
		OnWriteToSerial += queueWrite; //calls queueWrite when the OnWriteSerial event is called. 
		writeQueue = new Queue<byte[]>();
		string port = getSerialPort (); //gets the first port at position 0. 
		if (port == "") { 
		Debug.Log ("Terminating stream enable...\n (Hint: Hit semicolon (;) to switch to keyboard input.)");
				return; // TODO: The case of there not being input is handled a little inelegantly. --cap
		}
			stream = new SerialPort (port, 115200); //opens the serial port 
			stream.WriteTimeout = 1000; //this is long. - we might want to test this. 
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
        //this coroutine starts up when the serial port is opened successfully. It reads the data coming in from arduino and sends it to the right data sctructures.  
		public void startProcessCoroutine() {
		StartCoroutine
		(
		AsynchronousReadFromArduino
		(incommingData =>
		{
		//Debug.Log(incommingData);
		string [] sensors = incommingData.Split(' ');
		if (sensors.Length > 1 && sensors.Length < 3)
		{
		passOnTouch(new TouchedBots(sensors[0], sensors[1])); //creates a new touchedBots struct and passes in data.  
	

		}
		else if (sensors.Length == 6)
		{


		//if (OnBotDataReceived != null) {
		passOnBotDataReceived(new Bot(sensors[0], sensors[1], sensors[2], sensors[3], sensors[4], sensors[5])); 
		} 
		else if (sensors.Length == 3) {
			// Menu Button update
			MenuButtonState newMenu = new MenuButtonState(sensors[1], sensors[2]);
			if (menu_state.def) {
				if (newMenu.oc && !menu_state.oc) {
					MenuFreePlay();
				} 

				if (newMenu.slc && !menu_state.slc) {
					MenuSecretCiphers();
				} 
			}
				menu_state = newMenu;
		}



		},     // Callback
		() => Debug.LogError("Error!"), // Error callback
		10000f                          // Timeout (milliseconds)
		)
		);
		}
		
    //queues up data to write to the serial port 
	public void queueWrite(byte[] wri)
	{
		writeQueue.Enqueue (wri);
	}
    //function which is called when the coroutine starts up.  It fires off the call back function and returns data if it can read something from the port. 
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
