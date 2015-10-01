package com.smartmaker.android;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.Set;
import java.util.ArrayList;
import java.util.List;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.IBinder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.robotis.sdk.UartService;
import com.unity3d.player.*;


public class CommBluetooth
{
	private static CommBluetooth _Instance = null;
	private static String _logTag = "CommBluetooth";
	private Context _context;
	private static final UUID SPP_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private BluetoothAdapter _btAdapter;
	private BluetoothDevice _btDevice;
	private BluetoothSocket _btSocket;
	private InputStream _InStream;
	private OutputStream _OutStream;
	private UartService _bleService;
	private boolean _isOpen = false;
	private boolean _isSupportBLE = false;
	private String _unityObject;
	private String _unityMethodErrorClose;
	private String _unityMethodFoundDevice;
	private String _unityMethodSearchCompleted;

	
	public static CommBluetooth GetInstance()
    {
		Log.d(_logTag, "GetInstance");
		
        if(_Instance == null)
            _Instance = new CommBluetooth();        
        
        return _Instance;
    }
	
	public boolean Initialize(Context context, String unityObject)
    {
		Log.d(_logTag, "Initialize");
		
		_btAdapter = BluetoothAdapter.getDefaultAdapter();
        if(_btAdapter == null)
        	return false;
		
        _context = context;
        _unityObject = unityObject;
        _isSupportBLE = (Build.VERSION.SDK_INT >= 19 && _context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE));
        
