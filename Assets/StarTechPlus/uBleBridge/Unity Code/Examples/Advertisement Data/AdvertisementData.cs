using UnityEngine;
using System.Collections;
using startechplus.ble;
using UnityEngine.UI;
using System;

namespace startechplus.ble.examples
{
	/**
	 * An example uBluetoothLe pluging application. In the Start() function the IBleBridge object is instantiated based on whether the Unity Player is running on Android or iOS.  
	 * Once started all interaction with the Bluetooth Device via the IBleBridge is handled via Action() callbacks.  These callbacks are set when the varioius IBleBridge functions are called.
	 * These callbacks are created by you, see the examples in this file.  For details about the Action() callbakcs see IBleBridge.cs.
	 * @see Start()
	 */ 
	public class AdvertisementData : MonoBehaviour {

		public Text logText;
		public ScrollRect scrollRect;

		public Text debugText;

		/*
		NOTES
		Action<string, string> localNameAction, 
		Action<string, byte[]> manufactureDataAction,
		Action<string, string, byte[]> serviceDataAction,
		Action<string, string> serviceAction,
		Action<string, string> overflowServiceAction,
		Action<string, string> txPowerLevelAction,
		Action<string, string> isConnectable,
		Action<string, string> solicitedServiceAction);
		*/
		 
		public bool showScanData = true;
		public bool showLocalName = true;
		public bool showManufactureData = true;
		public bool showServiceData = true;
		public bool showServices = true;
		public bool showOverflowService = true;
		public bool showTxPowerLevel = true;
		public bool showIsConnectable = true;
		public bool showSolicitedService = true;

		public void AdvertiseLocalNameAction(string peripherialID, string localName)
		{
			if(showLocalName)
				updateLog("Bridge: AdvertiseLocalNameAction,  " + peripherialID + ", " + localName);
		}

		public void AdvertiseManufactureDataAction(string peripherialID, byte[]data)
		{
			if(showManufactureData)
				updateLog("Bridge: AdvertiseManufactureDataAction, " + peripherialID + ", " + BitConverter.ToString(data));
		}

		public void AdvertiseServiceDataAction(string peripherialID, string serviceID, byte[] data)
		{
			if(showServiceData)
				updateLog("Bridge: AdvertiseServiceDataAction, " + peripherialID + ", " + serviceID + ", " + BitConverter.ToString(data));
		}

		public void AdvertiseServiceAction(string peripherialID, string serviceID)
		{
			if(showServices)
				updateLog("Bridge: AdvertiseServiceAction, " + peripherialID + ", " + serviceID);
		}

		public void AdvertiseOverflowServiceAction(string peripherialID, string serviceID)
		{
			if(showOverflowService)
				updateLog("Bridge: AdvertiseOverflowServiceAction, " + peripherialID + ", " + serviceID);
		}

		public void AdvertiseTxPowerLevelActionAction(string peripherialID, string txPowerLevel)
		{
			if(showTxPowerLevel)
				updateLog("Bridge: AdvertiseTxPowerLevelActionAction, " + peripherialID + ", " + txPowerLevel);
		}

		public void AdvertiseIsConnectableAction(string peripherialID, string isConnectable)
		{
			if(showIsConnectable)
				updateLog("Bridge: AdvertiseIsConnectableAction, " + peripherialID + ", " + isConnectable);
		}

		public void AdvertiseSolicitedServiceAction(string peripherialID, string solicitedServiceID)
		{
			if(showSolicitedService)
				updateLog("Bridge: AdvertiseSolicitedServiceAction, " + peripherialID + ", " + solicitedServiceID);
		}


		/**
		 * Connected to the Scan button in the Unity Editor.
		 */
		public void Scan()
		{

			bleBridge.StopScanning ();

			updateLog("Applicaton: Scanning for ble devices...");

			deviceId = null;

			bleBridge.AddAdvertisementDataListeners(this.AdvertiseLocalNameAction,
			                                        this.AdvertiseManufactureDataAction,
			                                        this.AdvertiseServiceDataAction,
			                                        this.AdvertiseServiceAction,
			                                        this.AdvertiseOverflowServiceAction,
			                                        this.AdvertiseTxPowerLevelActionAction,
			                                        this.AdvertiseIsConnectableAction,
			                                        this.AdvertiseSolicitedServiceAction);

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

			}

		}

		/**
		 * Connected to the Disconnect button in the Unity Editor.
		 */
		public void Disconnect()
		{

			if(deviceId != null)
				bleBridge.DisconnectFromPeripheralWithIdentifier(deviceId, this.DisconnectedPeripheralAction);

		}

		/**
		 * Connected to the various GUI elements in the Unity Editor
		 */ 
		public void onValueChanged()
		{

		}

		private string deviceId;

		private bool isConnected = false;

		private IBleBridge bleBridge;

		private string logBuffer = "";

		private void updateLog(string newline)
		{
			if(logText != null)
			{

				logBuffer += newline + "\n\n";

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
			if(showScanData)
				updateLog("Bridge: Discovered Device = " + name + ", " + peripheralId);

			if(name == "Flex")
				this.deviceId = peripheralId;
		}

		/**
		 * Called when a Bluetooth device / peripheral is found via the IBleBridge.RetrieveListOfPeripheralsWithServiceUUIDs().  
		 * On iOS this is faster then scanning. However,there is no diffrence between RetrieveListOfPeripheralsWithServiceUUIDs() and ScanForPeripheralsWithServiceUUIDs() on Android.
		 */
		private void RetrievedConnectedPeripheralAction(string peripheralId, string name)
		{
			if(showScanData)
				updateLog("Bridge: Retrieved Device = " + name + ", " + peripheralId);
		}

		/**
		 * Called when a successful connection has been established with a Bluetooth device.  This is usually do to a call to IBleBridge.ConnectToPeripheralWithIdentifier()
		 */
		private void ConnectedPeripheralAction(string peripheralId, string name)
		{
			updateLog("Bridge: Connected to Device = " + name + ", " + peripheralId);
			Debug.Log("*** " + this.deviceId);
			bleBridge.ReadRssiWithIdentifier(this.deviceId);

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
			if(showScanData)
				updateLog("Bridge: RSSI Update = " + rssi + ", " + peripheralId);
		}

		//on iOS the ble stack is limited to updates every 1/50th of a second, so we use a coroutine to govern sending updates...
		IEnumerator UpdateValue()
		{

			while(true)
			{
				//send a packet every 1/25th of a second 
				yield return new WaitForSeconds(0.04f);

				if(isConnected)
				{


				}
			}

		}

		IEnumerator UpdateLogView()
		{
			int lastLogLength = 0;

			while (true) {
				
				yield return new WaitForSeconds (0.5f);

				if (logText != null) {

					if (logBuffer.Length != lastLogLength) {

						lastLogLength = logBuffer.Length;

						logText.text = logBuffer;

						if (scrollRect != null) {
							if (logText.preferredHeight > scrollRect.gameObject.GetComponent<RectTransform> ().rect.height) {
								logText.gameObject.GetComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
								scrollRect.verticalNormalizedPosition = 0.0f;
							}

						}
					}
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
			StartCoroutine("UpdateValue");
			StartCoroutine("UpdateLogView");
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
