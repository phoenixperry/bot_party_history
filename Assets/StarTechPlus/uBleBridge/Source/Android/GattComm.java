package com.startechplus.unityblebridge;

import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCharacteristic;

public class GattComm {
	public static final int READ_RSSI = 0;
	public static final int WRITE_DESCRIPTOR = 1;
	public static final int WRITE_CHARACTERISTIC = 2;
	public static final int READ_DESCRIPTOR = 3;
	public static final int READ_CHARACTERISTIC = 4;
	
	public BluetoothGatt gatt;
	public Object actor;
	public int type;
	public byte[] data;
	public int writeType;
	
	public GattComm(BluetoothGatt g, Object a, int t, byte[] d)
	{
		gatt = g;
		actor = a;
		type = t;
		data = d;
		writeType = BluetoothGattCharacteristic.WRITE_TYPE_DEFAULT;
	}
	
	public void setWriteType(int t)
	{
		writeType = t;
	}
}
