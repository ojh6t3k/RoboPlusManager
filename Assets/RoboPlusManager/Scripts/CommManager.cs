using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
#if (UNITY_STANDALONE || UNITY_EDITOR)
using System.IO.Ports;
#endif

[Serializable]
public class CommDevice
{
    public string name;
    public List<string> parameters = new List<string>();
}

public class CommManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public List<CommDevice> devices = new List<CommDevice>();
    [SerializeField]
    public CommDevice device;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnOpenFailed;
    public UnityEvent OnForceClosed;
    public UnityEvent OnFoundDevice;
    public UnityEvent OnSearchCompleted;

#if (UNITY_STANDALONE || UNITY_EDITOR)
    public int baudrate = 57600;

    private SerialPort _serialPort;
#endif
    #endregion

    #region MonoBehaviour
    void Awake()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR)
        _serialPort = new SerialPort();
        _serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
        _serialPort.RtsEnable = true;
        _serialPort.DataBits = 8;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
        _serialPort.WriteTimeout = 1000;
#endif
    }

    void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
    #endregion

    #region Public
    public void Open()
    {

    }

    public void Reopen()
    {

    }

    public void Close()
    {

    }

    public bool isOpen
    {
        get
        {
            return false;
        }
    }

    public void Search()
    {

    }

    public void Write(byte[] data)
    {

    }

    public byte[] Read()
    {
        return null;
    }
    #endregion

    #region Private
    #endregion
}
