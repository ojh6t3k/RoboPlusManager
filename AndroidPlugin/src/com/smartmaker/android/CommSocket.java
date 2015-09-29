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
import android.content.Context;
import android.util.Log;



public class CommSocket
{
	private static CommSocket _Instance = null;
	private Context _context;
	private BluetoothAdapter _btAdapter;
	private static final UUID _UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private BluetoothDevice _btDevice;
	private BluetoothSocket _btSocket;
	private InputStream _InStream;
	private OutputStream _OutStream;
	private boolean _isOpen = false;

	
	public static CommSocket GetInstance()
    {
		Log.d("CommSocket", "GetInstance");
		
        if(_Instance == null)
            _Instance = new CommSocket();        
        
        return _Instance;
    }
	
	public void SetContext(Context context)
    {
		Log.d("CommSocket", "SetContext");
		
        _context = context;
        _btAdapter = BluetoothAdapter.getDefaultAdapter();        
    }
	
	public synchronized String[] GetBondedBluetooth()
    {
		Log.d("CommSocket", "GetBondedBluetooth");
		
        List<String> btDevices = new ArrayList<String>();

        try
        {
            Set<BluetoothDevice> bondedDevices = _btAdapter.getBondedDevices();
            if (bondedDevices.size() > 0)
            {
                for (BluetoothDevice bd : bondedDevices)
                	btDevices.add(String.format("%s,%s", bd.getName(), bd.getAddress()));
            }
        }
        catch (Exception e)
        {
        }
        
        return btDevices.toArray(new String[btDevices.size()]);
    }
	
	public synchronized boolean Open(String address)
	{
		Log.d("CommSocket", "Open");
		
		if(_btAdapter == null)
			return false;
		
		if(!_btAdapter.isEnabled())
			return false;
		
		if(_isOpen)
			return false;
		
		_isOpen = false;

		Set<BluetoothDevice> bondedDevices = _btAdapter.getBondedDevices();
        for (BluetoothDevice bd : bondedDevices)
        {
            if (bd.getAddress().equalsIgnoreCase(address))
            {
            	_btDevice = _btAdapter.getRemoteDevice(address);
            	try
    			{
    				_btSocket = _btDevice.createRfcommSocketToServiceRecord(_UUID);
    				_btSocket.connect();
    				_InStream = _btSocket.getInputStream();
    				_OutStream = _btSocket.getOutputStream();
    				_isOpen = true;
    			}
    			catch (IOException e)
    			{
    				InternalClose();
    			}
            }
        }

        return _isOpen;
	}
	
	public synchronized void Close()
	{
		Log.d("CommSocket", "Close");
		
		InternalClose();
	}
	
	public synchronized boolean IsOpen()
	{
		return _isOpen;
	}
	
	public synchronized boolean Write(byte[] data)
	{
		try
		{
			_OutStream.write(data);
		}
		catch (Exception e)
		{
			InternalClose();
		}
		
		return _isOpen;
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
			InternalClose();
		}
		
		return null;
	}
	
	private void InternalClose()
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
}
