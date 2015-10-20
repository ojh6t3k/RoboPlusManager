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
    public UnityEvent OnLostConnection;
    public UnityEvent OnNoResponse;

    private byte _id = 0xff;
    private ushort _model = 0xffff;
    private byte _version = 0xff;
    private bool _connected = false;
    private bool _run = false;
    private List<ControlItemInfo> _writeItems = new List<ControlItemInfo>();
    private List<ControlItemInfo> _readItems = new List<ControlItemInfo>();

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
        set
        {
            _id = (byte)value;
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

    public void Connect()
    {
        Connect(_id);
    }

    public void Connect(int id)
    {
        if (_connected)
            return;

        if(commProtocol == null || id > CommProtocol.MAX_ID)
        {
            Debug.Log(string.Format("ID:{0:d} Connection Failed", id));
            OnConnectionFailed.Invoke();
            return;
        }

        _id = (byte)id;
        commProtocol.OnResult.AddListener(OnProtocolResult);
        commProtocol.Ping(_id);
    }

    public void Disconnect()
    {
        disconnect();

        Debug.Log(string.Format("ID:{0:d} Disconnect", _id));
        _id = 0xff;
        productInfo = null;
        OnDisconnected.Invoke();
    }

    private void disconnect()
    {
        Stop();
        _connected = false;
        _writeItems.Clear();
        productInfo = null;

        if (commProtocol != null)
            commProtocol.OnResult.RemoveListener(OnProtocolResult);
    }

    public void Run()
    {
        if (!_connected)
            return;

        _run = true;
        commProtocol.Ping(_id);
    }

    public void Stop()
    {
        _run = false;
    }

    public void ClearItem()
    {
        _writeItems.Clear();
        _readItems.Clear();
    }

    public void SetWriteItem(ControlItemInfo item)
    {
        _writeItems.Clear();
        _writeItems.Add(item);
    }

    public void AddWriteItem(ControlItemInfo item)
    {
        _writeItems.Add(item);
    }

    public void AddReadItem(ControlItemInfo item)
    {
        _readItems.Add(item);
    }

    public void RemoveReadItem(ControlItemInfo item)
    {
        _readItems.Remove(item);
    }

    private void OnProtocolResult(CommProtocol.Result result)
    {
        if (result.id != _id)
            return;

        if(result.txFail)
        {
            if (_connected)
            {
                disconnect();
                Debug.Log(string.Format("ID:{0:d} Lost Connection", _id));
                OnLostConnection.Invoke();
            }
            else
            {
                Debug.Log(string.Format("ID:{0:d} Connection Failed", _id));
                OnConnectionFailed.Invoke();
            }
        }

        if (_connected)
        {
            if (result.rxFail || result.timeout)
            {
                Debug.Log(string.Format("ID:{0:d} No Response", _id));
                OnNoResponse.Invoke();
            }
            else
            {
                if (result.query == CommProtocol.QUERY.Read)
                {
                    foreach (ControlItemInfo item in _readItems)
                    {
                        int index = item.address - result.address;
                        if((index + item.bytes - 1) < result.parameters.Count)
                        {
                            if (item.bytes == 1)
                                item.value = result.parameters[index];
                            else if (item.bytes == 2)
                                item.value = CommProtocol.Bytes2Word(result.parameters[index], result.parameters[index + 1]);
                        }                        
                    }
                }

                if (_run)
                {
                    if(_writeItems.Count > 0)
                    {
                        if (_writeItems[0].bytes == 1)
                            commProtocol.Write(_id, (ushort)_writeItems[0].address, (byte)_writeItems[0].value);
                        else if (_writeItems[0].bytes == 2)
                            commProtocol.Write(_id, (ushort)_writeItems[0].address, (ushort)_writeItems[0].value);

                        _writeItems.RemoveAt(0);
                    }
                    else if(_readItems.Count > 0)
                    {
                        int minAddr = 0xffff;
                        int maxAddr = -1;
                        foreach(ControlItemInfo item in _readItems)
                        {
                            minAddr = Mathf.Min(minAddr, item.address);
                            maxAddr = Mathf.Max(maxAddr, item.address + item.bytes - 1);
                        }
                        int bytes = maxAddr - minAddr + 1;
                        if (bytes > 0)
                            commProtocol.Read(_id, (ushort)minAddr, (ushort)bytes);
                    }
                    else
                        commProtocol.Ping(_id);
                }
            }
        }
        else
        {
            if (result.rxFail || result.timeout)
            {
                Debug.Log(string.Format("ID:{0:d} Connection Failed", _id));
                OnConnectionFailed.Invoke();
            }
            else
            {
                if(result.query == CommProtocol.QUERY.Ping)
                    commProtocol.AskWho(_id);
                else if(result.query == CommProtocol.QUERY.AskWho)
                {
                    _model = CommProtocol.Bytes2Word(result.parameters[0], result.parameters[1]);
                    _version = result.parameters[2];
                    bool tryUpdate = false;
                    if (productManager != null)
                    {
                        productInfo = productManager.GetProductInfo(_model);
                        if(productInfo != null)
                        {
                            int minAddr = 0xffff;
                            int maxAddr = -1;
                            foreach (ControlUIInfo uiInfo in productInfo.uiList)
                            {
                                foreach(ControlItemInfo uiItem in uiInfo.uiItems)
                                {
                                    minAddr = Mathf.Min(minAddr, uiItem.address);
                                    maxAddr = Mathf.Max(maxAddr, uiItem.address + uiItem.bytes - 1);
                                }
                            }
                            int bytes = maxAddr - minAddr + 1;
                            if (bytes > 0)
                            {
                                tryUpdate = true;
                                commProtocol.Read(_id, (ushort)minAddr, (ushort)bytes);
                            }
                        }                        
                    }
                                        
                    if(!tryUpdate)
                    {
                        _connected = true;
                        Debug.Log(string.Format("ID:{0:d} Connected", _id));
                        OnConnected.Invoke();
                    }       
                }
                else if(result.query == CommProtocol.QUERY.Read)
                {
                    foreach (ControlUIInfo uiInfo in productInfo.uiList)
                    {
                        foreach (ControlItemInfo uiItem in uiInfo.uiItems)
                        {
                            int index = uiItem.address - result.address;
                            if (uiItem.bytes == 1)
                                uiItem.value = result.parameters[index];
                            else if (uiItem.bytes == 2)
                                uiItem.value = CommProtocol.Bytes2Word(result.parameters[index], result.parameters[index + 1]);
                        }
                    }

                    _connected = true;
                    Debug.Log(string.Format("ID:{0:d} Connected", _id));
                    OnConnected.Invoke();
                }
            }            
        }
    }
}
