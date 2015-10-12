using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


public class CommProtocol : MonoBehaviour
{
    public enum VERSION
    {
        CM,
        DXL,
        DXL2
    }
    private enum INSTRUCTION
    {
        None = 0x00,
        Ping = 0x01,
        Read = 0x02,
        Write = 0x03,
        RegWrite = 0x04,
        Action = 0x05,
        FactoryReset = 0x06,
        Reset = 0x07,
        Status = 0x55,
        SyncRead = 0x82,
        SyncWrite = 0x83,
        BulkRead = 0x92,
        BulkWrite = 0x93,
    }
    private enum PROCESS
    {
        None,
        ReadProductInfo
    }
    public enum ERROR
    {
        Instruction,
        TxFailed,
        Busy,
        CRC,
        NotPaired,
        ShortLength,
        Timeout
    }
    [Serializable]
    public class ErrorEvent : UnityEvent<byte, ERROR> { }
    [Serializable]
    public class StatusEvent : UnityEvent<byte, byte> { }
    [Serializable]
    public class PingEvent : UnityEvent<byte> { }
    [Serializable]
    public class ProductInfoEvent : UnityEvent<byte, ushort, byte> { }
    [Serializable]
    public class ReadEvent : UnityEvent<byte, ushort, byte[]> { }
        
    public static readonly byte BROADCAST_ID = 254;
    public static readonly byte CM_ID = 200;
    public static readonly byte MAX_ID = 252;


    public VERSION version = VERSION.DXL2;
    public float timeout = 1f;
    public CommSocket socket;
    public bool debug = false;

    public ErrorEvent OnError;
    public StatusEvent OnStatus;
    public PingEvent OnPing;
    public ProductInfoEvent OnProductInfo;
    public ReadEvent OnRead;

    private bool _socketOpen = false;
    private bool _sendInstruction = false;    
    private float _time;
    private List<byte> _rcvBytes = new List<byte>();

    // Context
    private byte _id;
    private PROCESS _process = PROCESS.None;
    private INSTRUCTION _instruction = INSTRUCTION.None;
    private List<byte> _parameters = new List<byte>();

