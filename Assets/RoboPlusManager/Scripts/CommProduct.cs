using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


public class CommProduct : MonoBehaviour
{
    public ProductInfo productInfo;

    [Serializable]
    public class ProductEvent : UnityEvent<CommProduct> { }

    public ProductEvent OnConnected;
    public ProductEvent OnConnectionFailed;
    public ProductEvent OnDisconnected;
    public ProductEvent OnLostConnection;
    public ProductEvent OnNoResponse;
    public UnityEvent OnReadUpdate;
    public UnityEvent OnWriteCompleted;

    private byte _id = 0xff;
    private ushort _model = 0xffff;
    private byte _version = 0xff;
    private bool _connected = false;
    private bool _run = false;
    private List<ControlItemInfo> _writeItems = new List<ControlItemInfo>();
    private List<ControlItemInfo> _readItems = new List<ControlItemInfo>();

    void Awake()
    {
        if (CommProtocol.instance != null)
            CommProtocol.instance.OnResult.AddListener(OnProtocolResult);
    }

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

    public void Connect(int id)
    {
        if (_connected)
            return;

        if (CommProtocol.instance == null)
        {
      //      Debug.Log("Can not find CommProtocol!");
            OnConnectionFailed.Invoke(this);
            return;
        }

        if (id > CommProtocol.MAX_ID)
        {
       //     Debug.Log(string.Format("{0:d} is invalid ID!", id));
            OnConnectionFailed.Invoke(this);
            return;
        }

        _id = (byte)id;
        CommProtocol.instance.Ping(_id);
    }

    public void Disconnect()
    {
        disconnect();

      //  Debug.Log(string.Format("ID:{0:d} Disconnect", _id));
        _id = 0xff;
        productInfo = null;
        OnDisconnected.Invoke(this);
    }

    private void disconnect()
    {
        Stop();
        _connected = false;
        _writeItems.Clear();
        productInfo = null;
    }

    public void Run()
    {
        if (!_connected)
            return;

        _run = true;
        CommProtocol.instance.Ping(_id);
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

        if (result.txFail)
        {
            if (_connected)
            {
                disconnect();
         //       Debug.Log(string.Format("ID:{0:d} Lost Connection", _id));
                OnLostConnection.Invoke(this);
            }
            else
            {
         //       Debug.Log(string.Format("ID:{0:d} Connection Failed", _id));
                OnConnectionFailed.Invoke(this);
            }
        }
        else
        {
            if (result.rxFail || result.timeout)
            {
                if (_connected)
                {
           //         Debug.Log(string.Format("ID:{0:d} No Response", _id));
                    OnNoResponse.Invoke(this);
                }
                else
                {
           //         Debug.Log(string.Format("ID:{0:d} Connection Failed", _id));
                    OnConnectionFailed.Invoke(this);
                }
            }
            else
            {
                if (_connected)
                {
                    if (result.query == CommProtocol.QUERY.Write)
                    {
                        if (_writeItems.Count == 0)
                            OnWriteCompleted.Invoke();
                    }
                    else if (result.query == CommProtocol.QUERY.Read)
                    {
                        foreach (ControlItemInfo item in _readItems)
                        {
                            int index = item.address - result.address;
                            if (index >= 0 && (index + item.bytes - 1) < result.parameters.Count)
                            {
                                int value = 0;
                                if (item.bytes == 1)
                                    value = result.parameters[index];
                                else if (item.bytes == 2)
                                    value = CommProtocol.Bytes2Word(result.parameters[index], result.parameters[index + 1]);

                                if (value != item.value)
                                {
                                    item.update = true;
                                    item.value = value;
                                    item.writeValue = value;
                                }
                                else
                                    item.update = false;
                            }
                        }

                        OnReadUpdate.Invoke();
                    }
                }
                else
                {
                    if (result.query == CommProtocol.QUERY.Ping)
                        CommProtocol.instance.AskWho(_id);
                    else if (result.query == CommProtocol.QUERY.AskWho)
                    {
                        _model = CommProtocol.Bytes2Word(result.parameters[0], result.parameters[1]);
                        _version = result.parameters[2];
                        bool tryUpdate = false;
                        if (ProductManager.manager != null)
                        {
                            productInfo = ProductManager.manager.GetProductInfo(_model);
                            if (productInfo != null)
                            {
                                int minAddr = 0xffff;
                                int maxAddr = -1;
                                foreach (ControlUIInfo uiInfo in productInfo.uiList)
                                {
                                    foreach (ControlItemInfo uiItem in uiInfo.uiItems)
                                    {
                                        minAddr = Mathf.Min(minAddr, uiItem.address);
                                        maxAddr = Mathf.Max(maxAddr, uiItem.address + uiItem.bytes - 1);
                                    }
                                }
                                int bytes = maxAddr - minAddr + 1;
                                if (bytes > 0)
                                {
                                    tryUpdate = true;
                                    CommProtocol.instance.Read(_id, (ushort)minAddr, (ushort)bytes);
                                }
                            }
                        }

                        if (!tryUpdate)
                        {
                            _connected = true;
                //            Debug.Log(string.Format("ID:{0:d} Connected", _id));
                            OnConnected.Invoke(this);
                        }
                    }
                    else if (result.query == CommProtocol.QUERY.Read)
                    {
                        foreach (ControlUIInfo uiInfo in productInfo.uiList)
                        {
                            foreach (ControlItemInfo uiItem in uiInfo.uiItems)
                            {
                                int index = uiItem.address - result.address;
                                if ((index + uiItem.bytes - 1) < result.parameters.Count)
                                {
                                    int value = 0;
                                    if (uiItem.bytes == 1)
                                        value = result.parameters[index];
                                    else if (uiItem.bytes == 2)
                                        value = CommProtocol.Bytes2Word(result.parameters[index], result.parameters[index + 1]);

                                    uiItem.update = true;
                                    uiItem.value = value;
                                    uiItem.writeValue = value;
                                }
                            }
                        }

                        _connected = true;
                //        Debug.Log(string.Format("ID:{0:d} Connected", _id));
                        OnConnected.Invoke(this);
                    }
                }
            }

            if (_run && _connected)
            {
                bool send = false;

                while (_writeItems.Count > 0 && !send)
                {
                    ControlItemInfo writeItem = _writeItems[0];
                    _writeItems.RemoveAt(0);

                    if (writeItem.modify)
                    {
                        if (writeItem.bytes == 1)
                        {
                            send = true;
                            CommProtocol.instance.Write(_id, (ushort)writeItem.address, (byte)writeItem.writeValue);
                        }                            
                        else if (writeItem.bytes == 2)
                        {
                            send = true;
                            CommProtocol.instance.Write(_id, (ushort)writeItem.address, (ushort)writeItem.writeValue);
                        }                            
                    }
                }

                if (_readItems.Count > 0 && !send)
                {
                    int minAddr = 0xffff;
                    int maxAddr = -1;
                    foreach (ControlItemInfo item in _readItems)
                    {
                        minAddr = Mathf.Min(minAddr, item.address);
                        maxAddr = Mathf.Max(maxAddr, item.address + item.bytes - 1);
                    }
                    
                    int bytes = maxAddr - minAddr + 1;
                    if (bytes > 0)
                    {
                        send = true;
                        CommProtocol.instance.Read(_id, (ushort)minAddr, (ushort)bytes);
                    }
                }
                
                if(!send)
                    CommProtocol.instance.Ping(_id);
            }
        }
    }
}
