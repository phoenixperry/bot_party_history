using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace startechplus.ble
{
	
	public class OsxBleBridge : IBleBridge
	{

		public delegate void UnitySendMessageCallbackDelegate(IntPtr objectName, IntPtr commandName, IntPtr commandData);

		[DllImport ("uBluetoothLeOsx")]
		private static extern void ConnectUnitySendMessageCallback ([MarshalAs(UnmanagedType.FunctionPtr)]UnitySendMessageCallbackDelegate callbackMethod);

		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeStartup (string gameObjName, bool isCentral);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeShutdown ();
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgePauseWithState (bool isPaused);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeConnectToPeripheralWithIdentifier (string peripheralId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeDisconnectPeripheralWithIdentifier (string peripheralId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeScanForPeripheralsWithServiceUUIDs (string uuids);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeRetrieveListOfPeripheralsWithServiceUUIDs (string uuids);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeRetrieveListOfPeripheralsWithUUIDs (string uuids);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeStopScanning ();
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeReadCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeWriteCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId, byte[] data, int length, bool withResponse);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeSubscribeToCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeUnSubscribeFromCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeReadDescriptorWithIdentifiers (string peripheralId, string serviceId, string characteristicId, string descriptorId);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeWriteDescriptorWithIdentifiers (string peripheralId, string serviceId, string characteristicId, string descriptorId, byte[] data, int length);
		
		[DllImport ("uBluetoothLeOsx")]
		private static extern void iOSBleBridgeReadRssiWithIdentifier(string peripheralId);
		
		private static BluetoothLeDevice bluetoothDevice;
		
		public BluetoothLeDevice Startup (bool asCentral, Action action, Action<string> errorAction, Action<string> stateUpdateAction, Action<string, string> rssiUpdateAction)
		{
			bluetoothDevice = null;
			
			
			if (GameObject.Find ("BleBridge") == null)
			{
				
				GameObject bleBridgeObj = new GameObject ("BleBridge");
				bluetoothDevice = bleBridgeObj.AddComponent<BluetoothLeDevice> ();
				
				if (bluetoothDevice != null)
				{
					bluetoothDevice.StartupAction = action;
					bluetoothDevice.ErrorAction = errorAction;
					bluetoothDevice.StateUpdateAction = stateUpdateAction;
					bluetoothDevice.DidUpdateRssiAction = rssiUpdateAction;
				}

				ConnectUnitySendMessageCallback((_objectName, _commandName, _commandData) => {
					string objectName = Marshal.PtrToStringAuto(_objectName);
					string commandName = Marshal.PtrToStringAuto(_commandName);
					string commandData = Marshal.PtrToStringAuto(_commandData);

					GameObject foundObject = GameObject.Find(objectName);
					if(foundObject != null)
					{
						foundObject.SendMessage(commandName, commandData);
					}
				});
			}
			
			iOSBleBridgeStartup ("BleBridge", asCentral);
			
			return bluetoothDevice;
		}
		
		public void Shutdown (Action action)
		{
			
			if (bluetoothDevice != null)
				bluetoothDevice.ShutdownAction = action;
			
			iOSBleBridgeShutdown ();
		}
		
		public void Cleanup ()
		{
			GameObject bleBridgeObj = GameObject.Find ("BleBridge");
			
			if (bleBridgeObj != null)
				GameObject.Destroy (bleBridgeObj);
		}
		
		public void PauseWithState (bool isPaused)
		{
			iOSBleBridgePauseWithState (isPaused);	
		}
		
		public void ScanForPeripheralsWithServiceUUIDs (string[] serviceUUIDs, Action<string, string> action)
		{
			if (bluetoothDevice != null) 
			{
				bluetoothDevice.DiscoveredPeripheralAction = action;
			}
			
			string serviceUUIDsString = null;
			
			if (serviceUUIDs != null) 
			{
				serviceUUIDsString = "";
				
				foreach (string serviceUUID in serviceUUIDs)
					serviceUUIDsString += serviceUUID + "|";
				
				serviceUUIDsString = serviceUUIDsString.Substring (0, serviceUUIDsString.Length - 1);
			}
			
			iOSBleBridgeScanForPeripheralsWithServiceUUIDs (serviceUUIDsString);
		}
		
		public void ConnectToPeripheralWithIdentifier (string peripheralId, Action<string, string> connectAction, Action<string, string> serviceAction, Action<string, string, string> characteristicAction, Action<string, string, string, string> descriptorAction, Action<string, string>disconnectAction)
		{
			
			if (bluetoothDevice != null)
			{
				bluetoothDevice.ConnectedPeripheralAction = connectAction;
				bluetoothDevice.DiscoveredServiceAction = serviceAction;
				bluetoothDevice.DiscoveredCharacteristicAction = characteristicAction;
				bluetoothDevice.DiscoveredDescriptorAction = descriptorAction;
				bluetoothDevice.DisconnectedPeripheralAction = disconnectAction;
			}
			
			iOSBleBridgeConnectToPeripheralWithIdentifier(peripheralId);
		}
		
		public void DisconnectFromPeripheralWithIdentifier (string peripheralId, Action<string, string> action)
		{
			
			if (bluetoothDevice != null)
				bluetoothDevice.DisconnectedPeripheralAction = action;
			
			iOSBleBridgeDisconnectPeripheralWithIdentifier (peripheralId);
		}
		
		
		public void RetrieveListOfPeripheralsWithServiceUUIDs (string[] serviceUUIDs, Action<string, string> action)
		{
			if (bluetoothDevice != null)
			{
				bluetoothDevice.RetrievedPeripheralWithServiceAction = action;
			}
			
			string serviceUUIDsString = null;
			
			if(serviceUUIDs != null)
			{
				serviceUUIDsString = serviceUUIDs.Length > 0 ? "" : null;
				
				foreach (string serviceUUID in serviceUUIDs)
					serviceUUIDsString += serviceUUID + "|";
				
				// strip the last delimeter
				serviceUUIDsString = serviceUUIDsString.Substring (0, serviceUUIDsString.Length - 1);
			}
			
			iOSBleBridgeRetrieveListOfPeripheralsWithServiceUUIDs (serviceUUIDsString);
		}
		
		public void RetrieveListOfPeripheralsWithUUIDs (string[] uuids, Action<string, string> action)
		{
			if (bluetoothDevice != null)
			{
				bluetoothDevice.RetrievedPeripheralWithUUIDAction = action;
				
			}
			
			string uuidsString = null;
			
			if(uuids != null)
			{
				uuidsString = uuids.Length > 0 ? "" : null;
				foreach (string uuidString in uuids)
					uuidsString += uuidString + "|";
				
				// strip the last delimeter
				uuidsString = uuidsString.Substring (0, uuidsString.Length - 1);
			}
			
			iOSBleBridgeRetrieveListOfPeripheralsWithUUIDs (uuidsString);
		}
		
		public void StopScanning ()
		{
			iOSBleBridgeStopScanning();
		}
		
		public void SubscribeToCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId, Action<string, string, string> notificationAction, Action<string, string, string, byte[]> action, bool isIndication)
		{
			
			if (bluetoothDevice != null)
			{
				bluetoothDevice.DidUpdateNotificationStateForCharacteristicAction = notificationAction;
				bluetoothDevice.DidUpdateCharacteristicValueAction = action;
			}
			
			iOSBleBridgeSubscribeToCharacteristicWithIdentifiers (peripheralId, serviceId, characteristicId);
		}
		
		public void UnSubscribeFromCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId, Action<string, string, string> action)
		{
			
			iOSBleBridgeUnSubscribeFromCharacteristicWithIdentifiers (peripheralId, serviceId, characteristicId);
			
		}
		
		public void ReadCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId, Action<string, string, string, byte[]> action)
		{
			
			if (bluetoothDevice != null)
				bluetoothDevice.DidUpdateCharacteristicValueAction = action;
			
			iOSBleBridgeReadCharacteristicWithIdentifiers (peripheralId, serviceId, characteristicId);
			
		}
		
		public void WriteCharacteristicWithIdentifiers (string peripheralId, string serviceId, string characteristicId, byte[] data, int length, bool withResponse, Action<string, string, string> action)
		{
			
			if (bluetoothDevice != null)
				bluetoothDevice.DidWriteCharacteristicAction = action;
			
			iOSBleBridgeWriteCharacteristicWithIdentifiers(peripheralId, serviceId, characteristicId, data, length, withResponse);
			
		}
		
		
		public void ReadDescriptorWithIdentifiers(string peripheralId, string serviceId, string characteristicId, string descriptorId, Action<string, string, string, string, byte[]> action)
		{
			if(bluetoothDevice != null)
				bluetoothDevice.DidReadDescriptorValueAction = action;
			
			iOSBleBridgeReadDescriptorWithIdentifiers(peripheralId, serviceId, characteristicId, descriptorId);
		}
		
		public void WriteDescriptorWithIdentifiers(string peripheralId, string serviceId, string characteristicId, string descriptorId, byte[] data, int length, Action<string, string, string, string> action)
		{
			if(bluetoothDevice != null)
				bluetoothDevice.DidWriteDescriptorAction = action;
			
			iOSBleBridgeWriteDescriptorWithIdentifiers(peripheralId, serviceId, characteristicId, descriptorId, data, length);
		}
		
		public void ReadRssiWithIdentifier(string peripheralId)
		{
			iOSBleBridgeReadRssiWithIdentifier(peripheralId);
		}
		
		public void AddAdvertisementDataListeners(Action<string, string> localNameAction, 
		                                          Action<string, byte[]> manufactureDataAction,
		                                          Action<string, string, byte[]> serviceDataAction,
		                                          Action<string, string> serviceAction,
		                                          Action<string, string> overflowServiceAction,
		                                          Action<string, string> txPowerLevelAction,
		                                          Action<string, string> isConnectable,
		                                          Action<string, string> solicitedServiceAction)
		{
			if (bluetoothDevice != null)
			{
				bluetoothDevice.DidAdvertiseLocalNameAction = localNameAction;
				bluetoothDevice.DidAdvertiseManufactureDataAction = manufactureDataAction;
				bluetoothDevice.DidAdvertiseServiceDataAction = serviceDataAction;
				bluetoothDevice.DidAdvertiseServiceAction = serviceAction;
				bluetoothDevice.DidAdvertiseOverflowServiceAction = overflowServiceAction;
				bluetoothDevice.DidAdvertiseTxPowerLevelAction = txPowerLevelAction;
				bluetoothDevice.DidAdvertiseIsConnectable = isConnectable;
				bluetoothDevice.DidAdvertiseSolicitedServiceAction = solicitedServiceAction;
				
			}
		}
	}
}
