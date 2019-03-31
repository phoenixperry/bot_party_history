using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO.Ports;


public enum ComPort
{
    COM0,
    COM1,
    COM2,
    COM3,
    COM4,
    COM5,
    COM6,
    COM7,
    COM8,
    COM9,
}

public enum BaudRate
{
    Baud300 = 300,
    Baud600 = 600,
    Baud1200 = 1200,
    Baud2400 = 2400,
    Baud4800 = 4800,
    Baud9600 = 9600,
    Baud14400 = 14400,
    Baud19200 = 19200,
    Baud28800 = 28800,
    Baud38400 = 38400,
    Baud57600 = 57600,
    Baud115200 = 115200
}

namespace AlanZucconi.Arduino
{
    public class ArduinoConnector : MonoBehaviour
    {
        [Header("Serial Communication")]
        public ComPort Port = ComPort.COM3;
        public BaudRate BaudRate = BaudRate.Baud115200;

        [Space]
        public int ReadTimeout = 50; // millisecond
        public int WriteTimeout = 50; // millisecond

        private SerialPort Stream = null;

        public bool WriteToArduino(string message, bool flush = true)
        {
            // Stream not open
            if (Stream == null ||
                !Stream.IsOpen)
                return false;

            // Writes
            try
            {
                Stream.WriteLine(message);
            }
            catch (TimeoutException exception)
            {
                return false;
            }

            if (flush)
                Stream.BaseStream.Flush();

            return true;
        }


        public string ReadFromArduino()
        {
            // Stream not open
            if (Stream == null ||
                !Stream.IsOpen)
                return null;

            try
            {
                // Attemps a read
                return Stream.ReadLine();
            }
            catch (TimeoutException exception)
            {
                // Error!
                //Debug.Log(exception); 
                return null;
            }
        }





        public bool OpenStream()
        {
            // Stream already open
            if (Stream != null)
                return false;

            // Configures the serial port
            Stream = new SerialPort
            (
                Port.ToString(),
                (int)BaudRate
            );

            Stream.ReadTimeout = ReadTimeout;
            Stream.WriteTimeout = WriteTimeout;
            Stream.Open();

            return true;
        }

        public bool CloseStream()
        {
            // Stream already open
            if (Stream == null || !Stream.IsOpen)
            {
                return false;
            }
            else
            {
                Stream.BaseStream.Close();
                Stream.Close();

                Stream = null;

                return true;
            }
        }
    }
}