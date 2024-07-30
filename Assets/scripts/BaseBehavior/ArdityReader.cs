using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System;

public class ArdityReader : AbstractInputReader
{
    MenuButtonState menu_state;
    Queue<string> writeQueue = new Queue<string>();
    public SerialController serialController;

    void checkForWrites()
    {
        while (writeQueue.Count > 0)
        {
            serialController.SendSerialMessage(writeQueue.Dequeue());
        }
    }

    //queues up data to write to the serial port
    public void queueWrite(byte[] wri)
    {
        writeQueue.Enqueue(System.Text.Encoding.ASCII.GetString(wri));
    }

    void Setup() {
    }

    void OnEnable() {
        base.OnEnable();
        OnWriteToSerial += queueWrite;
    }
    void OnDisable()      
    {
        base.OnDisable();
        OnWriteToSerial -= queueWrite;
    }

    //this update function simply checks if anything needs to be written. 
    void Update()
    {
        checkForWrites(); //makes sure if there's data in our writeQueue, it sends. 
    }

    void OnMessageArrived(string msg) {
        Debug.Log("ArdityReader: "+msg);
        string[] data = SplitIncomingDataToStrings(msg);
        SetIncomingDataToGameData(data);
    }

    void OnConnectionEvent(bool success) {
        if (success) {
         Debug.Log("ArdityReader: Connected!");
        } else {
            Debug.Log("ArdityReader: Failed to Connect");
        }
    }

    public string[] SplitIncomingDataToStrings(string incomingSensorData)
    {
        string[] sensors = incomingSensorData.Split(' ');
        return sensors;
    }

    public void SetIncomingDataToGameData(string[] sensors) {
        //this is the touch passes
        //Debug.Log(sensors[0] + "this much data"); 
        if (sensors.Length == 2)
        {
            passOnTouch(new TouchedBots(sensors[0], sensors[1])); //creates a new touchedBots struct and passes in data.  

            Debug.Log(sensors[0] + sensors[1]);
        }
        //this is the accelerometers 
        else if (sensors.Length == 6)
        {

            passOnBotDataReceived(new Bot(sensors[0], sensors[1], sensors[2], sensors[3], sensors[4], sensors[5]));
        }

        //this is menu data
        else if (sensors.Length == 3)
        {
            // Menu Button update
            MenuButtonState newMenu = new MenuButtonState(sensors[1], sensors[2]);
            if (menu_state.def)
            {
                if (newMenu.oc && !menu_state.oc)
                {
                    MenuFreePlay();
                }

                if (newMenu.slc && !menu_state.slc)
                {
                    MenuSecretCiphers();
                }
            }
            menu_state = newMenu;
        }
    }

}

