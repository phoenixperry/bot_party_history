using UnityEngine;
using System.Collections;
using startechplus.ble;
using UnityEngine.UI;

namespace startechplus.ble.examples
{
	/**
	 * An example uBluetoothLe pluging application. In the Start() function the IBleBridge object is instantiated based on whether the Unity Player is running on Android or iOS.  
	 * Once started all interaction with the Bluetooth Device via the IBleBridge is handled via Action() callbacks.  These callbacks are set when the varioius IBleBridge functions are called.
	 * These callbacks are created by you, see the examples in this file.  For details about the Action() callbakcs see IBleBridge.cs.
	 * @see Start()
	 */ 
	public class BlueRx : MonoBehaviour {

		public Text logText;
		public ScrollRect scrollRect;

		public Toggle accelToggleCh1;
		public Toggle invertToggleCh1;
		public Slider sliderCh1;

		public Toggle accelToggleCh2;
		public Toggle invertToggleCh2;
		public Slider sliderCh2;

		public float centerOffsetCh1 = 25;
		public float centerOffsetCh2 = 25;

		public float rangeCh1 = 205;
		public float rangeCh2 = 205;

		public Text debugText;

		/**
		 * Connected to the Scan button in the Unity Editor.
		 */
		public void Scan()
		{
			updateLog("Applicaton: Scanning for ble devices...");

			deviceId = null;
			bleBridge.ScanForPeripheralsWithServiceUUIDs(null, this.DiscoveredPeripheralAction);

		}

		/**
		 * Connected to the Connect button in the Unity Editor.
		 */
		public void Connect()
		{
			if(deviceId != null)
			{
				bleBridge.ConnectToPeripheralWithIdentifier(deviceId, this.ConnectedPeripheralAction, this.DiscoveredServiceAction, 
				                                            this.DiscoveredCharacteristicAction, this.DiscoveredDescriptorAction, this.DisconnectedPeripheralAction);
		
			}
			else
			{
				updateLog("Applicaton: BlueRx not found...");
			}

		}

		/**
		 * Connected to the Disconnect button in the Unity Editor.
		 */
		public void Disconnect()
		{
			if(deviceId != null)
				bleBridge.DisconnectFromPeripheralWithIdentifier(deviceId, this.DisconnectedPeripheralAction);
			else
				updateLog("Applicaton: BlueRx not found...");
		}

		/**
		 * Connected to the various GUI elements in the Unity Editor
		 */ 
		public void onValueChanged()
		{
			ch1IsInverted = invertToggleCh1.isOn;
			ch2IsInverted = invertToggleCh2.isOn;

			ch1IsAccel = accelToggleCh1.isOn;
			ch2IsAccel = accelToggleCh2.isOn;

			ch1RawValue = sliderCh1.value;
			ch2RawValue = sliderCh2.value;

		}

		private string deviceId;

		private string blueRxServiceUuid = "e4377d72-3993-43d7-a941-cd96530783a4".ToUpper();

		private string fastServoControlUuid = "32094D1D-3BE7-4C4E-B14C-4BFD6EA57739";

		private bool ch1IsInverted = false;
		private bool ch2IsInverted = false;

		private bool ch1IsAccel = false;
		private bool ch2IsAccel = false;

		private float ch1RawValue = 0.5f;
		private float ch2RawValue = 0.5f;

		private bool isConnected = false;

		private IBleBridge bleBridge;

		private void updateLog(string newline)
		{
			if(logText != null)
			{
				logText.text += newline + "\n\n";
				if(scrollRect != null)
				{
					if(logText.preferredHeight > scrollRect.gameObject.GetComponent<RectTransform>().rect.height)
					{
						logText.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
						scrollRect.verticalNormalizedPosition = 0;
					}

				}
			}
			
		}

		/**
		 * Called when the Bluetooth adapter changes states, such as enabled by the user after the app has started.
		 */
		private void StateUpdateAction(string state)
		{
			updateLog("Adapter: State Update = " + state);
		}

		/**
		 * Called when the IBleBridge.Startup() function has finished creating all the native resources and is ready to start connecting to BLE devices.
		 */
		private void StartupAction()
		{
			updateLog("Bridge: Startup");
		}

		/**
		 * Called when the IBleBridge.Shutdown() function has finished and the native resources are ready to be released. 
		 */
		private void ShutdownAction()
		{
			updateLog("Bridge: Shutdown");
		}

