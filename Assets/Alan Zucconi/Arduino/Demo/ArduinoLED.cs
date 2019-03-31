using UnityEngine;
using AlanZucconi.Arduino;

public class ArduinoLED : MonoBehaviour
{
    public ArduinoThread Arduino;

	void Start ()
    {
        Arduino.StartThread();
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ON");
        Arduino.WriteToArduino("ON");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OFF");
        Arduino.WriteToArduino("OFF");
    }
}
