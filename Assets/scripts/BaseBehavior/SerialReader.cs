using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System;
using Unity.Collections;
using Unity.Jobs; 


//public struct Colors {
//   public static Color     
//} 

public class SerialReader : AbstractInputReader
{

    //opens a queue of bites, which is the opposite of a stack. It's first in first out. Just like a line, you deal with the data in the order it shows up. 
    public Queue<byte[]> writeQueue;
    MenuButtonState menu_state;

    SerialPort stream = null; //serial port data 

    string incommingData; //data coming in through the port 

    bool openStream; //is the SerialPort Open? 

    //list the port names and return the first serial port in the stack of possible serial ports on your machine, this is usually your arduino
    string getSerialPort()
    {

        string[] ports = SerialPort.GetPortNames();
        if (ports.Length == 0) {
            Debug.Log("No serial port found.");
            return "";
        }
        return ports[0]; // TODO: At present this uses the first found serial input. --cap
    }


    public bool OpenStream()
    {

        string port = getSerialPort(); //gets the first port at position 0. 

        if (port == "")
        {
            Debug.Log("Terminating stream enable...\n (Hint: Hit semicolon (;) to switch to keyboard input.)");
            return false; // TODO: The case of there not being input is handled a little inelegantly. --cap
        }

        stream = new SerialPort(port, 115200); //opens the serial port 
        stream.WriteTimeout = 10; //this is long. - we might want to test this. 
        stream.ReadTimeout = 10; // Need to nicely handle this.
        Debug.Log("Opening stream...");
        stream.Open();
        return true;
    }

    public bool CloseStream()
    {
        // Stream already open
        if (stream == null || !stream.IsOpen)
        {
            return false;
        }
        else
        {
            stream.BaseStream.Close();
            stream.Close();

            stream = null;

            return true;
        }
    }

    public string ReadDataFromArduino()
    {
        // Stream not open
        if (stream == null ||
            !stream.IsOpen)
            return null;

        try
        {
            // Attemps a read
            return stream.ReadLine();
        }
        catch (TimeoutException exception)
        {
            // Error!
            Debug.Log(exception);
            return null;
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

    void checkForWrites()
    {
        while (writeQueue.Count > 0 && stream != null)
        {
            stream.Write(writeQueue.Dequeue(), 0, 2); //this sends the first byte in the writeQueue, it starts with the first byte in the buffer and sends 2 bytes of data. we are sending only 2 bytes to arduino this way to save memory and increase speed. 
        }
    }

    //queues up data to write to the serial port
    public void queueWrite(byte[] wri)
    {
        writeQueue.Enqueue(wri);
    }

    //this update function simply checks if anything needs to be written. 
    void Update()
    {
        checkForWrites(); //makes sure if there's data in our writeQueue, it sends. 
 
    }
    void OnEnable()
    {
        base.OnEnable(); //calls the base class enable function

        string testString = "this is a test that will be converted";

        byte[] testBytesToConvert = System.Text.Encoding.ASCII.GetBytes(testString);

        string testStringTranslated = System.Text.Encoding.ASCII.GetString(testBytesToConvert);

        Debug.Log(testStringTranslated);

        Debug.Log( Unity.Collections.LowLevel.Unsafe.UnsafeUtility.IsBlittable<nByteArray>());

        OnWriteToSerial += queueWrite; //calls queueWrite when the OnWriteSerial event is called. 
        writeQueue = new Queue<byte[]>();
        //openStream = OpenStream(); 

        //if(openStream)
        //{ 
        InvokeRepeating("handleData",0, 0.001f);
        //Debug.Log("Starting stream coroutine...");
        //}

    }

    //function which is called when the coroutine starts up.  It fires off the call back function and returns data if it can read something from the port.
    string data=""; 
    public void handleData()
    {

        byte[] dataForJob = System.Text.Encoding.ASCII.GetBytes(data);
        byte[][] writeQueueForJob = writeQueue.ToArray();
        var job = new ArduinoHandler(dataForJob, openStream, writeQueueForJob);

        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();
        writeQueue.Clear();


        if (data != null)
            {
            Debug.Log(data);
            string[] dataStrings = SplitIncomingDataToStrings(data);
            SetIncomingDataToGameData(dataStrings);
            }
            
            //if (openStream)
            //{
            //    data = ReadDataFromArduino();
            //    Debug.Log("I AM HERE");
            //    if (data != null)
            //    {
            //        Debug.Log(data);
            //        string[] dataStrings = SplitIncomingDataToStrings(data);
            //        SetIncomingDataToGameData(dataStrings);
            //    }
            //}
     }

    void OnDisable()
    {
        base.OnDisable();
        OnWriteToSerial -= queueWrite;
        CancelInvoke("handleData");
      //  CloseStream(); 
    }

    public struct nString
    {
        public nString(string value)
            : this()
        {
            Value = value ?? "N/A";
        }

        public string Value
        {
            get;
            private set;
        }

        public static implicit operator nString(string value)
        {
            return new nString(value);
        }

        public static implicit operator string(nString value)
        {
            return value.Value;
        }
    }

    public struct nQueueByte
    {
        public nQueueByte(Queue<byte[]> value)
            : this()
        {
            Value = value ?? new Queue<byte[]>();
        }

        public Queue<byte[]> Value
        {
            get;
            private set;
        }

        public static implicit operator nQueueByte(Queue<byte[]> value)
        {
            return new nQueueByte(value);
        }

        public static implicit operator Queue<byte[]>(nQueueByte value)
        {
            return value.Value;
        }
    }

    public struct nByteArray
    {
        public nByteArray(System.Byte[] value)
            : this()
        {
            Value = value ?? new System.Byte[] { 0x20 } ;
        }

        public System.Byte[] Value
        {
            get;
            private set;
        }

        public static implicit operator nByteArray(System.Byte[] value)
        {
            return new nByteArray(value);
        }

        public static implicit operator System.Byte[](nByteArray value)
        {
            return value.Value;
        }
    }

    struct ArduinoHandler : IJob
    {

        public NativeArray<byte> data;
        public bool openStream; //is the SerialPort Open? 
        public NativeArray<byte> writeQueue;

        // constructor
        public ArduinoHandler(byte[] dataPassed, bool openStreamPassed, byte[][] writeQueuePassed)
            : this()
        {
            // creating data string
            data = new NativeArray<byte>(dataPassed.Length, Allocator.Persistent);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = dataPassed[i];
            }

            // creating openstream flag
            openStream = openStreamPassed;

            // creating writequeue 
            writeQueue = new NativeArray<byte>(writeQueuePassed.Length, Allocator.Persistent);
            int k = 0;
            for (int i = 0; i < writeQueuePassed.Length; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (k < writeQueuePassed.Length) // TO DO WRITE WELL THIS ARRAY, NOW THIS DOES NOT WRITE ALL INFO
                    {
                        writeQueue[k++] = writeQueuePassed[i][j];

                    }
                }
            }

        }

        public void Execute()
        {
            SerialPort stream = null;
            Debug.Log("I spawn!");
            openStream = OpenStream(stream);

            //if (openStream) {
            //    data = ReadDataFromArduino(stream);
            //    checkForWrites(stream); 
            //    CloseStream(stream); 
            //} 
        }
        string getSerialPort()
        {

            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                Debug.Log("No serial port found.");
                return "";
            }
            return ports[0]; // TODO: At present this uses the first found serial input. --cap
        }