		/**
		 * Called when there is an error on the native side of the code.
		 */
		private void ErrorAction(string error)
		{
			updateLog("Error: " + error);
		}

		/**
		 * Called when a Bluetooth device / peripheral is found via the IBleBridge.ScanForPeripheralsWithServiceUUIDs().
		 */
		private void DiscoveredPeripheralAction(string peripheralId, string name)
		{
			updateLog("Bridge: Discovered Device = " + name + ", " + peripheralId);

			if(name == "Star Technologies")
				this.deviceId = peripheralId;
		}

		/**
		 * Called when a Bluetooth device / peripheral is found via the IBleBridge.RetrieveListOfPeripheralsWithServiceUUIDs().  
		 * On iOS this is faster then scanning. However,there is no diffrence between RetrieveListOfPeripheralsWithServiceUUIDs() and ScanForPeripheralsWithServiceUUIDs() on Android.
		 */
		private void RetrievedConnectedPeripheralAction(string peripheralId, string name)
		{
			updateLog("Bridge: Retrieved Device = " + name + ", " + peripheralId);
		}

		/**
		 * Called when a successful connection has been established with a Bluetooth device.  This is usually do to a call to IBleBridge.ConnectToPeripheralWithIdentifier()
		 */
		private void ConnectedPeripheralAction(string peripheralId, string name)
		{
			updateLog("Bridge: Connected to Device = " + name + ", " + peripheralId);
		}

		/**
		 * Called when a Bluetooth device has been disconnected, either from a call to IBleBridge.DisconnectFromPeripheralWithIdentifier() or the device has been shut off or gone out of range.
		 */
		private void DisconnectedPeripheralAction(string peripheralId, string name)
		{
			updateLog("Bridge: Disconnected from device = " + name + ", " + peripheralId);
			this.deviceId = null;
			this.isConnected = false;
			updateLog("Application: Blue-Rx is disconnected...");
		}

		/**
		 * Called when a Service has been discovered.  Services are automatically scanned for with a call to IBleBridge.ScanForPeripheralsWithServiceUUIDs() or IBleBridge.RetrieveListOfPeripheralsWithServiceUUIDs().
		 */
		private void DiscoveredServiceAction(string peripheralId, string service)
		{
			updateLog("Bridge: Discovered Service = " + service + ", " + peripheralId);
		}

		/**
		 * Called when a Characteristic has been discovered.  Characteristic are automatically scanned for with a call to IBleBridge.ScanForPeripheralsWithServiceUUIDs() or IBleBridge.RetrieveListOfPeripheralsWithServiceUUIDs().
		 */
		private void DiscoveredCharacteristicAction(string peripheralId, string service, string characteristic)
		{
			updateLog("Bridge: Discovered Characteristic = " + characteristic + ", " + service + ", " + peripheralId);

			if(peripheralId == deviceId && service == blueRxServiceUuid && characteristic == fastServoControlUuid)
			{
				isConnected = true;

				updateLog("Application: Blue-Rx is connected...");
			}

		}

		/**
		 * Called when a Characterisic has been successully written to, after a call to IBleBridge.WriteCharacteristicWithIdentifiers() and withResponse it true.
		 */
		private void DidWriteCharacteristicAction(string peripheralId, string service, string characteristic)
		{
			updateLog("Bridge: Did Write Characteristic = " + characteristic + ", " + service + ", " + peripheralId);
		}

		/**
		 * Called when the notification state has changed on a Characteristic, after a call to IBleBridge.SubscribeToCharacteristicWithIdentifiers() or IBleBridge.UnSubscribeFromCharacteristicWithIdentifiers()
		 */
		private void DidUpdateNotifiationStateForCharacteristicAction(string peripheralId, string uuid)
		{
			updateLog("Bridge: Did Update Notification State = " + uuid + ", " + peripheralId);
		}

		/**
		 * Called when a Characteristic value has been updated, either in reponse to a Notification / Indication or a call to IBleBridge.ReadCharacteristicWithIdentifiers()
		 */ 
		private void DidUpdateCharacteristicValueAction(string peripheralId, string characteristic, byte[] data)
		{

		}

