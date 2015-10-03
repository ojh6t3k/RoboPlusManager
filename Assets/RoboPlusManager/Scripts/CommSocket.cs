using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Threading;
#if UNITY_STANDALONE
using System.IO.Ports;
#endif

[Serializable]
public class CommDevice
{
    public enum Type
    {
        Serial,
        BT,
        BLE
    }

    public string name;
    public Type type;
    public string address;
    public List<string> args = new List<string>();

    public CommDevice()
    {

    }

    public CommDevice(CommDevice device)
    {
        name = device.name;
        type = device.type;
        address = device.address;
        for (int i = 0; i < device.args.Count; i++)
            args.Add(device.args[i]);
    }

    public string displayName
    {
        get
        {
            return string.Format("{0}/{1}", type.ToString(), name);
        }
    }

    public bool Equals(CommDevice device)
    {
        if (!name.Equals(device.name))
            return false;

        if (type != device.type)
            return false;

        if (!address.Equals(device.address))
            return false;

        if (args.Count != device.args.Count)
            return false;

        for (int i = 0; i < args.Count; i++)
        {
            if (!args[i].Equals(device.args[i]))
                return false;
        }

        return true;
    }
}

public class CommSocket : MonoBehaviour
{
    #region Declares
    [SerializeField]
    public List<CommDevice> foundDevices = new List<CommDevice>();
    [SerializeField]
    public CommDevice device;
    public float searchTimeout = 5f;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnOpenFailed;
    public UnityEvent OnErrorClosed;
    public UnityEvent OnFoundDevice;
    public UnityEvent OnSearchCompleted;

    private CommDevice _device;
    private Thread _openThread;
    private Thread _searchThread;
    private bool _threadOnOpen = false;
    private bool _threadOnClose = false;
    private bool _threadOnOpenFailed = false;
    private bool _threadOnErrorClosed = false;
    private bool _threadOnFoundDevice = false;
    private bool _threadOnSearchCompleted = false;   

#if UNITY_STANDALONE
    private SerialPort _serialPort;
#elif UNITY_ANDROID
    private AndroidJavaObject _androidBluetooth = null;
    private float _btSearchTimeout = 0f;
    private float _bleSearchTimeout = 0f;
    private bool _btSearchContinue = false;
#endif
    #endregion

    #region MonoBehaviour
    void Awake()
    {
#if UNITY_STANDALONE
        _serialPort = new SerialPort();
        _serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
        _serialPort.RtsEnable = true;
        _serialPort.DataBits = 8;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
        _serialPort.WriteTimeout = 1000;
#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.smartmaker.android.CommBluetooth");
            _androidBluetooth = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
            if (_androidBluetooth.Call<bool>("Initialize", activityContext, gameObject.name) == true)
            {
                _androidBluetooth.Call("SetUnityMethodOpenSuccess", "AndroidMessageOpenSuccess");
                _androidBluetooth.Call("SetUnityMethodOpenFailed", "AndroidMessageOpenFailed");
                _androidBluetooth.Call("SetUnityMethodErrorClose", "AndroidMessageErrorClose");
                _androidBluetooth.Call("SetUnityMethodFoundDevice", "AndroidMessageFoundDevice");
            }
            else
                _androidBluetooth = null;
        }
        catch(Exception)
        {
            _androidBluetooth = null;
        }

