package com.startechplus.unityblebridge;

import java.util.HashMap;

import android.bluetooth.BluetoothGattService;

public class PeripheralService {
	public BluetoothGattService service;
	public HashMap<String, PeripheralCharacteristic> characteristics = new HashMap<String, PeripheralCharacteristic>();
	
	public PeripheralService(BluetoothGattService s)
	{
		service = s;
	}
}
