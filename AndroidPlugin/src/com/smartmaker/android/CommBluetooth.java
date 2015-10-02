package com.smartmaker.android;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.Set;
import java.util.ArrayList;
import java.util.List;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothSocket;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
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


@SuppressLint("NewApi")
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
	private String _unityMethodOpenSuccess;
	private String _unityMethodOpenFailed;
	private String _unityMethodErrorClose;
	private String _unityMethodFoundDevice;
	private boolean _isErrorClose = false;
	private String _errorMessage;

	
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
		
		_isSupportBLE = (Build.VERSION.SDK_INT >= 21 && context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE));
        if(_isSupportBLE)
        	Log.d(_logTag, "Support BLE");
        else
        	Log.d(_logTag, "Not support BLE");
		
        if(_isSupportBLE)
        {
        	final BluetoothManager bluetoothManager = (BluetoothManager)context.getSystemService(Context.BLUETOOTH_SERVICE);
        	_btAdapter = bluetoothManager.getAdapter();
        }
        else
        	_btAdapter = BluetoothAdapter.getDefaultAdapter();
        if(_btAdapter == null)
        {
        	Log.d(_logTag, "Bluetooth Adapter Failed");
        	return false;
        }
		
        _context = context;
        _unityObject = unityObject;        
        _btDevice = null;
        _btSocket = null;
        _bleService = null;
        _InStream = null;
        _OutStream = null;
        
        final IntentFilter intentFilter = new IntentFilter();
		intentFilter.addAction(BluetoothDevice.ACTION_FOUND);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_CONNECTED);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECTED);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECT_REQUESTED);
        _context.registerReceiver(_bluetoothReceiver, intentFilter);
        _context.registerReceiver(_bluetoothReceiver, new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED));

        return true;
    }
	
	public void SetUnityMethodOpenSuccess(String unityMethod)
	{
		_unityMethodOpenSuccess = unityMethod;
	}
	
	public void SetUnityMethodOpenFailed(String unityMethod)
	{
		_unityMethodOpenFailed = unityMethod;
	}
	
	public void SetUnityMethodErrorClose(String unityMethod)
	{
		_unityMethodErrorClose = unityMethod;
	}
	
	public void SetUnityMethodFoundDevice(String unityMethod)
	{
		_unityMethodFoundDevice = unityMethod;
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
	
	public void StartSearchBT()
	{
		if(_btAdapter == null)
			return;
		
		_btAdapter.startDiscovery();
	}
	
	public void StopSearchBT()
	{
		if(_btAdapter == null)
			return;
		
		if (_btAdapter.isDiscovering())
			_btAdapter.cancelDiscovery();
	}
	
	public void StartSearchBLE()
	{
		if(_btAdapter == null || !_isSupportBLE)
			return;
		
		_btAdapter.getBluetoothLeScanner().startScan(_bleScanCallback);
	}
	
	public void StopSearchBLE()
	{
		if(_btAdapter == null || !_isSupportBLE)
			return;
		
		_btAdapter.getBluetoothLeScanner().stopScan(_bleScanCallback);
	}
	
	public boolean IsSearchDevice()
	{
		if(_btAdapter == null)
			return false;
		
		return _btAdapter.isDiscovering();
	}
	
	public synchronized void Open(String address)
	{
		Log.d(_logTag, "Open");
		
		if(_isOpen)
		{
			if(!_btDevice.getAddress().equals(address))
				close();
		}
		
		if(_btAdapter.isEnabled())
		{
			_btDevice = _btAdapter.getRemoteDevice(address);
			if(_btDevice != null)
			{
				if(_isSupportBLE)
				{
					if(_btDevice.getType() == BluetoothDevice.DEVICE_TYPE_CLASSIC)
						openBT();
					else if(_btDevice.getType() == BluetoothDevice.DEVICE_TYPE_LE)
						openBLE();
				}
				else
					openBT();
			}			
		}
		else
			_context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
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
		if(_isOpen)
		{
			try
			{
				_OutStream.write(data);
			}
			catch (Exception e)
			{
				Log.d(_logTag, "Write Error");
				_isErrorClose = true;
				_errorMessage = "Write Error";
				close();
			}
		}
	}
	
	public int Avaliable()
	{
		if(_isOpen)
		{
			try
			{
				return _InStream.available();
			}
			catch (Exception e)
			{
				Log.d(_logTag, "Avaliable Error");
				_isErrorClose = true;
				_errorMessage = "Avaliable Error";
				close();
			}
		}
		
		return 0;
	}
	
	public synchronized byte[] Read()
	{
		if(_isOpen)
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
				_isErrorClose = true;
				_errorMessage = "Read Error";
				close();
			}
		}
		
		return null;
	}
	
	
	private void openBT()
	{
		try
		{
			_btSocket = _btDevice.createRfcommSocketToServiceRecord(SPP_UUID);
			_btSocket.connect();
			_InStream = _btSocket.getInputStream();
			_OutStream = _btSocket.getOutputStream();
		}
		catch (IOException e)
		{
			Log.d(_logTag, "BT Open Failed");
			close();
			UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "BT Open Failed");
		}
	}
	
	private void openBLE()
	{
		Intent bindIntent = new Intent(_context, UartService.class);
    	_context.bindService(bindIntent, _serviceConnection, Context.BIND_AUTO_CREATE);
    	final IntentFilter intentFilter = new IntentFilter();
		intentFilter.addAction(UartService.ACTION_GATT_CONNECTED);
		intentFilter.addAction(UartService.ACTION_GATT_DISCONNECTED);
		intentFilter.addAction(UartService.ACTION_GATT_SERVICES_DISCOVERED);
		intentFilter.addAction(UartService.ACTION_DATA_AVAILABLE);
		intentFilter.addAction(UartService.DEVICE_DOES_NOT_SUPPORT_UART);
    	LocalBroadcastManager.getInstance(_context).registerReceiver(_serviceReceiver, intentFilter);
	}
	
	private void close()
	{
		if(!_isOpen)
			return;
		
		_isOpen = false;
		
		if (_bleService != null)
		{
			_bleService.disconnect();
			_bleService.close();
			
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
			
			_bleService = null;
		}
		
		if(_btSocket != null)
		{
			try
			{
				_btSocket.close();
			}
			catch(Exception e)
			{	
			}
			_btSocket = null;
		}
		
		_btDevice = null;
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
	
	private ServiceConnection _serviceConnection = new ServiceConnection()
	{
		public void onServiceConnected(ComponentName className, IBinder rawBinder)
		{
			_bleService = ((UartService.LocalBinder) rawBinder).getService();
			Log.d(_logTag, "BLE Service connected");

			if (!_bleService.initialize())
			{
				Log.d(_logTag, "BLE Service failed to initialize");
				close();
				UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "BLE Service failed to initialize");
			}
			else
			{
				if(!_bleService.connect(_btDevice.getAddress()))
				{
					Log.d(_logTag, "BLE Connect Failed");
					close();
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "BLE Connect Failed");
				}
			}				
		}

		public void onServiceDisconnected(ComponentName classname)
		{
			Log.d(_logTag, "BLE Service disconnected");
			close();
		}
	};

	private final BroadcastReceiver _serviceReceiver = new BroadcastReceiver()
	{
		public void onReceive(Context context, Intent intent)
		{
			String action = intent.getAction();

			if (action.equals(UartService.ACTION_GATT_CONNECTED))
			{
				Log.d(_logTag, "ACTION_GATT_CONNECTED");
			}
			else if (action.equals(UartService.ACTION_GATT_DISCONNECTED))
			{
				Log.d(_logTag, "ACTION_GATT_DISCONNECTED");
				close();
			}
			else if (action.equals(UartService.ACTION_GATT_SERVICES_DISCOVERED))
			{
				Log.d(_logTag, "ACTION_GATT_SERVICES_DISCOVERED");
				_bleService.enableTXNotification(_btDevice.getAddress());
				_isOpen = true;
				UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenSuccess, "BLE Connected");
			}
			else if (action.equals(UartService.ACTION_DATA_AVAILABLE))
			{
			}
			else if (action.equals(UartService.DEVICE_DOES_NOT_SUPPORT_UART))
			{
				Log.d(_logTag, "DEVICE_DOES_NOT_SUPPORT_UART");
				close();
				UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "BLE dose not support UART");
			}
		}
	};
	
	private final BroadcastReceiver _bluetoothReceiver = new BroadcastReceiver()
	{
		@Override
		public void onReceive(Context context, Intent intent)
		{
			String action = intent.getAction();

			if (BluetoothDevice.ACTION_FOUND.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "Device Found");
				
				BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
				if(deviceFilter(device.getAddress()))
				{
					if(_isSupportBLE)
					{
						if(device.getType() == BluetoothDevice.DEVICE_TYPE_CLASSIC)
							UnityPlayer.UnitySendMessage(_unityObject, _unityMethodFoundDevice, String.format("BT,%s,%s", device.getName(), device.getAddress()));
						else if(device.getType() == BluetoothDevice.DEVICE_TYPE_LE)
							UnityPlayer.UnitySendMessage(_unityObject, _unityMethodFoundDevice, String.format("BLE,%s,%s", device.getName(), device.getAddress()));
					}
					else
						UnityPlayer.UnitySendMessage(_unityObject, _unityMethodFoundDevice, String.format("BT,%s,%s", device.getName(), device.getAddress()));
				}				
			}
			else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "ACTION_DISCOVERY_FINISHED");
			}
			else if (BluetoothDevice.ACTION_ACL_CONNECTED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "Device Connected!!");
				
				if(_isSupportBLE)
				{
					if(_btDevice.getType() == BluetoothDevice.DEVICE_TYPE_CLASSIC)
					{
						_isOpen = true;
						UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenSuccess, "BT Connected");
					}
				}
				else
				{
					_isOpen = true;
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenSuccess, "BT Connected");
				}				
			}
			else if (BluetoothDevice.ACTION_ACL_DISCONNECTED.equalsIgnoreCase(action) || BluetoothDevice.ACTION_ACL_DISCONNECT_REQUESTED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "Device Disconnected!!");
				if(_isOpen)
				{
					close();
					_isErrorClose = true;
					_errorMessage = "Device Disconnected";
				}
				
				if(_isErrorClose)
				{
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodErrorClose, _errorMessage);
					_isErrorClose = false;
					_errorMessage = "";
				}
			}
			else
			{
				Log.d(_logTag, "UNKNOWN ACTION : " + action);
			}
		}
	};
	
	private ScanCallback _bleScanCallback = new ScanCallback()
	{
        @Override
        public void onScanResult(int callbackType, ScanResult result)
        {
            BluetoothDevice device = result.getDevice();
            if (device != null)
            {
            	Log.d(_logTag, "BLE Found: " + device.getName());
            	
//            	if(deviceFilter(device.getAddress()))
            		UnityPlayer.UnitySendMessage(_unityObject, _unityMethodFoundDevice, String.format("BLE,%s,%s", device.getName(), device.getAddress()));
            }        	
        }
    };
}