    void Awake()
    {
        if(socket != null)
        {
            socket.OnOpen.AddListener(OnSocketOpen);
            socket.OnErrorClosed.AddListener(OnSocketClose);
            socket.OnClose.AddListener(OnSocketClose);
        }

        OnError.AddListener(ErrorHandler);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_sendInstruction && _socketOpen)
        {
            byte[] data = socket.Read();
            if(data != null)
            {
                if(data.Length > 0)
                {
                    _rcvBytes.AddRange(data);
                    if (version == VERSION.CM)
                    {
                    }
                    else if (version == VERSION.DXL)
                    {
                    }
                    else if (version == VERSION.DXL2)
                    {
                        while (_rcvBytes.Count >= 4)
                        {
                            if (_rcvBytes[0] == 0xff && _rcvBytes[1] == 0xff && _rcvBytes[2] == 0xfd && _rcvBytes[3] == 0x00)
                                break;

                            _rcvBytes.RemoveAt(0);
                        }

                        if (_rcvBytes.Count >= 10)
                        {
                            byte id = _rcvBytes[4];
                            ushort length = Bytes2Word(_rcvBytes[5], _rcvBytes[6]);
                            if (_rcvBytes.Count >= (length + 7))
                            {                                
                                ushort crc = Bytes2Word(_rcvBytes[length + 5], _rcvBytes[length + 6]);
                                ushort crc2 = GetCRC(_rcvBytes.GetRange(0, length + 5).ToArray());
                                if (crc == crc2)
                                {
                                    if((_id == id) && (_rcvBytes[7] == (byte)INSTRUCTION.Status))
                                    {
                                        _sendInstruction = false;
                                        if (debug)
                                        {
                                            string debugString = "RX:";
                                            for (int i = 0; i < (length + 7); i++)
                                                debugString += string.Format(" {0:X}", _rcvBytes[i]);
                                            Debug.Log(debugString);
                                        }

                                        if (_process == PROCESS.None)
                                        {
                                            if (_instruction == INSTRUCTION.Ping)
                                            {
                                                OnPing.Invoke(id);
                                            }
                                            else if (_instruction == INSTRUCTION.Read)
                                            {
                                                ushort address = Bytes2Word(_rcvBytes[9], _rcvBytes[10]);
                                                byte[] result = _rcvBytes.GetRange(0, length + 7).ToArray();
                                                OnRead.Invoke(id, address, result);
                                            }

                                            byte error = _rcvBytes[8];
                                            OnStatus.Invoke(id, error);
                                        }
                                        else if (_process == PROCESS.ReadProductInfo)
                                        {
                                            if (_instruction == INSTRUCTION.Ping)
                                            {
                                                _process = PROCESS.None;
                                                ushort model = Bytes2Word(_rcvBytes[9], _rcvBytes[10]);
                                                byte ver = _rcvBytes[11];
                                                OnProductInfo.Invoke(id, model, ver);
                                            }
                                        }                                        
                                    }
                                    else
                                    {
                                        _rcvBytes.RemoveRange(0, length + 7);
                                        OnError.Invoke(_id, ERROR.NotPaired);
                                    }
                                }
                                else
                                {
                                    _rcvBytes.RemoveRange(0, length + 7);
                                    OnError.Invoke(_id, ERROR.CRC);
                                }
                            }
                        }
                    }
                }                
            }
            
            if (_time > timeout)
            {
                _sendInstruction = false;
                _process = PROCESS.None;
                if (_rcvBytes.Count > 0)
                    OnError.Invoke(_id, ERROR.ShortLength);

                OnError.Invoke(_id, ERROR.Timeout);
            }
            else
                _time += Time.deltaTime;
        }
	}

    public void Ping(byte id)
    {
        if(_sendInstruction)
        {
            OnError.Invoke(id, ERROR.Busy);
            return;
        }

        _id = id;
        _instruction = INSTRUCTION.Ping;
        _parameters.Clear();

        SendInstruction();
    }

    public void ReadProductInfo(byte id)
    {
        if (_sendInstruction)
        {
            OnError.Invoke(id, ERROR.Busy);
            return;
        }

        _process = PROCESS.ReadProductInfo;
        if(version == VERSION.CM)
        {

        }
        else if(version == VERSION.DXL)
        {

        }
        else if(version == VERSION.DXL2)
        {
            Ping(id);
        }
    }

    public void Read(byte id, ushort address, ushort length)
    {
        if (_sendInstruction)
        {
            OnError.Invoke(id, ERROR.Busy);
            return;
        }

        if (length == 0)
        {
            OnError.Invoke(id, ERROR.Instruction);
            return;
        }

        _id = id;
        _instruction = INSTRUCTION.Read;
        _parameters.Clear();
        if (version == VERSION.DXL2)
        {
            _parameters.AddRange(Word2Bytes(address));
            _parameters.AddRange(Word2Bytes(length));
        }            
        else
        {
            _parameters.Add((byte)address);
            _parameters.Add((byte)length);
        }

        SendInstruction();
    }

    public void Write(byte id, ushort address, byte parameter)
    {
        Write(id, address, new byte[] { parameter });
    }

    public void Write(byte id, ushort address, ushort parameter)
    {
        Write(id, address, Word2Bytes(parameter));
    }

    public void Write(byte id, ushort address, byte[] parameters)
    {
        if (_sendInstruction)
        {
            OnError.Invoke(id, ERROR.Busy);
            return;
        }

        if(parameters == null)
        {
            OnError.Invoke(id, ERROR.Instruction);
            return;
        }

        _id = id;
        _instruction = INSTRUCTION.Write;
        _parameters.Clear();
        if (version == VERSION.DXL2)
            _parameters.AddRange(Word2Bytes(address));
        else
            _parameters.Add((byte)address);
        _parameters.AddRange(parameters);

        SendInstruction();
    }

    private void OnSocketOpen()
    {
        _socketOpen = true;
    }

    private void OnSocketClose()
    {
        _socketOpen = false;
        _sendInstruction = false;
    }

    public void SendInstruction()
    {
        if(socket == null || !_socketOpen)
        {
            OnError.Invoke(_id, ERROR.TxFailed);
            return;
        }

        List<byte> packet = new List<byte>();
        if(version == VERSION.CM)
        {

        }
        else if(version == VERSION.DXL)
        {
            packet.AddRange(new byte[] { 0xff, 0xff });
        }
        else if (version == VERSION.DXL2)
        {
            packet.AddRange(new byte[] { 0xff, 0xff, 0xfd, 0x00 });
            packet.Add(_id);
            packet.AddRange(Word2Bytes((ushort)(_parameters.Count + 3)));
            packet.Add((byte)_instruction);
            if (_parameters.Count > 0)
                packet.AddRange(_parameters.ToArray());
            packet.AddRange(Word2Bytes(GetCRC(packet.ToArray())));
        }

        if(packet.Count == 0)
        {
            OnError.Invoke(_id, ERROR.Instruction);
            return;
        }

        _rcvBytes.Clear();
        socket.Write(packet.ToArray());
        if(debug)
        {
            string debugString = "TX:";
            for (int i = 0; i < packet.Count; i++)
                debugString += string.Format(" {0:X}", packet[i]);
            Debug.Log(debugString);
        }

        if (_id != BROADCAST_ID)
        {
            _time = 0f;
            _sendInstruction = true;
        }        
    }

    private void ErrorHandler(byte id, ERROR error)
    {
        if (debug)
            Debug.Log("Error: " + error);
    }

    public static byte[] Word2Bytes(ushort word)
    {
        return new byte[] { (byte)(word & 0xff), (byte)(word >> 8) };
    }

    public static ushort Bytes2Word(byte low, byte high)
    {
        return (ushort)((high << 8) + low);
    }
    
    private static ushort GetCRC(byte[] bytes)
    {
        ushort[] _crc_table = {
                0x0000, 0x8005, 0x800F, 0x000A, 0x801B, 0x001E, 0x0014, 0x8011,
                0x8033, 0x0036, 0x003C, 0x8039, 0x0028, 0x802D, 0x8027, 0x0022,
                0x8063, 0x0066, 0x006C, 0x8069, 0x0078, 0x807D, 0x8077, 0x0072,
                0x0050, 0x8055, 0x805F, 0x005A, 0x804B, 0x004E, 0x0044, 0x8041,
                0x80C3, 0x00C6, 0x00CC, 0x80C9, 0x00D8, 0x80DD, 0x80D7, 0x00D2,
                0x00F0, 0x80F5, 0x80FF, 0x00FA, 0x80EB, 0x00EE, 0x00E4, 0x80E1,
                0x00A0, 0x80A5, 0x80AF, 0x00AA, 0x80BB, 0x00BE, 0x00B4, 0x80B1,
                0x8093, 0x0096, 0x009C, 0x8099, 0x0088, 0x808D, 0x8087, 0x0082,
                0x8183, 0x0186, 0x018C, 0x8189, 0x0198, 0x819D, 0x8197, 0x0192,
                0x01B0, 0x81B5, 0x81BF, 0x01BA, 0x81AB, 0x01AE, 0x01A4, 0x81A1,
                0x01E0, 0x81E5, 0x81EF, 0x01EA, 0x81FB, 0x01FE, 0x01F4, 0x81F1,
                0x81D3, 0x01D6, 0x01DC, 0x81D9, 0x01C8, 0x81CD, 0x81C7, 0x01C2,
                0x0140, 0x8145, 0x814F, 0x014A, 0x815B, 0x015E, 0x0154, 0x8151,
                0x8173, 0x0176, 0x017C, 0x8179, 0x0168, 0x816D, 0x8167, 0x0162,
                0x8123, 0x0126, 0x012C, 0x8129, 0x0138, 0x813D, 0x8137, 0x0132,
                0x0110, 0x8115, 0x811F, 0x011A, 0x810B, 0x010E, 0x0104, 0x8101,
                0x8303, 0x0306, 0x030C, 0x8309, 0x0318, 0x831D, 0x8317, 0x0312,
                0x0330, 0x8335, 0x833F, 0x033A, 0x832B, 0x032E, 0x0324, 0x8321,
                0x0360, 0x8365, 0x836F, 0x036A, 0x837B, 0x037E, 0x0374, 0x8371,
                0x8353, 0x0356, 0x035C, 0x8359, 0x0348, 0x834D, 0x8347, 0x0342,
                0x03C0, 0x83C5, 0x83CF, 0x03CA, 0x83DB, 0x03DE, 0x03D4, 0x83D1,
                0x83F3, 0x03F6, 0x03FC, 0x83F9, 0x03E8, 0x83ED, 0x83E7, 0x03E2,
                0x83A3, 0x03A6, 0x03AC, 0x83A9, 0x03B8, 0x83BD, 0x83B7, 0x03B2,
                0x0390, 0x8395, 0x839F, 0x039A, 0x838B, 0x038E, 0x0384, 0x8381,
                0x0280, 0x8285, 0x828F, 0x028A, 0x829B, 0x029E, 0x0294, 0x8291,
                0x82B3, 0x02B6, 0x02BC, 0x82B9, 0x02A8, 0x82AD, 0x82A7, 0x02A2,
                0x82E3, 0x02E6, 0x02EC, 0x82E9, 0x02F8, 0x82FD, 0x82F7, 0x02F2,
                0x02D0, 0x82D5, 0x82DF, 0x02DA, 0x82CB, 0x02CE, 0x02C4, 0x82C1,
                0x8243, 0x0246, 0x024C, 0x8249, 0x0258, 0x825D, 0x8257, 0x0252,
                0x0270, 0x8275, 0x827F, 0x027A, 0x826B, 0x026E, 0x0264, 0x8261,
                0x0220, 0x8225, 0x822F, 0x022A, 0x823B, 0x023E, 0x0234, 0x8231,
                0x8213, 0x0216, 0x021C, 0x8219, 0x0208, 0x820D, 0x8207, 0x0202  };

        ushort sum = 0;
        for (int i=0; i< bytes.Length; i++)
        {
            int n = ((sum >> 8) ^ bytes[i]) & 0xff;
            sum = (ushort)((sum << 8) ^ _crc_table[n]);
        }
 
        return sum;
    }
}