		/**
		 * Called when a Descriptor has been writen to either from a call to IBleBridge.SubscribeToCharacteristicWithIdentifiers() or IBleBridge.UnSubscribeFromCharacteristicWithIdentifiers() or IBleBridge.WriteDescriptorWithIdentifiers()
		 */
		private void DidWriteDescriptorAction(string peripheralId, string characteristic, string descriptor)
		{
			updateLog("Bridge: Did Write Descriptor = " + descriptor + ", " + characteristic + ", " + peripheralId);
		}

		/**
		 * Called when a Descriptor has been read after a call to IBleBridge.ReadDescriptorWithIdentifiers()
		 */ 
		private void DidReadDescriptorAction(string peripheralId, string characteristic, string descriptor,  byte[] data)
		{
			updateLog("Bridge: Did Read Descriptor = " + descriptor + ", " + characteristic + ", " + peripheralId);
		}

		/**
		 * Called when a Descriptor has been discovered.  Descriptor are automatically scanned for with a call to IBleBridge.ScanForPeripheralsWithServiceUUIDs() or IBleBridge.RetrieveListOfPeripheralsWithServiceUUIDs().
		 */
		private void DiscoveredDescriptorAction(string peripheralId, string service, string characteristic, string descriptor)
		{
			updateLog("Bridge: Discovered Descriptor = " + peripheralId + ", " + characteristic + ", " + peripheralId);
		}

		/**
		 * Called when the RSSI or Received Signal Strength Indicator and changed, either during a scan or after a call to IBleBridge.ReadRssiWithIdentifier()
		 */ 
		private void DidUpdateRssiAction(string peripheralId, string rssi)
		{
			updateLog("Bridge: RSSI Update = " + rssi + ", " + peripheralId);
		}

		//on iOS the ble stack is limited to updates every 1/50th of a second, so we use a coroutine to govern the servo updates...
		IEnumerator UpdateServoValue()
		{

			while(true)
			{
				//send a packet every 1/25th of a second 
				yield return new WaitForSeconds(0.04f);

				if(isConnected)
				{

					if(debugText != null)
						debugText.text = "" + Input.acceleration;

					byte[] fastServoPacket = new byte[9];
					fastServoPacket[0] = 0x06; //channels 1 - 8

					if(ch1IsAccel)
						ch1RawValue = (Input.acceleration.y + 1.0f) / 2.0f;

					if(ch2IsAccel)
						ch2RawValue = (Input.acceleration.x + 1.0f) / 2.0f;


					if(ch1IsInverted)
						fastServoPacket[1] = (byte)((ch1RawValue * rangeCh1) + centerOffsetCh1); //channel 1
					else
						fastServoPacket[1] = (byte)(((1.0f - ch1RawValue) * rangeCh1) + centerOffsetCh1); //channel 1



					if(ch2IsInverted)
						fastServoPacket[2] = (byte)((ch2RawValue * rangeCh2) + centerOffsetCh2); //channel 2
					else
						fastServoPacket[2] = (byte)(((1.0f - ch2RawValue) * rangeCh2) + centerOffsetCh2); //channel 
					

					fastServoPacket[3] = 0x80; //channel 3
					fastServoPacket[4] = 0x80; //channel 4
					fastServoPacket[5] = 0x80; //channel 5
					fastServoPacket[6] = 0x80; //channel 6
					fastServoPacket[7] = 0x80; //channel 7
					fastServoPacket[8] = 0x80; //channel 8

					bleBridge.WriteCharacteristicWithIdentifiers(deviceId, blueRxServiceUuid, fastServoControlUuid, fastServoPacket, 9, 
					                                             false, this.DidWriteCharacteristicAction);

				}
			}

		}


		// Use this for initialization
		void Start () {

			if(logText != null)
				logText.text = "";

			//Determine which native IBleBridge to use based on the runtime platform; Android or iOS
			switch(Application.platform)
			{
			case RuntimePlatform.Android:
				bleBridge = new AndroidBleBridge();
				break;
			case RuntimePlatform.IPhonePlayer:
				bleBridge = new iOSBleBridge();
				break;
			default:
				bleBridge = new DummyBleBridge(); //modify this class if you want to emulate ble interaction in the editor...
				break;
			}

			//Startup the native side of the code and include our callbacks...
			bleBridge.Startup(true, this.StartupAction, this.ErrorAction, this.StateUpdateAction, this.DidUpdateRssiAction);

			//Start the packet updates...
			StartCoroutine("UpdateServoValue");
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
