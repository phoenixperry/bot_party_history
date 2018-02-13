using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;

// MAKE SURE THIS IS ON SerialDataManager 
public static class AppHelper
{
#if UNITY_WEBPLAYER
     public static string webplayerQuitURL = "http://google.com";
#endif
    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }
}

//public struct Colors {
 //   public static Color     
//} 

public class SerialReader : AbstractReader
{

    SerialPort stream = new SerialPort("COM5", 115200); //this is for the port you're on = it has to match what arduino is plugged into       

   

    string incommingData;
    // Use this for initialization

    void Start()
    {

        // Get a list of serial port names in case we are not dealing with com3 .
        string[] ports = SerialPort.GetPortNames();

        Debug.Log("The following serial ports were found:");

        // Display each port name to the console just in case you are not using the same com port you did last time. Note on a mac this will be something like a path.
        foreach (string port in ports)
        {
            Debug.Log(port);
        }
        stream.Open();
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
