using UnityEngine;
using System.Collections;
using startechplus.ble;
using UnityEngine.UI;
using System;
using AHRS;

namespace startechplus.ble.examples
{
	/**
	 * An example uBluetoothLe pluging application. In the Start() function the IBleBridge object is instantiated based on whether the Unity Player is running on Android or iOS.  
	 * Once started all interaction with the Bluetooth Device via the IBleBridge is handled via Action() callbacks.  These callbacks are set when the varioius IBleBridge functions are called.
	 * These callbacks are created by you, see the examples in this file.  For details about the Action() callbakcs see IBleBridge.cs.
	 * @see Start()
	 */ 
	public class SensorTag2650 : MonoBehaviour {

		public Text logText;
		public ScrollRect scrollRect;
		public string baseUuid = "F0000000-0451-4000-B000-000000000000";
		public GameObject virtualTag;
		public Text accelerationText;
		public string deviceScanTag = "CC2650 SensorTag";

		//private Vector3 gravity = new Vector3(0,0,-1);
		private MadgwickAHRS ahrs = new MadgwickAHRS(0.1f, 0.25f); 

		private float deg2rad(float degrees)
		{
			return (float)(Math.PI / 180) * degrees;
		}

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

		private SensorTag2650Sensor movement;

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
			if(name.StartsWith(deviceScanTag))
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
			if(characteristic == movement.DataUuid)
			{
				updateLog("Application: Subscribing to movement notifications...");
				bleBridge.SubscribeToCharacteristicWithIdentifiers(peripheralId, service, characteristic, 
				                                                   this.DidUpdateNotifiationStateForCharacteristicAction, 
				                                                   this.DidUpdateCharacteristicValueAction, false);
			}

			//enable the sensor
			if(characteristic == movement.ConfigurationUuid)
			{
				updateLog("Application: Enabling movement...");
				byte[] enableSensor = new byte[2];
				enableSensor[0] = 0x7F; //enable all sensors and WOM
				enableSensor[1] = 0x7F; //enable 16G accel range

				bleBridge.WriteCharacteristicWithIdentifiers(peripheralId, service, characteristic,
				                                             enableSensor, 2, true, this.DidWriteCharacteristicAction);

			}

			//update the period
			if(characteristic == movement.PeriodUuid)
			{
				updateLog("Application: Updating movement notification period...");
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
			if(peripheralId == deviceId && characteristic == movement.DataUuid)
			{

				float gDivisor = 65526.0f / 500.0f; // deg / second
				float aDivisor = 32768.0f / 8.0f; // 8G range
				//float mDivisor = 1.0f;

				float gX = BitConverter.ToInt16(data, 0) / gDivisor;
				float gY = BitConverter.ToInt16(data, 2) / gDivisor;
				float gZ = BitConverter.ToInt16(data, 4) / gDivisor;
				
				float aX = BitConverter.ToInt16(data, 6) / aDivisor;
				float aY = BitConverter.ToInt16(data, 8) / aDivisor;
				float aZ = BitConverter.ToInt16(data, 10) / aDivisor;

				//float mX = BitConverter.ToInt16(data, 12) / mDivisor;
				//float mY = BitConverter.ToInt16(data, 14) / mDivisor;
				//float mZ = BitConverter.ToInt16(data, 16) / mDivisor;

				ahrs.Update(deg2rad(gX), deg2rad(gY), deg2rad(gZ), aX, aY, aZ);

				Quaternion originalRotation = new Quaternion(ahrs.Quaternion[1], ahrs.Quaternion[2], ahrs.Quaternion[3], ahrs.Quaternion[0]);
				Vector3 originalEuler = originalRotation.eulerAngles;
				Vector3 newEuler = new Vector3(-originalEuler.x, -originalEuler.y, originalEuler.z);

				virtualTag.transform.rotation = Quaternion.Euler(newEuler);

				if(accelerationText != null)
				{
					accelerationText.text = "X:" + aX.ToString("F3") + " Y:" + aY.ToString("F3") + " Z:" + aZ.ToString("F3");
					//accelerationText.text = "X:" + ahrs.Quaternion[0].ToString("F3") + " Y:" + ahrs.Quaternion[1].ToString("F3") + " Z:" + ahrs.Quaternion[2].ToString("F3") + " W:" + ahrs.Quaternion[3].ToString("F3");
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

			//setup the movement UUIDs
			movement = new SensorTag2650Sensor(baseUuid, "AA81", "AA82", "AA83");

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
