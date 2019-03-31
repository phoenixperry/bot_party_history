using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AlanZucconi.Arduino;

public class Phoenixino : MonoBehaviour
{

    public ArduinoConnector Connector;
    public ArduinoThread Arduino;

    // Start is called before the first frame update
    IEnumerator Start()
    {

        Connector.OpenStream();

        yield return new WaitForSeconds(1);

        //while (true)
        //{
        //    string s = Connector.ReadFromArduino();
        //    Debug.Log(s);

        //    yield return null;
        //}

        
        Arduino.StartThread();


        while (true)
        {
            Arduino.WriteToArduino("xxx");
            //Debug.Log("xxx"); 
            yield return Arduino.WaitForMessage(0.25f);
            string s = Arduino.ReadFromArduino();
            Debug.Log(s); 
            yield return null;
        }
    }
    	void OnDisable() {
            Arduino.Arduino.CloseStream();

        Connector.CloseStream();
		}
}
