/*
 * Copyright (C) 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.robotis.bluetooth;

// Add "<service android:enabled="true" android:name="com.robotis.sdk.UartService" />" to AndroidManifest.xml

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.UUID;

import android.app.Service;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothProfile;
import android.content.Context;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

/**
 * Service for managing connection and data communication with a GATT server
 * hosted on a given Bluetooth LE device.
 */
public class UartService extends Service
{
	private final static String TAG = "ROBOTIS_UART";
	public final static String ACTION_GATT_CONNECTED = "com.robotis.ble.ACTION_GATT_CONNECTED";
	public final static String ACTION_GATT_DISCONNECTED = "com.robotis.ble.ACTION_GATT_DISCONNECTED";
	public final static String ACTION_GATT_SERVICES_DISCOVERED = "com.robotis.ble.ACTION_GATT_SERVICES_DISCOVERED";
	public final static String ACTION_DATA_AVAILABLE = "com.robotis.ble.ACTION_DATA_AVAILABLE";
	public final static String EXTRA_DATA = "com.robotis.ble.EXTRA_DATA";
	public final static String EXTRA_ADDRESS = "com.robotis.ble.EXTRA_ADDRESS";
	public final static String DEVICE_DOES_NOT_SUPPORT_UART = "com.robotis.ble.DEVICE_DOES_NOT_SUPPORT_UART";
	public static final UUID TX_POWER_UUID = UUID.fromString("00001804-0000-1000-8000-00805f9b34fb");
	public static final UUID TX_POWER_LEVEL_UUID = UUID.fromString("00002a07-0000-1000-8000-00805f9b34fb");
	public static final UUID CCCD = UUID.fromString("00002902-0000-1000-8000-00805f9b34fb");
	public static final UUID FIRMWARE_REVISON_UUID = UUID.fromString("00002a26-0000-1000-8000-00805f9b34fb");
	public static final UUID DIS_UUID = UUID.fromString("0000180a-0000-1000-8000-00805f9b34fb");
	public static final UUID RX_SERVICE_UUID = UUID.fromString("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
	public static final UUID RX_CHAR_UUID = UUID.fromString("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
	public static final UUID TX_CHAR_UUID = UUID.fromString("6e400003-b5a3-f393-e0a9-e50e24dcca9e");

	private final IBinder mBinder = new LocalBinder();
	private BluetoothManager mBluetoothManager;
	private BluetoothAdapter mBluetoothAdapter;
	private HashMap<String, ArrayList<Byte>> mReceivedMap;
	private BluetoothGatt bluetoothGatt;
	private String mAddress;
	private boolean mWritable = true;

	private final BluetoothGattCallback mGattCallback = new BluetoothGattCallback()
	{
		@Override
		public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState)
		{
			String intentAction;

			if (newState == BluetoothProfile.STATE_CONNECTED)
			{
				intentAction = ACTION_GATT_CONNECTED;
				broadcastUpdate(intentAction, gatt.getDevice().getAddress());
				Log.i(TAG, "Connected to GATT server.");
				Log.i(TAG, "Attempting to start service discovery:" + gatt.discoverServices());
				Log.i(TAG, "Address : " + gatt.getDevice().getAddress());
			}
			else if (newState == BluetoothProfile.STATE_DISCONNECTED)
			{
				intentAction = ACTION_GATT_DISCONNECTED;
				Log.i(TAG, "Disconnected from GATT server. " + gatt.getDevice().getAddress());
				broadcastUpdate(intentAction, gatt.getDevice().getAddress());
			}
		}

		@Override
		public void onServicesDiscovered(BluetoothGatt gatt, int status)
		{
			if (status == BluetoothGatt.GATT_SUCCESS)
			{
				broadcastUpdate(ACTION_GATT_SERVICES_DISCOVERED, gatt.getDevice().getAddress());
			}
		}

		@Override
		public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status)
		{
			if (status == BluetoothGatt.GATT_SUCCESS)
			{
				broadcastUpdate(ACTION_DATA_AVAILABLE, characteristic, gatt.getDevice().getAddress());

				byte[] value = characteristic.getValue();
				Log.i(TAG, "onCharacteristicRead " + value);
			}
		}

		@Override
		public void onCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
		{
			broadcastUpdate(ACTION_DATA_AVAILABLE, characteristic, gatt.getDevice().getAddress());

			byte[] value = characteristic.getValue();
			Log.i(TAG, "onCharacteristicChanged " + value);

			synchronized (mReceivedMap)
			{
				if (!mReceivedMap.containsKey(gatt.getDevice().getAddress()))
				{
					mReceivedMap.put(gatt.getDevice().getAddress(), new ArrayList<Byte>());
				}

				for (int i = 0; i < value.length; i++)
				{
					mReceivedMap.get(gatt.getDevice().getAddress()).add(value[i]);
				}
			}
		}

		@Override
		public void onCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status)
		{
			super.onCharacteristicWrite(gatt, characteristic, status);
			mWritable = true;
			Log.i(TAG, "onCharacteristicWrite");
		}
	};

	private void broadcastUpdate(final String action, final String address)
	{
		final Intent intent = new Intent(action);
		intent.putExtra(EXTRA_ADDRESS, address);
		LocalBroadcastManager.getInstance(this).sendBroadcast(intent);
	}

	private void broadcastUpdate(final String action, final BluetoothGattCharacteristic characteristic, final String address)
	{
		final Intent intent = new Intent(action);
		if (TX_CHAR_UUID.equals(characteristic.getUuid()))
		{
			intent.putExtra(EXTRA_DATA, characteristic.getValue());
		}
		else
		{

		}
		LocalBroadcastManager.getInstance(this).sendBroadcast(intent);
	}

	public class LocalBinder extends Binder
	{
		public UartService getService()
		{
			return UartService.this;
		}
	}

	@Override
	public IBinder onBind(Intent intent)
	{
		Log.d(TAG, "Bind Service");
		return mBinder;
	}

	@Override
	public void onCreate()
	{
		super.onCreate();
		
		Log.d(TAG, "Create Service");
		mReceivedMap = new HashMap<String, ArrayList<Byte>>();
	}

	@Override
	public boolean onUnbind(Intent intent)
	{
		Log.d(TAG, "Unbind Service");
		
		disconnect();
		close();
		return super.onUnbind(intent);
	}

	public boolean initialize()
	{
		if (mBluetoothManager == null)
		{
			mBluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
			if (mBluetoothManager == null)
			{
				return false;
			}
		}

		mBluetoothAdapter = mBluetoothManager.getAdapter();
		if (mBluetoothAdapter == null)
		{
			return false;
		}

		return true;
	}

	public boolean connect(final String address)
	{
		if (mBluetoothAdapter == null || address == null)
		{
			Log.w(TAG, "BluetoothAdapter not initialized or unspecified address.");
			return false;
		}

		// Previously connected device. Try to reconnect.
		if (bluetoothGatt != null && bluetoothGatt.getDevice().getAddress().equals(address))
		{
			Log.d(TAG, "Trying to use an existing mBluetoothGatt for connection.");
			return bluetoothGatt.connect();
		}

		final BluetoothDevice device = mBluetoothAdapter.getRemoteDevice(address);
		if (device == null)
		{
			Log.w(TAG, "Device not found.  Unable to connect.");
			return false;
		}

		disconnect();
		close();

		bluetoothGatt = device.connectGatt(this, false, mGattCallback);
		Log.d(TAG, "Trying to create a new connection.");
		mAddress = address;
		return true;
	}

	public void disconnect()
	{
		if (mBluetoothAdapter == null || bluetoothGatt == null)
			return;

		bluetoothGatt.disconnect();
	}

	public void close()
	{
		if (bluetoothGatt == null)
			return;

		bluetoothGatt.close();
		bluetoothGatt = null;
	}

	public void enableTXNotification(String address)
	{
		BluetoothGattService RxService = bluetoothGatt.getService(RX_SERVICE_UUID);
		if (RxService == null)
		{
			Log.d(TAG, "Rx service not found! enableTXNotification " + bluetoothGatt.getDevice().getAddress());
			broadcastUpdate(DEVICE_DOES_NOT_SUPPORT_UART, address);
			return;
		}
		BluetoothGattCharacteristic TxChar = RxService.getCharacteristic(TX_CHAR_UUID);
		if (TxChar == null)
		{
			Log.d(TAG, "Tx charateristic not found!");
			broadcastUpdate(DEVICE_DOES_NOT_SUPPORT_UART, address);
			return;
		}
		bluetoothGatt.setCharacteristicNotification(TxChar, true);

		BluetoothGattDescriptor descriptor = TxChar.getDescriptor(CCCD);
		descriptor.setValue(BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE);
		bluetoothGatt.writeDescriptor(descriptor);

	}

	public void writeRXCharacteristic(byte[] value)
	{
		ArrayList<byte[]> buffer20 = divideArray(value, 20);

		BluetoothGattService RxService = bluetoothGatt.getService(RX_SERVICE_UUID);
		if (RxService == null)
		{
			Log.e(TAG, "Rx service not found! writeRXCharacteristic " + bluetoothGatt.getDevice().getAddress());
			broadcastUpdate(DEVICE_DOES_NOT_SUPPORT_UART, bluetoothGatt.getDevice().getAddress());
			return;
		}
		BluetoothGattCharacteristic RxChar = RxService.getCharacteristic(RX_CHAR_UUID);
		if (RxChar == null)
		{
			Log.e(TAG, "Rx charateristic not found!");
			broadcastUpdate(DEVICE_DOES_NOT_SUPPORT_UART, bluetoothGatt.getDevice().getAddress());
			return;
		}

		Log.i(TAG, "BLE Write:" + value.length + ":" + value);
		for (int i = 0; i < buffer20.size(); i++)
		{
			byte[] txValue = buffer20.get(i);
			RxChar.setValue(txValue);
			// RxChar.setWriteType(BluetoothGattCharacteristic.WRITE_TYPE_NO_RESPONSE);

			long startTime = System.currentTimeMillis();
			while ((!mWritable) && System.currentTimeMillis() - startTime < 100)
				;

			mWritable = false;
			boolean status = bluetoothGatt.writeCharacteristic(RxChar);
			if (!status)
				Log((status ? "-O" : "-X") + "-T: ", txValue);
		}
	}

	public static ArrayList<byte[]> divideArray(byte[] source, int chunksize)
	{
		ArrayList<byte[]> ret = new ArrayList<byte[]>();

		int share = (int) Math.ceil(source.length / (double) chunksize);

		if (share <= 1)
		{
			ret.add(source);
		}
		else
		{
			int start = 0;
			for (int i = 0; i < share; i++)
			{
				if (i >= share - 1)
				{
					int remainder = source.length % chunksize;
					if(remainder == 0) remainder += chunksize; // @@ 나머지가 0되는경우 확인해봐야함
					byte[] part = Arrays.copyOfRange(source, start, start + remainder);
					ret.add(part);
				}
				else
				{
					byte[] part = Arrays.copyOfRange(source, start, start + chunksize);
					start += chunksize;
					ret.add(part);
				}
			}
		}

		return ret;
	}

	public int available()
	{
		int count = 0;
		
		synchronized (mReceivedMap)
		{			
			if (mAddress == null)
			{
				Iterator<String> iterator = mReceivedMap.keySet().iterator();
				if (iterator.hasNext())
				{
					mAddress = iterator.next();
				}
			}			

			if (mReceivedMap.get(mAddress) != null)
			{	
				count = mReceivedMap.get(mAddress).size();
				Log.d(TAG, "Avaliable: " + count);
			}
		}
		return count;
	}

	public Object read()
	{
		if (mReceivedMap == null || mReceivedMap.size() < 1)
			return null;

		synchronized (mReceivedMap)
		{
			if (mAddress == null)
			{
				Iterator<String> iterator = mReceivedMap.keySet().iterator();
				if (iterator.hasNext())
					mAddress = iterator.next();
			}

			if (mReceivedMap.get(mAddress) != null)
			{
				byte data = mReceivedMap.get(mAddress).get(0);
				mReceivedMap.get(mAddress).remove(0);

				return data;
			}
		}

		return null;
	}

	@Override
	public void onDestroy()
	{
		super.onDestroy();
		Log.i(TAG, "UartService is destroied......................");
	}

	public static void Log(String headStr, byte[] value)
	{
		// StringBuilder sb = new StringBuilder();
		// for (byte b : value)
		// {
		// sb.append(String.format("%02x", (0xFF & b)).toLowerCase() + ", ");
		// }
		//
		// Log.w(TAG, headStr + sb.toString());
	}
}