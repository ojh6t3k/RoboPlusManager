package com.smartmaker.android;

import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;


import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.ComponentName;
import android.content.Context;
import android.content.ServiceConnection;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.IBinder;
import android.util.Log;


public class CommBLE
{
	private static CommBLE _Instance = null;
	private static String _logTag = "CommBLE";
	private Context _context;
	private BluetoothAdapter _btAdapter;

	private boolean _isOpen = false;
	private String _unityObject;
	private String _unityMethodErrorClose;

	
	public static CommBLE GetInstance()
    {
		Log.d(_logTag, "GetInstance");
		
        if(_Instance == null)
            _Instance = new CommBLE();        
        
        return _Instance;
    }
	
	public boolean Initialize(Context context, String unityObject)
    {
		Log.d(_logTag, "Initialize");
		
		if(Build.VERSION.SDK_INT < 19
			|| !context.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE))
			return false; // Not supported BLE
		
		_btAdapter = BluetoothAdapter.getDefaultAdapter();
        if(_btAdapter == null)
        	return false;
		
        _context = context;
        _unityObject = unityObject;
        
        return true;
    }
	
	private ServiceConnection _serviceConnection = new ServiceConnection()
	{
		public void onServiceConnected(ComponentName className, IBinder rawBinder)
		{
			_bleService = ((UartService.LocalBinder) rawBinder).getService();
			Log.w(_logTag, "onServiceConnected : mService : " + _bleService);

			if (!_bleService.initialize())
			{
				Log.w(TAG, "!mService.initialize()");
				// finish(); @@
			}
			else
			{
				mService.connect(address);
			}
		}

		public void onServiceDisconnected(ComponentName classname)
		{
			Log.w(TAG, "onServiceDisconnected : mService : " + mService);
			mService = null;
		}
	};

	private final BroadcastReceiver mUARTStatusChangeReceiver = new BroadcastReceiver()
	{
		public void onReceive(Context context, Intent intent)
		{
			String action = intent.getAction();
			// final String address =
			// intent.getStringExtra(UartService.EXTRA_ADDRESS);

			if (action.equals(UartService.ACTION_GATT_CONNECTED))
			{
				// motion.handler.post(new Runnable()
				// {
				// public void run()
				// {
				// Log.d(TAG, "ACTION_GATT_CONNECTED " + address);
				// mService.connect(address);
				// }
				// });
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
