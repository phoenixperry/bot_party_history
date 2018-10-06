package com.startechplus.unityblebridge;

import java.util.HashMap;

import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;

public class PeripheralCharacteristic {
	public BluetoothGattCharacteristic characteristic;
	public HashMap<String, BluetoothGattDescriptor> descriptors = new HashMap<String, BluetoothGattDescriptor>();
	
	public PeripheralCharacteristic(BluetoothGattCharacteristic c)
	{
		characteristic = c;
	}
}
