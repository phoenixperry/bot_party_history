using UnityEngine;
using AlanZucconi.Arduino;

public class ArduinoReader : MonoBehaviour
{
    public ArduinoThread Arduino;

	void Start ()
    {
        Arduino.StartThread();
	}
	
	// Update is called once per frame
	void Update ()
    {
        string message = Arduino.ReadFromArduino();
        if (message != null)
            Debug.Log(message);
	}
}