        if(_isSupportBLE)
        {
        	Intent bindIntent = new Intent(_context, UartService.class);
        	_context.bindService(bindIntent, _serviceConnection, Context.BIND_AUTO_CREATE);
        	LocalBroadcastManager.getInstance(_context).registerReceiver(_serviceReceiver, makeGattUpdateIntentFilter());
        }
        return true;
    }
	
	public void Terminate()
	{
		Log.d(_logTag, "Terminate");
		
		if(_isSupportBLE)
        {
			if (_bleService != null)
			{
				_bleService.disconnect();
				_bleService.close();
			}

			try
			{
				LocalBroadcastManager.getInstance(_context).unregisterReceiver(_serviceReceiver);
			}
			catch (Exception e)
			{
			}

			try
			{
				_context.unbindService(_serviceConnection);
			}
			catch (Exception e)
			{
			}        
        }
	}
	
	public void SetUnityMethodErrorClose(String unityMethod)
	{
		_unityMethodErrorClose = unityMethod;
	}
	
	public void SetUnityMethodFoundDevice(String unityMethod)
	{
		_unityMethodFoundDevice = unityMethod;
	}
	
	public void SetUnityMethodSearchCompleted(String unityMethod)
	{
		_unityMethodSearchCompleted = unityMethod;
	}
	
	public boolean IsSupportBLE()
	{
		return _isSupportBLE;
	}
	
	public synchronized String[] GetBondedDevices()
    {
		Log.d(_logTag, "GetBondedDevices");
		
        List<String> btDevices = new ArrayList<String>();
        
        if(_btAdapter.isEnabled())
        {
        	try
            {
                Set<BluetoothDevice> bondedDevices = _btAdapter.getBondedDevices();
                if (bondedDevices.size() > 0)
                {
                    for (BluetoothDevice bd : bondedDevices)
                    {
                    	if(deviceFilter(bd.getAddress()))
                    		btDevices.add(String.format("%s,%s", bd.getName(), bd.getAddress()));
                    }                    	
                }
            }
            catch (Exception e)
            {
            }
        }
        else
            _context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
        
        return btDevices.toArray(new String[btDevices.size()]);
    }
	
	public void SearchDevice()
	{
		
	}
	
	public synchronized boolean Open(String address)
	{
		Log.d(_logTag, "Open");
		
		if(!_btAdapter.isEnabled())
		{
			_context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
			return false;
		}
		
		if(_isOpen)
		{
			if(!_btDevice.getAddress().equals(address))
				close();
			else
				return true;
		}

		_btDevice = _btAdapter.getRemoteDevice(address);
		if(_isSupportBLE)
		{
			if(_btDevice.getType() == BluetoothDevice.DEVICE_TYPE_CLASSIC)
			{
				try
				{
					_btSocket = _btDevice.createRfcommSocketToServiceRecord(SPP_UUID);
					_btSocket.connect();
					_InStream = _btSocket.getInputStream();
					_OutStream = _btSocket.getOutputStream();
					_isOpen = true;
				}
				catch (IOException e)
				{
					Log.d(_logTag, "Open Failed");    				
					close();
				}
			}
			else if(_btDevice.getType() == BluetoothDevice.DEVICE_TYPE_LE)
			{
				
			}
		}
		else
		{
			try
			{
				_btSocket = _btDevice.createRfcommSocketToServiceRecord(SPP_UUID);
				_btSocket.connect();
				_InStream = _btSocket.getInputStream();
				_OutStream = _btSocket.getOutputStream();
				_isOpen = true;
			}
			catch (IOException e)
			{
				Log.d(_logTag, "Open Failed");    				
				close();
			}
		}

        return _isOpen;
	}
	
	public synchronized void Close()
	{
		Log.d(_logTag, "Close");
		
		close();
	}
	
	public boolean IsOpen()
	{
		return _isOpen;
	}
	
	public synchronized void Write(byte[] data)
	{
		try
		{
			_OutStream.write(data);
		}
		catch (Exception e)
		{
			Log.d(_logTag, "Write Error");
			close();
			UnityPlayer.UnitySendMessage(_unityObject, _unityMethodErrorClose, "Write Error");
		}
	}
	
	public int Avaliable()
	{
		try
		{
			return _InStream.available();
		}
		catch (Exception e)
		{
			Log.d(_logTag, "Write Error");
			close();
			UnityPlayer.UnitySendMessage(_unityObject, _unityMethodErrorClose, "Avaliable Error");
		}
		
		return 0;
	}
	
	public synchronized byte[] Read()
	{		
		try
		{
			byte[] data = new byte[_InStream.available()];
			_InStream.read(data);
			return data;
		}
		catch (Exception e)
		{
			Log.d(_logTag, "Read Error");
			close();
			UnityPlayer.UnitySendMessage(_unityObject, _unityMethodErrorClose, "Read Error");
		}
		
		return null;
	}
	
	
	
	private void close()
	{
		_isOpen = false;
		
		try
		{
			_btSocket.close();
		}
		catch(Exception e)
		{	
		}
		
		_btDevice = null;
		_btSocket = null;
		_InStream = null;
		_OutStream = null;
	}
	
	private static boolean deviceFilter(String address)
	{
		if (address.startsWith("00:19:01") || address.startsWith("B8:63:BC") || address.startsWith("B2:10:FF"))
		{
			return true;
		}
		else
		{
			int mac1 = Integer.parseInt(Character.toString(address.charAt(0)), 16);
			int mac2 = Integer.parseInt(Character.toString(address.charAt(1)), 16);
			int macMsb = (mac1 << 4) | mac2;

			return macMsb < 0xC0;
		}
	}
	
	private static IntentFilter makeGattUpdateIntentFilter()
	{
		final IntentFilter intentFilter = new IntentFilter();
		intentFilter.addAction(UartService.ACTION_GATT_CONNECTED);
		intentFilter.addAction(UartService.ACTION_GATT_DISCONNECTED);
		intentFilter.addAction(UartService.ACTION_GATT_SERVICES_DISCOVERED);
		intentFilter.addAction(UartService.ACTION_DATA_AVAILABLE);
		intentFilter.addAction(UartService.DEVICE_DOES_NOT_SUPPORT_UART);
		return intentFilter;
	}
	
	private ServiceConnection _serviceConnection = new ServiceConnection()
	{
		public void onServiceConnected(ComponentName className, IBinder rawBinder)
		{
			_bleService = ((UartService.LocalBinder) rawBinder).getService();
			Log.w(_logTag, "BLE Service connected");

			if (!_bleService.initialize())
				Log.w(_logTag, "BLE Service failed to initialize");
			else
				_bleService.connect(_btDevice.getAddress());
		}

		public void onServiceDisconnected(ComponentName classname)
		{
			Log.w(_logTag, "BLE Service disconnected");
			_bleService = null;
		}
	};

	private final BroadcastReceiver _serviceReceiver = new BroadcastReceiver()
	{
		public void onReceive(Context context, Intent intent)
		{
			String action = intent.getAction();

			if (action.equals(UartService.ACTION_GATT_CONNECTED))
			{
			}

			if (action.equals(UartService.ACTION_GATT_DISCONNECTED))
			{
				motion.handler.post(new Runnable()
				{
					public void run()
					{
						Log.w(TAG, "ACTION_GATT_DISCONNECTED");
						mService.close();
					}
				});
			}

			if (action.equals(UartService.ACTION_GATT_SERVICES_DISCOVERED))
			{
				// Log.w(TAG, "ACTION_GATT_SERVICES_DISCOVERED");
				mService.enableTXNotification(address);
			}

			if (action.equals(UartService.ACTION_DATA_AVAILABLE))
			{
				// Log.w(TAG, "ACTION_DATA_AVAILABLE");
			}

			if (action.equals(UartService.DEVICE_DOES_NOT_SUPPORT_UART))
			{
				Log.w(TAG, "DEVICE_DOES_NOT_SUPPORT_UART");
				// mService.disconnect(address);
			}
		}
	};
}
