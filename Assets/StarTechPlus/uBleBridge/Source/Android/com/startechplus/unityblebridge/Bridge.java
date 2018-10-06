package com.startechplus.unityblebridge;

import com.unity3d.player.UnityPlayer;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothProfile;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.os.Handler;
import android.os.Looper;
import android.os.ParcelUuid;
import android.util.Base64;
import android.util.Log;
import android.util.SparseArray;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map.Entry;
import java.util.Queue;
import java.util.UUID;
import java.util.HashMap;

public class Bridge {

	private HashMap<String, BluetoothPeripheral> peripherals = new HashMap<String, BluetoothPeripheral>();
	

	Context context;
	private static final String TAG = "UnityBleBridge";
	
	private static final UUID CLIENT_CHARACTERISTIC_CONFIGURATION = UUID.fromString("00002902-0000-1000-8000-00805f9b34fb");

	public static String gameObjectName = "BleBridge";
	private static final long SCAN_PERIOD = 60000;
	private BluetoothAdapter mBluetoothAdapter;
	private boolean mScanning = false;
	private Handler mHandler;
	private PeripheralFilter peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);

	private Queue<GattComm> gattQueue = new LinkedList<GattComm>();
	private boolean gattQueueIsPending = false;
	
	private void sendGattComm(GattComm gattComm)
	{
		switch(gattComm.type)
		{
		case GattComm.READ_CHARACTERISTIC:
			gattComm.gatt.readCharacteristic((BluetoothGattCharacteristic)gattComm.actor);
			break;
		case GattComm.READ_DESCRIPTOR:
			gattComm.gatt.readDescriptor((BluetoothGattDescriptor)gattComm.actor);		
			break;
		case GattComm.READ_RSSI:
			gattComm.gatt.readRemoteRssi();
			break;
		case GattComm.WRITE_CHARACTERISTIC:
			if(gattComm.data.length > 0)
			{
				((BluetoothGattCharacteristic)gattComm.actor).setValue(gattComm.data);
				((BluetoothGattCharacteristic)gattComm.actor).setWriteType(gattComm.writeType);
				gattComm.gatt.writeCharacteristic((BluetoothGattCharacteristic)gattComm.actor);
			}
			break;
		case GattComm.WRITE_DESCRIPTOR:
			if(gattComm.data.length > 0)
			{
				((BluetoothGattDescriptor)gattComm.actor).setValue(gattComm.data);
				gattComm.gatt.writeDescriptor((BluetoothGattDescriptor)gattComm.actor);
			}
			break;	
			
		}
	}
	
	private void processGattComm(GattComm gattComm)
	{
		if(!gattQueueIsPending)
		{
			gattQueueIsPending = true;
			sendGattComm(gattComm);
		}
		else
		{
			gattQueue.add(gattComm);
		}
	}
	
	private void checkGattQueue()
	{
		if(gattQueue.isEmpty())
		{
			gattQueueIsPending = false;
		}
		else
		{
			sendGattComm(gattQueue.poll());
		}
	}
	
	static Bridge _instance = null;

	public Bridge() {
		_instance = this;
		mHandler = new Handler(Looper.getMainLooper());
	}

	public static Bridge instance() {
		if (_instance == null)
			_instance = new Bridge();

		return _instance;

	}

	public void setContext(Context ctx) {
		this.context = ctx;
		Log.i(TAG, "Application context set...");
	}

	private static final BroadcastReceiver mReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			final String action = intent.getAction();

			if (action.equals(BluetoothAdapter.ACTION_STATE_CHANGED)) {
				final int state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, BluetoothAdapter.ERROR);
				switch (state) {
				case BluetoothAdapter.STATE_OFF:
					UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Powered Off");
					break;
				case BluetoothAdapter.STATE_TURNING_OFF:
					UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Unknown");
					break;
				case BluetoothAdapter.STATE_ON:
					UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Powered On");
					break;
				case BluetoothAdapter.STATE_TURNING_ON:
					UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Unknown");
					break;
				}
			} else if (action.equals(BluetoothDevice.ACTION_FOUND)) {
				//BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

			} else if (action.equals(BluetoothDevice.ACTION_ACL_CONNECTED)) {
				//BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

			} else if (action.equals(BluetoothDevice.ACTION_ACL_DISCONNECTED)) {
				//BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

			}
		}
	};

	public static void RegisterBroadcastReciever(Activity activity) {
		// Register for broadcasts on BluetoothAdapter state change
		IntentFilter filter = new IntentFilter();
		filter.addAction(BluetoothAdapter.ACTION_STATE_CHANGED);
		filter.addAction(BluetoothDevice.ACTION_FOUND);
		filter.addAction(BluetoothDevice.ACTION_ACL_CONNECTED);
		filter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECTED);
		activity.registerReceiver(mReceiver, filter);
	}

	public static void DeregisterBroadcastReciever(Activity activity) {
		activity.unregisterReceiver(mReceiver);
	}

	/*
	 * public static boolean onActivityResult(int requestCode, int resultCode,
	 * Intent data){ boolean retVal = true;
	 * 
	 * switch (requestCode) { case ActivityResultCodes.REQUEST_ENABLE_BT:
	 * if(resultCode == Activity.RESULT_OK) {
	 * UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate",
	 * "Powered On"); } else { UnityPlayer.UnitySendMessage (gameObjectName,
	 * "OnBleStateUpdate", "Powered Off"); } break;
	 * 
	 * default: retVal = false; break; }
	 * 
	 * return retVal; }
	 */

	private String onUpdatePeripheral(BluetoothPeripheral peripheral, String event, String identifier) {
		String message;

		String ident = identifier == null ? "Unknown" : identifier;

		Iterator<Entry<java.lang.String, BluetoothPeripheral>> it = peripherals.entrySet().iterator();

		while (it.hasNext()) {
			Entry<java.lang.String, BluetoothPeripheral> kvp = it.next();

			BluetoothPeripheral listPeripheral = kvp.getValue();

			if (listPeripheral.equals(peripheral)) {

				message = String.format("%d:%s%d:%s", kvp.getKey().length(), kvp.getKey(), ident.length(), ident);

				UnityPlayer.UnitySendMessage(gameObjectName, event, message);

				return kvp.getKey();
			}
		}

		String newKey = UUID.randomUUID().toString();

		if (peripheral.device.getAddress() != null && peripheral.device.getAddress().length() > 0)
			newKey = peripheral.device.getAddress();

		Log.i(TAG, "onUpdatePeripheral() : adding peripheral : " + newKey );
		peripherals.put(newKey, peripheral);

		message = String.format("%d:%s%d:%s", newKey.length(), newKey, ident.length(), ident);

		UnityPlayer.UnitySendMessage(gameObjectName, event, message);
		
		return newKey;

	}
	
	private void unitySendUpdate(String peripherialID,  String key, String value)
	{
		//Log.i(TAG, "onLeScan() : unitySendUpdate " + peripherialID + ", " + key + ", "+ value);
		String message = String.format("%d:%s%d:%s", peripherialID.length(), peripherialID, value.length(), value);
		UnityPlayer.UnitySendMessage(gameObjectName, key, message);	
	}

	// Device scan callback.
	private BluetoothAdapter.LeScanCallback mLeScanCallback = new BluetoothAdapter.LeScanCallback() {
		@Override
		public void onLeScan(final BluetoothDevice device, int rssi, byte[] scanRecord) {
			

			BluetoothPeripheral peripheral = new BluetoothPeripheral(device, rssi, scanRecord);

			String deviceName = device.getName();
			
			if (deviceName == null) {
				deviceName = peripheral.advertisedData.getName();
			}
						
			if(peripheralFilter.filterWith(peripheral.device.getAddress(), PeripheralFilter.PERIPHERAL_UUID))
			{
				Log.i(TAG, "onLeScan() : " + device.getAddress().toString() + ", " + rssi + ", matched filter...");
				
				String peripherialID = onUpdatePeripheral(peripheral, "OnDiscoveredPeripheral", deviceName);
				
				if(peripheral.advertisedData.getName() != null)
				{
					unitySendUpdate(peripherialID, "OnAdvertisementDataLocalName", peripheral.advertisedData.getName());
				}
				
				//Log.i(TAG, "onLeScan() : D1 " + scanRecord.length);
				
				ScanRecord sRecord = ScanRecord.parseFromBytes(scanRecord);
				
				SparseArray<byte[]> manufactureData = sRecord.getManufacturerSpecificData();
				
				if(manufactureData != null)
				{
					for(int i = 0, nsize = manufactureData.size(); i < nsize; i++) {
					    byte[] data = manufactureData.valueAt(i);
					    unitySendUpdate(peripherialID, "OnAdvertisementDataManufactureData", Base64.encodeToString(data, Base64.DEFAULT));
					}
				}
				
				//Log.i(TAG, "onLeScan() : D2");
				List<ParcelUuid> sUUIDs = sRecord.getServiceUuids();
				//Log.i(TAG, "onLeScan() : D2a ");
				
				if(sUUIDs != null)
				{
					//Log.i(TAG, "onLeScan() : D2b");

					for(int i = 0; i < sUUIDs.size(); i++)
					{
						//Log.i(TAG, "onLeScan() : D2c");
						ParcelUuid sUUID = sUUIDs.get(i);
						if(sUUID != null)
						{
							unitySendUpdate(peripherialID, "OnAdvertisementDataServiceUUID", sUUID.toString());
							//Log.i(TAG, "onLeScan() : D2d");
							byte[] data = sRecord.getServiceData(sUUID);
							//Log.i(TAG, "onLeScan() : D2e");
							if(data != null)
							{

								/*
								NSString *message = [NSString stringWithFormat:@"%d:%@%d:%@%d:%@",
                                     (int)strlen([peripheralId UTF8String]), peripheralId,
                                     (int)strlen([serviceUUID UTF8String]), serviceUUID,
                                     (int)strlen([dataString  UTF8String]), dataString];

                                 unitySendUpdate(peripherialID, "OnAdvertisementDataServiceData", Base64.encodeToString(data, Base64.DEFAULT));

								 */

								String dataString = Base64.encodeToString(data, Base64.DEFAULT);

								//Log.i(TAG, "onLeScan() : D3, " + dataString);

								String message = String.format("%d:%s%d:%s%d:%s",
										peripherialID.length(), peripherialID,
										sUUID.toString().length(), sUUID.toString(),
										dataString.length(), dataString);

								UnityPlayer.UnitySendMessage(gameObjectName, "OnAdvertisementDataServiceData", message);

							}
						}
					}
				}
				
				//Log.i(TAG, "onLeScan() : D3");
				if(sRecord.getTxPowerLevel() != java.lang.Integer.MIN_VALUE)
				{
					unitySendUpdate(peripherialID, "OnAdvertisementDataTxPowerLevel", "" + sRecord.getTxPowerLevel());
				}
				
				//Log.i(TAG, "onLeScan() : D4");
				if(sRecord.getAdvertiseFlags() != -1)
				{
					unitySendUpdate(peripherialID, "OnAdvertisementDataIsConnectable", "" + sRecord.getAdvertiseFlags());
				}
				
				//Log.i(TAG, "onLeScan() : D5");
				unitySendUpdate(peripherialID, "OnRssiUpdate", "" + rssi);
				
				/*
				Iterator<Entry<java.lang.String, BluetoothPeripheral>> it = peripherals.entrySet().iterator();

				while (it.hasNext()) {
					Entry<java.lang.String, BluetoothPeripheral> kvp = it.next();

					BluetoothPeripheral listPeripheral = kvp.getValue();

					if (listPeripheral.equals(peripheral)) {

						String message = String.format("%d:%s%d:%s", kvp.getKey().length(), kvp.getKey(), (rssi+"").length(), rssi);

						UnityPlayer.UnitySendMessage(gameObjectName, "OnRssiUpdate", message);

						return;
					}
				}
				*/
				
			}
			else
			{
				Log.i(TAG, "onLeScan() : " + device.getAddress().toString() + ", " + rssi + ", mismatched filter...");
			}
		}
	};

	public void startup(String goName, boolean asCentral) {
		Log.i(TAG, "startup(" + goName + ", " + asCentral + ")");

		gameObjectName = goName;

		if (context == null) {
			Log.i(TAG, "startup() : " + "context == null");
			return;
		}

		UnityPlayer.UnitySendMessage(gameObjectName, "OnStartup", "Active");

		if (context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
			final BluetoothManager bluetoothManager = (BluetoothManager) context.getSystemService(Context.BLUETOOTH_SERVICE);
			mBluetoothAdapter = bluetoothManager.getAdapter();
			RegisterBroadcastReciever(UnityPlayer.currentActivity);

			if (mBluetoothAdapter == null || !mBluetoothAdapter.isEnabled()) {
				Log.i(TAG, "startup() : " + "ble disabled, asking user...");

				Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
				UnityPlayer.currentActivity.startActivityForResult(enableBtIntent, ActivityResultCodes.REQUEST_ENABLE_BT);
			} else {
				Log.i(TAG, "startup() : " + "ble enabled...");

				UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Powered On");
			}
		} else {

			Log.i(TAG, "startup() : " + "ble not supported...");

			UnityPlayer.UnitySendMessage(gameObjectName, "OnBleStateUpdate", "Unsupported");
		}

	}
	
	private void sendCharacteristicUpdate(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
	{
		String cValue = Base64.encodeToString(characteristic.getValue(), Base64.DEFAULT);
		
		String message = String.format("%d:%s%d:%s%d:%s%d:%s", gatt.getDevice().getAddress().length(), gatt.getDevice().getAddress(), 
				characteristic.getService().getUuid().toString().length(), characteristic.getService().getUuid().toString(),
				characteristic.getUuid().toString().length(), characteristic.getUuid().toString(), 
				cValue.length(), cValue);
                    
        UnityPlayer.UnitySendMessage(gameObjectName, "OnBluetoothData", message);
	}
	

	private final BluetoothGattCallback mGattCallback = new BluetoothGattCallback() 
	{
		@Override
		public void onReadRemoteRssi (BluetoothGatt gatt, int rssi, int status)
		{
			
			Log.i(TAG, "onReadRemoteRssi() : " + gatt.getDevice().getAddress());
			
			if(status == BluetoothGatt.GATT_SUCCESS)
			{
				String message = String.format("%d:%s%d:%s", gatt.getDevice().getAddress().length(), gatt.getDevice().getAddress(), (rssi+"").length(), rssi);

				UnityPlayer.UnitySendMessage(gameObjectName, "OnRssiUpdate", message);
			}
			
			checkGattQueue();
			
		}
		
		@Override
		public void onCharacteristicChanged (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
		{
			Log.i(TAG, "onCharacteristicChanged() : " + characteristic.getUuid().toString());
			sendCharacteristicUpdate(gatt, characteristic);
		}
		
		@Override
		public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status)
		{
			if(status == BluetoothGatt.GATT_SUCCESS)
			{
				Log.i(TAG, "onCharacteristicRead() : " + characteristic.getUuid().toString());
				sendCharacteristicUpdate(gatt, characteristic);
			}
			else
			{
				Log.i(TAG, "onCharacteristicRead() : failed : " + status);
			}
			
			checkGattQueue();
		}
		
		@Override
		public void onCharacteristicWrite (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status)
		{
			if(status == BluetoothGatt.GATT_SUCCESS)
			{
				if( characteristic.getWriteType() == BluetoothGattCharacteristic.WRITE_TYPE_DEFAULT )
				{
				Log.i(TAG, "onCharacteristicWrite() : " + characteristic.getUuid().toString());
				
				String peripheralAddress = gatt.getDevice().getAddress();
										
					String message = String.format("%d:%s%d:%s%d:%s", peripheralAddress.length(), peripheralAddress,
							characteristic.getService().getUuid().toString().length(), characteristic.getService().getUuid().toString(),
							characteristic.getUuid().toString().length(), characteristic.getUuid().toString() );
			    				
			    UnityPlayer.UnitySendMessage (gameObjectName, "OnDidWriteCharacteristic", message);
				}
				
			}
			else
			{
				Log.i(TAG, "onCharacteristicWrite() : failed : " + status);
			}
			
			checkGattQueue();
		}
		
		@Override
		public void onDescriptorRead (BluetoothGatt gatt, BluetoothGattDescriptor descriptor, int status)
		{
			if(status == BluetoothGatt.GATT_SUCCESS)
			{
				Log.i(TAG, "onDescriptorRead() : " + descriptor.getUuid().toString());
				
			BluetoothGattCharacteristic characteristic = descriptor.getCharacteristic();
			
			String dValue = Base64.encodeToString(descriptor.getValue(), Base64.DEFAULT);
			
				String message = String.format("%d:%s%d:%s%d:%s%d:%s%d:%s", gatt.getDevice().getAddress().length(), gatt.getDevice().getAddress(), 
						characteristic.getService().getUuid().toString().length(), characteristic.getService().getUuid().toString(),
						characteristic.getUuid().toString().length(), characteristic.getUuid().toString(), 
						descriptor.getUuid().toString().length(), descriptor.getUuid().toString(), 
						dValue.length(), dValue);
	                    
	        UnityPlayer.UnitySendMessage(gameObjectName, "OnDescriptorRead", message);
		}
			else
			{
				Log.i(TAG, "onDescriptorRead() : failed : " + status);
			}
			
			checkGattQueue();
			
		}
		
		@Override
		public void onDescriptorWrite (BluetoothGatt gatt, BluetoothGattDescriptor descriptor, int status)
		{
			if(status == BluetoothGatt.GATT_SUCCESS)
			{
				BluetoothGattCharacteristic characteristic = descriptor.getCharacteristic();
				
				Log.i(TAG, "onDescriptorWrite() : " + descriptor.getUuid().toString());
				
				String peripheralAddress = gatt.getDevice().getAddress();

				String message = String.format("%d:%s%d:%s%d:%s%d:%s", 
						peripheralAddress.length(), peripheralAddress, 
						characteristic.getService().getUuid().toString().length(), characteristic.getService().getUuid().toString(),
						characteristic.getUuid().toString().length(), characteristic.getUuid().toString(), 
						descriptor.getUuid().toString().length(), descriptor.getUuid().toString() );
			    				
			    UnityPlayer.UnitySendMessage (gameObjectName, "OnDidWriteDescriptor", message);
				
			}
			else
			{
				Log.i(TAG, "onDescriptorWrite() : failed : " + status);
			}
			
			checkGattQueue();
		}
		
		@Override
		public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {

			Log.i(TAG, "onConnectionStateChange()");

			// String intentAction;

			if (newState == BluetoothProfile.STATE_CONNECTED) {
				Log.i(TAG, "onConnectionStateChange() : BluetoothProfile.STATE_CONNECTED");

				BluetoothDevice device = gatt.getDevice();

				BluetoothPeripheral peripheral = peripherals.get(device.getAddress());

				if (peripheral != null) {
					Log.i(TAG, "onConnectionStateChange() : BluetoothProfile.STATE_CONNECTED = " + peripheral.device.getAddress());

					peripheral.gatt = gatt;
					onUpdatePeripheral(peripheral, "OnConnectedPeripheral", peripheral.advertisedData.getName());
					gatt.discoverServices();

				}
			} else if (newState == BluetoothProfile.STATE_DISCONNECTED) {

				Log.i(TAG, "onConnectionStateChange() : BluetoothProfile.STATE_DISCONNECTED");

				BluetoothDevice device = gatt.getDevice();

				BluetoothPeripheral peripheral = peripherals.get(device.getAddress());

				if (peripheral != null)
				{
					peripheral.gatt.close();
					onUpdatePeripheral(peripheral, "OnDisconnectedPeripheral", peripheral.advertisedData.getName());
				}
			}
		}

		@Override
		// New services discovered
		public void onServicesDiscovered(BluetoothGatt gatt, int status) {

			Log.i(TAG, "onServicesDiscovered()");

			if (status == BluetoothGatt.GATT_SUCCESS) {
				List<BluetoothGattService> services = gatt.getServices();
				for (int i = 0; i < services.size(); i++) {

					String peripheralId = "Unknown";

					BluetoothPeripheral peripheral = peripherals.get(gatt.getDevice().getAddress());

					if (peripheral != null)
						peripheralId = peripheral.device.getAddress();

					String sUuid = services.get(i).getUuid().toString();

					if(peripheralFilter.filterWith(sUuid, PeripheralFilter.SERVICE_UUID) && peripherals.containsKey(peripheralId))
					{
												
						Log.i(TAG, "onServicesDiscovered() : sUuid matched filter = " + sUuid);
	
						PeripheralService peripheralService = new PeripheralService(services.get(i));
						
						peripherals.get(peripheralId).services.put(sUuid, peripheralService);
	
						String message = String.format("%d:%s%d:%s", peripheralId.length(), peripheralId, sUuid.length(), sUuid);
	
						UnityPlayer.UnitySendMessage(gameObjectName, "OnDiscoveredService", message);
	
						List<BluetoothGattCharacteristic> chars = services.get(i).getCharacteristics();
	
						for (int j = 0; j < chars.size(); j++) {
							String cUuid = chars.get(j).getUuid().toString();
	
							Log.i(TAG, "onServicesDiscovered() : cUuid = " + cUuid);
																					
								PeripheralCharacteristic peripheralCharacteristic = new PeripheralCharacteristic(chars.get(j));
								
							peripheralService.characteristics.put(cUuid, peripheralCharacteristic);
								
							message = String.format("%d:%s%d:%s%d:%s", peripheralId.length(), peripheralId, 
									sUuid.length(), sUuid,
									cUuid.length(), cUuid);
								
								UnityPlayer.UnitySendMessage(gameObjectName, "OnDiscoveredCharacteristic", message);
								
								List<BluetoothGattDescriptor> descriptors = chars.get(j).getDescriptors();
								
								for(int k = 0; k < descriptors.size(); k++)
								{
									BluetoothGattDescriptor descriptor = descriptors.get(k);
									
									peripheralCharacteristic.descriptors.put(descriptor.getUuid().toString(), descriptor);
									
								message = String.format("%d:%s%d:%s%d:%s%d:%s", 
										peripheralId.length(), peripheralId, 
										sUuid.length(), sUuid,
										cUuid.length(), cUuid,
										descriptor.getUuid().toString().length(), descriptor.getUuid().toString());
									
									UnityPlayer.UnitySendMessage(gameObjectName, "OnDiscoveredDescriptor", message);
								
								}//descriptors loop
					

						} //chars loop
					}
					else
					{
						Log.i(TAG, "onServicesDiscovered() : sUuid mismatched filter = " + sUuid);
					}
				}
			} else {
				Log.w(TAG, "onServicesDiscovered received: " + status);
			}
		}

	};

	public void shutdown() {
		Log.i(TAG, "shutdown()");

		UnityPlayer.UnitySendMessage(gameObjectName, "OnShutdown", "Inactive");

		DeregisterBroadcastReciever(UnityPlayer.currentActivity);

		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			if (mScanning) {
				mScanning = false;
				mBluetoothAdapter.stopLeScan(mLeScanCallback);
			}
		}
	}

	public void pauseWithState(boolean isPaused) {
		Log.i(TAG, "pauseWithState(" + isPaused + ")");
	}
	
	private void scanForPeripherals()
	{
		Log.i(TAG, "scanForPeripherals()");

		if (mScanning) {
			Log.i(TAG, "scanForPeripherals() : " + "already scanning...");
			return;
		}

		// Stops scanning after a pre-defined scan period.
		mHandler.postDelayed(new Runnable() {
			@Override
			public void run() {
				if (mScanning) {
					mScanning = false;
					mBluetoothAdapter.stopLeScan(mLeScanCallback);
					Log.i(TAG, "scanForPeripherals() : " + "scanning timeout...");
				}
			}
		}, SCAN_PERIOD);

		mScanning = true;

		Log.i(TAG, "scanForPeripherals() : " + "starting scan...");

		mBluetoothAdapter.startLeScan(mLeScanCallback);
	}

	public void scanForPeripheralsWithServiceUUIDs(String serviceUUIDsString) {
		
		if(serviceUUIDsString != null && serviceUUIDsString.length() > 0)
		{
			String[] uuids = serviceUUIDsString.split("\\|");
			
			
			if(uuids.length > 0)
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.SERVICE_UUID);
				
				for(int i = 0; i < uuids.length; i++)
				{
					peripheralFilter.addUuid(uuids[i]);
					
				}
			}
			else
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			}
			
		}
		else
		{
			peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			
		}
				

		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			scanForPeripherals();
		} else {
			Log.i(TAG, "scanForPeripheralsWithServiceUUIDs() : Bluetooth Adapter Disabled...");
		}

	}

	public void connectToPeripheralWithIdentifier(String peripheralId) {
		
		Log.i(TAG, "connectToPeripheralWithIdentifier()");
		
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) 
		{
			Log.i(TAG, "connectToPeripheralWithIdentifier(" + (peripheralId == null ? "null" : peripheralId) + ")");

			if (peripherals.containsKey(peripheralId)) {
				if (mScanning)
					stopScanning();

				Log.i(TAG, "connectToPeripheralWithIdentifier() : connecting...");
				BluetoothPeripheral peripheral = peripherals.get(peripheralId);
				peripheral.device.connectGatt(context, false, mGattCallback);
			}
		} else {
			Log.i(TAG, "connectToPeripheralWithIdentifier() : Bluetooth Adapter Disabled...");
		}

	}

	public void disconnectFromPeripheralWithIdentifier(String peripheralId) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "disconnectFromPeripheralWithIdentifier()");
			
			if (peripherals.containsKey(peripheralId)) {
				
				if (mScanning)
					stopScanning();

				Log.i(TAG, "disconnectFromPeripheralWithIdentifier() : disconnecting...");
				
				BluetoothPeripheral peripheral = peripherals.get(peripheralId);
				
				if(peripheral.gatt != null)
				{
					peripheral.gatt.close();
					//peripheral.gatt.disconnect();
				}
			}
			
		} else {
			Log.i(TAG, "disconnectFromPeripheralWithIdentifier() :  Bluetooth Adapter Disabled...");
		}
	}

	public void retrieveListOfPeripheralsWithServiceUUIDs(String serviceUUIDsString) {
		
		if(serviceUUIDsString != null && serviceUUIDsString.length() > 0)
		{
			String[] uuids = serviceUUIDsString.split("\\|");
			
			
			if(uuids.length > 0)
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.SERVICE_UUID);
				
				for(int i = 0; i < uuids.length; i++)
				{
					peripheralFilter.addUuid(uuids[i]);
					
				}
			}
			else
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			}
			
		}
		else
		{
			peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			
		}
		
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "retrieveListOfPeripheralsWithServiceUUIDs()" + serviceUUIDsString);
			scanForPeripherals();
		} else {
			Log.i(TAG, "retrieveListOfPeripheralsWithServiceUUIDs() :  Bluetooth Adapter Disabled...");
		}
	}

	public void retrieveListOfPeripheralsWithUUIDs(String uuidsString) {
		
		if(uuidsString != null && uuidsString.length() > 0)
		{
			String[] uuids = uuidsString.split("\\|");
			
			
			if(uuids.length > 0)
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.PERIPHERAL_UUID);
				
				for(int i = 0; i < uuids.length; i++)
				{
					peripheralFilter.addUuid(uuids[i]);
					
				}
			}
			else
			{
				peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			}
			
		}
		else
		{
			peripheralFilter = new PeripheralFilter(PeripheralFilter.NONE);
			
		}
		
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "retrieveListOfPeripheralsWithUUIDs()");
			scanForPeripherals();
		} else {
			Log.i(TAG, "retrieveListOfPeripheralsWithUUIDs() :  Bluetooth Adapter Disabled...");
		}
	}

	public void stopScanning() {

		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "stopScanning()");
			
			if (mScanning) {
				Log.i(TAG, "stopScanning()");
				mScanning = false;
				mBluetoothAdapter.stopLeScan(mLeScanCallback);
			}
			
		} else {
			Log.i(TAG, "stopScanning() :  Bluetooth Adapter Disabled...");
		}

		
	}

	public void subscribeToCharacteristicWithIdentifiers(String peripheralId, String serviceId, String characteristicId, boolean isIndication) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "subscribeToCharacteristicWithIdentifiers()");
			
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothGatt gatt = peripherals.get(peripheralId).gatt;
					
					BluetoothGattCharacteristic characteristic = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).characteristic;
					
					Log.i(TAG, "subscribeToCharacteristicWithIdentifiers() : " + characteristicId);
					
					gatt.setCharacteristicNotification(characteristic, true);
					
					BluetoothGattDescriptor descriptor = characteristic.getDescriptor(CLIENT_CHARACTERISTIC_CONFIGURATION);
					
					if(descriptor != null)
					{
						if(isIndication)
						{
							Log.i(TAG, "subscribeToCharacteristicWithIdentifiers() : Indication...");
							processGattComm(new GattComm(gatt, descriptor, GattComm.WRITE_DESCRIPTOR, BluetoothGattDescriptor.ENABLE_INDICATION_VALUE));
						}
						else
						{
							Log.i(TAG, "subscribeToCharacteristicWithIdentifiers() : Notification...");
							processGattComm(new GattComm(gatt, descriptor, GattComm.WRITE_DESCRIPTOR, BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE));
						}
						
					}
					
				}
			}
			
		} else {
			Log.i(TAG, "subscribeToCharacteristicWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}

	public void unSubscribeFromCharacteristicWithIdentifiers(String peripheralId, String serviceId, String characteristicId) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "unSubscribeFromCharacteristicWithIdentifiers()");
			
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothGatt gatt = peripherals.get(peripheralId).gatt;
					BluetoothGattCharacteristic characteristic = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).characteristic;
					
					gatt.setCharacteristicNotification(characteristic, false);
					
					BluetoothGattDescriptor descriptor = characteristic.getDescriptor(CLIENT_CHARACTERISTIC_CONFIGURATION);
					
					if(descriptor != null)
					{
						processGattComm(new GattComm(gatt, descriptor, GattComm.WRITE_DESCRIPTOR, BluetoothGattDescriptor.DISABLE_NOTIFICATION_VALUE));
					}
					
				}
			}
			
		} else {
			Log.i(TAG, "unSubscribeFromCharacteristicWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}

	public void readCharacteristicWithIdentifiers(String peripheralId, String serviceId, String characteristicId) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "readCharacteristicWithIdentifiers()");
			
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothPeripheral peripheral = peripherals.get(peripheralId);
					BluetoothGatt gatt = peripheral.gatt;
					
					BluetoothGattCharacteristic characteristic = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).characteristic;
					
					processGattComm(new GattComm(gatt, characteristic, GattComm.READ_CHARACTERISTIC, null));
				}
			}
			
		} else {
			Log.i(TAG, "readCharacteristicWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}
	
	public void readDescriptorWithIdentifiers(String peripheralId, String serviceId, String characteristicId, String descriptorId) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "readCharacteristicWithIdentifiers()");
			
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothGatt gatt = peripherals.get(peripheralId).gatt;
					BluetoothGattDescriptor descriptor = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).descriptors.get(descriptorId);
					
					if(descriptor != null)
					{
						processGattComm(new GattComm(gatt, descriptor, GattComm.READ_DESCRIPTOR, null));
					}
					
				}
			}
			
		} else {
			Log.i(TAG, "readCharacteristicWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}

	public void writeCharacteristicWithIdentifiers(String peripheralId, String serviceId, String characteristicId, byte[] data, int length, boolean withResponse) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "writeCharacteristicWithIdentifiers()");
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothGatt gatt = peripherals.get(peripheralId).gatt;
					BluetoothGattCharacteristic characteristic = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).characteristic;
					
					GattComm gattComm = new GattComm(gatt, characteristic, GattComm.WRITE_CHARACTERISTIC, data);
					
					
					if(withResponse)
						gattComm.setWriteType(BluetoothGattCharacteristic.WRITE_TYPE_DEFAULT);
					else
						gattComm.setWriteType(BluetoothGattCharacteristic.WRITE_TYPE_NO_RESPONSE);
					
					processGattComm(gattComm);
					
				}
			}
		} else {
			Log.i(TAG, "writeCharacteristicWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}
	
	
	
	public void writeDescriptorWithIdentifiers(String peripheralId, String serviceId, String characteristicId, String descriptorId, byte[] data, int length) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "writeDescriptorWithIdentifiers()");
			if(peripheralId != null && serviceId != null && characteristicId != null)
			{
				if(peripherals.containsKey(peripheralId) && 
						peripherals.get(peripheralId).services.containsKey(serviceId) && 
						peripherals.get(peripheralId).services.get(serviceId).characteristics.containsKey(characteristicId))
				{
					BluetoothGatt gatt = peripherals.get(peripheralId).gatt;
					BluetoothGattDescriptor descriptor = peripherals.get(peripheralId).services.get(serviceId).characteristics.get(characteristicId).descriptors.get(descriptorId);
					
					if(descriptor != null)
					{
						processGattComm(new GattComm(gatt, descriptor, GattComm.WRITE_DESCRIPTOR, data));
					}
					
				}
			}
		} else {
			Log.i(TAG, "writeDescriptorWithIdentifiers() :  Bluetooth Adapter Disabled...");
		}
	}
	
	public void readRssiWithIdentifier(String peripheralId) {
		if (mBluetoothAdapter != null && mBluetoothAdapter.isEnabled()) {
			Log.i(TAG, "readRssiWithIdentifier()");
			if(peripheralId != null)
			{
				if(peripherals.containsKey(peripheralId))
				{
					BluetoothPeripheral peripheral = peripherals.get(peripheralId);
					BluetoothGatt gatt = peripheral.gatt;
					
					String message = String.format("%d:%s%d:%s", peripheral.device.getAddress().length(), peripheral.device.getAddress(), (peripheral.rssi+"").length(), peripheral.rssi);

					UnityPlayer.UnitySendMessage(gameObjectName, "OnRssiUpdate", message);
					
					processGattComm(new GattComm(gatt, null, GattComm.READ_RSSI, null));
					
				}
			}
		} else {
			Log.i(TAG, "readRssiWithIdentifier() :  Bluetooth Adapter Disabled...");
		}
	}
}