        public bool OpenStream(SerialPort stream)
        {

            string port = getSerialPort(); //gets the first port at position 0. 

            if (port == "")
            {
                Debug.Log("Terminating stream enable...\n (Hint: Hit semicolon (;) to switch to keyboard input.)");
                return false; // TODO: The case of there not being input is handled a little inelegantly. --cap
            }

            stream = new SerialPort(port, 115200); //opens the serial port 
            stream.WriteTimeout = 10; //this is long. - we might want to test this. 
            stream.ReadTimeout = 10; // Need to nicely handle this.
            Debug.Log("Opening stream...");
            stream.Open();
            return true;
        }

        public bool CloseStream(SerialPort stream)
        {
            // Stream already open
            if (stream == null || !stream.IsOpen)
            {
                return false;
            }
            else
            {
                stream.BaseStream.Close();
                stream.Close();

                stream = null;

                return true;
            }
        }

        public string ReadDataFromArduino(SerialPort stream)
        {
            // Stream not open
            if (stream == null ||
                !stream.IsOpen)
                return null;

            try
            {
                // Attemps a read
                return stream.ReadLine();
            }
            catch (TimeoutException exception)
            {
                // Error!
                Debug.Log(exception);
                return null;
            }
        }

        public void checkForWrites(SerialPort stream, int lengthQueue)
        {
            //while (writeQueue[0].Value.Count > 0 && stream != null)
            //{
            //    stream.Write(writeQueue[0].Value.Dequeue(), 0, 2); //this sends the first byte in the writeQueue, it starts with the first byte in the buffer and sends 2 bytes of data. we are sending only 2 bytes to arduino 
            //    //this way to save memory and increase speed. 
            //}

            int k = 0;
            for (int i = 0; i < lengthQueue; i++)
            {
                for (int j = 1; j < 3; j++)
                {


                    byte byteFromWriteQueue = writeQueue[k + j];
                    byte[] bytesToSend = new byte[1];
                    bytesToSend[0] = byteFromWriteQueue;
                    string dataToSend = System.Text.Encoding.ASCII.GetString(bytesToSend);

                    stream.Write(dataToSend);
                }

                k += 2;
            }

        }

    }
}

