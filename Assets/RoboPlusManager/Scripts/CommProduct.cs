using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


public class CommProduct : MonoBehaviour
{
    public ProductManager productManager;
    public CommProtocol commProtocol;
    public ProductInfo productInfo;

    public UnityEvent OnConnected;
    public UnityEvent OnConnectionFailed;
    public UnityEvent OnDisconnected;

    private byte _id = 0xff;
    private ushort _model = 0xffff;
    private byte _version = 0xff;
    private bool _connected = false;
    private static readonly int RETRY_NUM = 3;
    private int _retryNum = 0;
    private List<ControlItemInfo> _writeItems = new List<ControlItemInfo>();

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public bool connected
    {
        get
        {
            return _connected;
        }
    }

    public int id
    {
        get
        {
            return _id;
        }
    }

    public int version
    {
        get
        {
            return _version;
        }
    }

    public int model
    {
        get
        {
            return _model;
        }
    }

    public void Clear()
    {
        Disconnect();
        _id = 0xff;
        productInfo = null;
    }

    public void Connect()
    {
        Connect(_id);
    }

    public void Connect(int id)
    {
        if (_connected)
            return;

        if(commProtocol == null)
        {
            OnConnectionFailed.Invoke();
            return;
        }

        if(id > CommProtocol.MAX_ID)
        {
            OnConnectionFailed.Invoke();
            return;
        }

        _id = (byte)id;

        commProtocol.OnError.AddListener(OnProtocolError);
        commProtocol.OnStatus.AddListener(OnProtocolStatus);
        commProtocol.OnPing.AddListener(OnProtocolPing);
        commProtocol.OnProductInfo.AddListener(OnProtocolProductInfo);
        commProtocol.OnRead.AddListener(OnProtocolRead);

        commProtocol.ReadProductInfo(_id);
    }

    public void Disconnect()
    {
        _writeItems.Clear();

        if (commProtocol != null)
        {
            commProtocol.OnError.RemoveListener(OnProtocolError);
            commProtocol.OnStatus.RemoveListener(OnProtocolStatus);
            commProtocol.OnPing.RemoveListener(OnProtocolPing);
            commProtocol.OnProductInfo.RemoveListener(OnProtocolProductInfo);
            commProtocol.OnRead.RemoveListener(OnProtocolRead);
        }

        if (!_connected)
            OnConnectionFailed.Invoke();
        else
        {
            _connected = false;
            OnDisconnected.Invoke();
        }        
    }

    public void Ping()
    {
        if (!_connected)
            return;

        commProtocol.Ping(_id);
    }

    public void PlayMelody(int index)
    {
        ControlUIInfo ui = productInfo.GetControlUIInfo("MusicUI");
        ControlItemInfo melodyIndex = ui.GetUIItem("MelodyIndex");
        melodyIndex.value = index;
        ControlItemInfo musicTime = ui.GetUIItem("MusicTime");
        musicTime.value = 255;

        _writeItems.Add(musicTime);
        _writeItems.Add(melodyIndex);

        OnProtocolStatus(_id, 0);
    }

    public void MoveServo(int pos)
    {
        ControlUIInfo ui = productInfo.GetControlUIInfo("AuxDeviceUI");
        ControlItemInfo servoMode = ui.GetUIItem("ServoMode1");
        servoMode.value = 1;
        ControlItemInfo servoPos = ui.GetUIItem("ServoPos1");
        servoPos.value = pos;
        ControlItemInfo servoSpeed = ui.GetUIItem("ServoSpeed1");
        servoSpeed.value = 1023;

        _writeItems.Add(servoMode);
        _writeItems.Add(servoPos);

        OnProtocolStatus(_id, 0);
    }

    private void OnProtocolError(byte id, CommProtocol.ERROR error)
    {
        if (id != _id)
            return;

        if(error == CommProtocol.ERROR.Timeout)
        {
            if (_retryNum == 0)
                Disconnect();
            else
            {
                _retryNum--;
                commProtocol.SendInstruction();
            }                
        }
    }

    private void OnProtocolStatus(byte id, byte error)
    {
        if (id != _id)
            return;

        if (_connected)
        {
            if(_writeItems.Count > 0)
            {
                if(_writeItems[0].bytes == 1)
                    commProtocol.Write(_id, (ushort)_writeItems[0].address, (byte)_writeItems[0].value);
                else if(_writeItems[0].bytes == 2)
                    commProtocol.Write(_id, (ushort)_writeItems[0].address, (ushort)_writeItems[0].value);

                _writeItems.RemoveAt(0);
            }
        }            
    }

    private void OnProtocolPing(byte id)
    {
        if (id != _id)
            return;
    }

    private void OnProtocolProductInfo(byte id, ushort model, byte ver)
    {
        if (id != _id)
            return;

        if(!_connected)
        {
            _model = model;
            _version = ver;
            if (productManager != null)
                productInfo = productManager.GetProductInfo(_model);

            _connected = true;
            OnConnected.Invoke();
        }        
    }

    private void OnProtocolRead(byte id, ushort address, byte[] parameters)
    {
        if (id != _id)
            return;
    }
}