        if(_androidBluetooth == null)
            Debug.Log("AndroidPlugin Failed!");
#endif
    }

    void Start ()
    {
	
	}
	
	void Update ()
    {
        // For threading event
        if(_threadOnOpen)
        {
            OnOpen.Invoke();
            _threadOnOpen = false;
        }
        if (_threadOnClose)
        {
            OnClose.Invoke();
            _threadOnClose = false;
        }
        if (_threadOnOpenFailed)
        {
            OnOpenFailed.Invoke();
            _threadOnOpenFailed = false;
        }
        if (_threadOnErrorClosed)
        {
            OnErrorClosed.Invoke();
            _threadOnErrorClosed = false;
        }
        if (_threadOnFoundDevice)
        {
            OnFoundDevice.Invoke();
            _threadOnFoundDevice = false;
        }
        if (_threadOnSearchCompleted)
        {
            OnSearchCompleted.Invoke();
            _threadOnSearchCompleted = false;
        }

#if UNITY_ANDROID
        if (_bleSearchTimeout > 0f)
        {
            _bleSearchTimeout -= Time.deltaTime;
            if (_bleSearchTimeout <= 0f)
            {
                if (_androidBluetooth != null)
                {
                    _androidBluetooth.Call("StopSearchBLE");
                    if(_btSearchContinue)
                    {
                        _androidBluetooth.Call("StartSearchBT");
                        _btSearchTimeout = searchTimeout * 0.7f;
                        Debug.Log(string.Format("BT Search time: {0:f1}", _btSearchTimeout));
                        _btSearchContinue = false;
                    }
                    else
                        _threadOnSearchCompleted = true;
                }
            }
        }

        if (_btSearchTimeout > 0f)
        {
            _btSearchTimeout -= Time.deltaTime;
            if(_btSearchTimeout <= 0f)
            {
                if(_androidBluetooth != null)
                    _androidBluetooth.Call("StopSearchBT");

                _threadOnSearchCompleted = true;
            }
        }        
#endif
    }
    #endregion

    #region Public
    public void Open()
    {
        CancelSearch();

        if (isOpen)
        {
            if (_device.Equals(device))
                return;

            close();                
        }

        _device = new CommDevice(device);
        _openThread = new Thread(openThread);
        _openThread.Start();
    }

    public void Close()
    {
        if (!isOpen)
            return;

        close();
        OnClose.Invoke();
    }

    public bool isOpen
    {
        get
        {
            if (_device != null)
            {
                if (_device.type == CommDevice.Type.Serial)
                {
#if UNITY_STANDALONE
                        return _serialPort.IsOpen;
#endif
                }
                else if (_device.type == CommDevice.Type.BT)
                {
#if UNITY_ANDROID
                    if (_androidBluetooth != null)
                        return _androidBluetooth.Call<bool>("IsOpen");
#endif
                }
                else if (_device.type == CommDevice.Type.BLE)
                {
#if UNITY_ANDROID
                    if (_androidBluetooth != null)
                        return _androidBluetooth.Call<bool>("IsOpen");
#endif
                }
            }

            return false;
        }
    }

    public bool isSupportBLE
    {
        get
        {
#if UNITY_ANDROID
            if (_androidBluetooth != null)
                return _androidBluetooth.Call<bool>("IsSupportBLE");
#endif
            return false;
        }
    }

    public void Search(params CommDevice.Type[] types)
    {
        _searchThread = new Thread(searchThread);
        _searchThread.Start(types);
    }

    public void CancelSearch()
    {
        if (_searchThread != null)
        {
            if (_searchThread.IsAlive)
                _searchThread.Abort();
        }

#if UNITY_ANDROID
        if(_bleSearchTimeout > 0f)
        {
            if (_androidBluetooth != null)
                _androidBluetooth.Call("StopSearchBLE");
            OnSearchCompleted.Invoke();
            _bleSearchTimeout = 0f;
        }

        if (_btSearchTimeout > 0f)
        {
            if (_androidBluetooth != null)
                _androidBluetooth.Call("StopSearchBT");
            OnSearchCompleted.Invoke();
            _btSearchTimeout = 0f;
        }
#endif
    }

    public void Write(byte[] data)
    {
        if (data == null)
            return;
        if (data.Length == 0)
            return;

        if (_device != null)
        {
            if (_device.type == CommDevice.Type.Serial)
            {
#if UNITY_STANDALONE
                try
                {
                    _serialPort.Write(data, 0, data.Length);
                    return;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if (_device.type == CommDevice.Type.BT)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                {
                    _androidBluetooth.Call("Write", data);
                    return;
                }
#endif
            }
            else if (_device.type == CommDevice.Type.BLE)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                {
                    _androidBluetooth.Call("Write", data);
                    return;
                }
#endif
            }
        }

        close();
        OnErrorClosed.Invoke();
    }

    public byte[] Read()
    {
        if (_device != null)
        {
            if (_device.type == CommDevice.Type.Serial)
            {
#if UNITY_STANDALONE
                List<byte> bytes = new List<byte>();
                while (true)
                {
                    try
                    {
                        bytes.Add((byte)_serialPort.ReadByte());
                    }
                    catch (TimeoutException)
                    {
                        return bytes.ToArray();
                    }
                    catch (Exception)
                    {
                    }
                }
#endif
            }
            else if (_device.type == CommDevice.Type.BT)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                {
                    if(_androidBluetooth.Call<int>("Available") > 0)
                        return _androidBluetooth.Call<byte[]>("Read");
                }
#endif
            }
            else if (_device.type == CommDevice.Type.BLE)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                {
                    if (_androidBluetooth.Call<int>("Available") > 0)
                        return _androidBluetooth.Call<byte[]>("Read");
                }
#endif
            }
        }

        close();
        OnErrorClosed.Invoke();
        return null;
    }
