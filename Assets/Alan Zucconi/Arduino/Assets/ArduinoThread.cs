using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;

namespace AlanZucconi.Arduino
{
    public class ArduinoThread : MonoBehaviour
    {
        public ArduinoConnector Arduino;

        // Thread
        private Thread Thread;

        private Queue OutputQueue;  // From Unity to Arduino
        private Queue InputQueue;   // From Arduino to Unity

        [HideInInspector]
        private bool Looping = true;

        // Last message read from Arduino
        public string LastMessageReceived
        {
            get;
            private set;
        }

        #region StartStop
        public void StartThread()
        {
            // Creates the queues
            OutputQueue = Queue.Synchronized(new Queue());
            InputQueue = Queue.Synchronized(new Queue());

            Looping = true;

            // Creates and starts the thread
            Thread = new Thread(ThreadLoop);
            Thread.Start();
        }

        public void StopThread()
        {
            lock (this)
            {
                Looping = false;
            }
        }

        public bool IsLooping()
        {
            lock (this)
            {
                return Looping;
            }
        }
        #endregion


        public void WriteToArduino(string command)
        {
            OutputQueue.Enqueue(command);
        }

        public string ReadFromArduino()
        {
            if (InputQueue.Count == 0)
                return null;

            return (string) InputQueue.Dequeue();
        }

        private void ThreadLoop()
        {

            Arduino.OpenStream();

            // Looping
            while (IsLooping())
            {
                // Send to Arduino
                // (if there is any message)
                if (OutputQueue.Count != 0)
                {
                    string command = OutputQueue.Dequeue() as string;
                    Arduino.WriteToArduino(command);
                }

                // Read from Arduino
                string result = Arduino.ReadFromArduino();
                if (result != null)
                {
                    InputQueue.Enqueue(result);
                    LastMessageReceived = result;
                }
            }

            Arduino.CloseStream();
        }



        // Custom Yield
        public CustomYieldInstruction WaitForMessage (float timeout = 1f)
        {
            // Waits for reply
            float timeStart = Time.realtimeSinceStartup;
            return new WaitUntil
            (
                delegate ()
                {
                    // Timeout?
                    float timeNow = Time.realtimeSinceStartup;
                    if (timeNow > timeStart + timeout)
                        return true;

                    // A message has arrived
                    return InputQueue.Count != 0;
                }
            );
        }
    }
}