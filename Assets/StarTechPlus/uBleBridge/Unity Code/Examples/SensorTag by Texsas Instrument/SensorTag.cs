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
	public class SensorTag : MonoBehaviour {

		public Text logText;
		public ScrollRect scrollRect;
		public string baseUuid = "F0000000-0451-4000-B000-000000000000";
		public GameObject virtualTag;
		public Text accelerationText;

		Vector3 gravity = new Vector3(0,0,-1);

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
				updateLog("Applicaton: SensorTag not found...");
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
				updateLog("Applicaton: SensorTag not found...");
		}

		private SensorTagSensor accelerometer;

		private string deviceId;

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

			//if we found the SensorTag, save its peripheralId / MAC Address
			if(name == "SensorTag")
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
		}

		/**
		 * Called when a Service has been discovered.  Services are automatically scanned for with a call to IBleBridge.ConnectToPeripheralWithIdentifiers().
		 */
		private void DiscoveredServiceAction(string peripheralId, string service)
		{
			updateLog("Bridge: Discovered Service = " + service + ", " + peripheralId);
		}

		/**
		 * Called when a Characteristic has been discovered.  Characteristic are automatically scanned for with a call to IBleBridge.ScaConnectToPeripheralWithIdentifier().
		 */
		private void DiscoveredCharacteristicAction(string peripheralId, string service, string characteristic)
		{
			updateLog("Bridge: Discovered Characteristic = " + characteristic + ", " + peripheralId);

			//subscribe to notifications
			if(characteristic == accelerometer.DataUuid)
			{
				updateLog("Application: Subscribing to accelerometer notifications...");
				bleBridge.SubscribeToCharacteristicWithIdentifiers(peripheralId, service, characteristic, 
				                                                   this.DidUpdateNotifiationStateForCharacteristicAction, 
				                                                   this.DidUpdateCharacteristicValueAction, false);
			}

			//enable the sensor
			if(characteristic == accelerometer.ConfigurationUuid)
			{
				updateLog("Application: Enabling accelerometer...");
				byte[] enableSensor = new byte[1];
				enableSensor[0] = 0x01;

				bleBridge.WriteCharacteristicWithIdentifiers(peripheralId, service, characteristic,
				                                             enableSensor, 1, true, this.DidWriteCharacteristicAction);

			}

			//update the period
			if(characteristic == accelerometer.PeriodUuid)
			{
				updateLog("Application: Updating accelerometer notification period...");
				byte[] period = new byte[1];
				period[0] = 10; //every 100 ms
				
				bleBridge.WriteCharacteristicWithIdentifiers(peripheralId, service, characteristic,
				                                             period, 1, true, this.DidWriteCharacteristicAction);
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
		private void DidUpdateNotifiationStateForCharacteristicAction(string peripheralId, string service, string characteristic)
		{
			updateLog("Bridge: Did Update Notification State = " + characteristic + ", " + service + ", " + peripheralId);
		}

		/**
		 * Called when a Characteristic value has been updated, either in reponse to a Notification / Indication or a call to IBleBridge.ReadCharacteristicWithIdentifiers()
		 */ 
		private void DidUpdateCharacteristicValueAction(string peripheralId, string service, string characteristic, byte[] data)
		{
			//updateLog("Bridge: Did Update Characteristic = " + characteristic + ", " + peripheralId);

			//if this is the characterisic we want orient the GameObject accordingly...
			if(peripheralId == deviceId && characteristic == accelerometer.DataUuid)
			{

				float aX = (sbyte)data[0] / 64.0f;
				float aY = (sbyte)data[1] / 64.0f;
				float aZ = (sbyte)data[2] / 64.0f;

				gravity = Vector3.Lerp(gravity, new Vector3(-aX, aZ, aY), 0.25f);

				virtualTag.transform.up = -gravity;

				if(accelerationText != null)
				{
					accelerationText.text = "X:" + aX.ToString("F3") + " Y:" + aY.ToString("F3") + " Z:" + aZ.ToString("F3");
				}
			}
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
		 * Called when a Descriptor has been discovered.  Descriptors are automatically scanned for with a call to IBleBridge.ConnectToPeripheralWithIdentifier().
		 */
		private void DiscoveredDescriptorAction(string peripheralId, string service, string characteristic, string descriptor)
		{
			updateLog("Bridge: Discovered Descriptor = " + descriptor + ", " + characteristic + ", " + service + ", " + peripheralId);
		}

		/**
		 * Called when the RSSI or Received Signal Strength Indicator and changed, either durring a scan or after a call to IBleBridge.ReadRssiWithIdentifier()
		 */ 
		private void DidUpdateRssiAction(string peripheralId, string rssi)
		{
			updateLog("Bridge: RSSI Update = " + rssi + ", " + peripheralId);
		}


		// Use this for initialization
		void Start () {

			//setup the accelerometer UUIDs
			accelerometer = new SensorTagSensor(baseUuid, "AA11", "AA12", "AA13");

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

		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