#endregion

#region Private
    private void close()
    {
        if (_openThread != null)
        {
            if (_openThread.IsAlive)
                _openThread.Abort();
        }

        if(_device != null)
        {
            if(_device.type == CommDevice.Type.Serial)
            {
#if UNITY_STANDALONE
                _serialPort.Close();
#endif
            }
            else if(_device.type == CommDevice.Type.BT)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                    _androidBluetooth.Call("Close");
#endif
            }
            else if(_device.type == CommDevice.Type.BLE)
            {
#if UNITY_ANDROID
                if (_androidBluetooth != null)
                    _androidBluetooth.Call("Close");
#endif
            }
        }

        _device = null;
    }

#if UNITY_ANDROID
    private void AndroidMessageOpenSuccess(string message)
    {
        Debug.Log(message);
        _threadOnOpen = true;
    }

    private void AndroidMessageOpenFailed(string message)
    {
        Debug.Log(message);
        close();
        _threadOnOpenFailed = true;
    }

    private void AndroidMessageErrorClose(string message)
    {
        Debug.Log(message);
        close();
        _threadOnErrorClosed = true;
    }

    private void AndroidMessageFoundDevice(string message)
    {
        Debug.Log(message);

        string[] tokens = message.Split(new char[] { ',' });
        CommDevice foundDevice = new CommDevice();
        if(tokens[0].Equals("BT"))
            foundDevice.type = CommDevice.Type.BT;
        else if (tokens[0].Equals("BLE"))
            foundDevice.type = CommDevice.Type.BLE;
        foundDevice.name = tokens[1];
        foundDevice.address = tokens[2];

        for(int i=0; i<foundDevices.Count; i++)
        {
            if (foundDevices[i].Equals(foundDevice))
                return;
        }

        foundDevices.Add(foundDevice);
        _threadOnFoundDevice = true;
    }
#endif

    private void openThread()
    {
#if UNITY_ANDROID
        AndroidJNI.AttachCurrentThread();
#endif
        bool openTry = false;

        if (_device.type == CommDevice.Type.Serial)
        {
#if UNITY_STANDALONE
            _serialPort.PortName = device.address;        
            try
            {
                _serialPort.BaudRate = int.Parse(device.args[0]);
                _serialPort.Open();
                _threadOnOpen = true;
                openTry = true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }
        else if (device.type == CommDevice.Type.BT)
        {
#if UNITY_ANDROID
            if (_androidBluetooth != null)
            {
                _androidBluetooth.Call("Open", device.address);
                openTry = true;
            }
#endif
        }
        else if (device.type == CommDevice.Type.BLE)
        {
#if UNITY_ANDROID
            if (_androidBluetooth != null)
            {
                _androidBluetooth.Call("Open", device.address);
                openTry = true;
            }
#endif
        }
        else
            Debug.Log("Not supported device type");

        if (!openTry)
            _threadOnOpenFailed = true;

#if UNITY_ANDROID
        AndroidJNI.DetachCurrentThread();
#endif
        _openThread.Abort();
        return;
    }

    private void searchThread(object parameter)
    {
#if UNITY_ANDROID
        AndroidJNI.AttachCurrentThread();
#endif

        CommDevice.Type[] types = (CommDevice.Type[])parameter;

        foundDevices.Clear();

        bool serial = false;
        bool bt = false;
        bool ble = false;
        if (types.Length == 0)
        {
            serial = true;
            bt = true;
            ble = true;
        }
        else
        {
            foreach (CommDevice.Type t in types)
            {
                if (t == CommDevice.Type.Serial)
                    serial = true;
                else if (t == CommDevice.Type.BT)
                    bt = true;
                else if (t == CommDevice.Type.BLE)
                    ble = true;
            }
        }

        ble &= isSupportBLE;
        bool searchStart = false;

        if (serial)
        {
#if UNITY_STANDALONE
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            string[] devInfos = SerialPort.GetPortNames();
            for (int i = 0; i < devInfos.Length; i++)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = devInfos[i];
                foundDevice.type = CommDevice.Type.Serial;
                foundDevice.address = "//./" + devInfos[i];
                foundDevice.args.Add("57600");
                foundDevices.Add(foundDevice);
            }
            if (devInfos.Length > 0)
                _threadOnFoundDevice = true;
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            string prefix = "/dev/";
            string[] devInfos = Directory.GetFiles(prefix, "*.*");
            for(int i=0; i<ports.Length; i++)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = devInfos[i].Substring(prefix.Length));            
                foundDevice.address = devInfos[i];
                if(devInfos[i].StartsWith ("/dev/cu.usb") | devInfos[i].StartsWith ("/dev/tty.usb"))
                {
                    foundDevice.type = CommDevice.Type.Serial;
                    foundDevice.args.Add("57600");
                }            
                foundDevices.Add(foundDevice);
            }
            if (devInfos.Length > 0)
                _threadOnFoundDevice = true;
#endif
#endif
        }

        if (bt)
        {
#if UNITY_ANDROID
            if (_androidBluetooth != null)
            {
                string[] devInfos = _androidBluetooth.Call<string[]>("GetBondedDevices");
                for(int i=0; i<devInfos.Length; i++)
                {
                    string[] tokens = devInfos[i].Split(new char[] { ',' });
                    CommDevice foundDevice = new CommDevice();
                    foundDevice.type = CommDevice.Type.BT;
                    foundDevice.name = tokens[0];
                    foundDevice.address = tokens[1];
                    foundDevices.Add(foundDevice);
                }
                if (devInfos.Length > 0)
                    _threadOnFoundDevice = true;

                if(!ble)
                {
                    _androidBluetooth.Call("StartSearchBT");
                    _btSearchTimeout = searchTimeout;
                    Debug.Log(string.Format("BT Search time: {0:f1}", _btSearchTimeout));
                    searchStart = true;
                }                
            }
#endif
        }

        if (ble)
        {
#if UNITY_ANDROID
            if (_androidBluetooth != null)
            {
                _androidBluetooth.Call("StartSearchBLE");
                if (bt)
                {
                    _bleSearchTimeout = searchTimeout * 0.3f;                    
                    _btSearchContinue = true;
                }
                else
                    _bleSearchTimeout = searchTimeout;

                Debug.Log(string.Format("BLE Search time: {0:f1}", _bleSearchTimeout));
                searchStart = true;
            }
#endif
        }

        if(!searchStart)
            _threadOnSearchCompleted = true;

#if UNITY_ANDROID
        AndroidJNI.DetachCurrentThread();
#endif
        _searchThread.Abort();
        return;
    }
#endregion
}
