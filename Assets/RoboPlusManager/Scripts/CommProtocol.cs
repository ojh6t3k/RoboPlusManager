using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


public enum PROTOCOL
{
    CM,
    DXL,
    DXL2,
    UnKnown
}

public class CommProtocol : MonoBehaviour
{
    public enum QUERY
    {
        None,
        Ping,
        AskWho,
        Read,
        Write
    }
    private class Context
    {
        public bool errorRetry = false;        
        public QUERY query = QUERY.None;
        public byte id;
        public byte instruction;
        public List<byte> parameters = new List<byte>();
        public float timeout = 0.1f;
    }
    public class Result
    {
        public QUERY query;
        public byte id;
        public bool txFail = false;
        public bool timeout = false;
        public bool rxFail = false;
        public byte statusError = 0;
        public ushort address;
        public List<byte> parameters = new List<byte>();
    }
    [Serializable]
    public class ResultEvent : UnityEvent<Result> { }


    public static readonly byte BROADCAST_ID = 254;
    public static readonly byte CM_ID = 200;
    public static readonly byte MAX_ID = 252;


    public PROTOCOL protocol = PROTOCOL.DXL2;
    public CommSocket socket;
    public bool debug = false;

    public static CommProtocol _instance = null;

    public ResultEvent OnResult;

    private bool _socketOpen = false;
    private List<Context> _contexts = new List<Context>();
    private Coroutine _processCoroutine = null;

    void Awake()
    {
        _instance = this;

        if (socket != null)
        {
            socket.OnOpen.AddListener(OnSocketOpen);
            socket.OnErrorClosed.AddListener(OnSocketClose);
            socket.OnClose.AddListener(OnSocketClose);
        }
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {        
	}

    public void SetProtocolDXL()
    {
        protocol = PROTOCOL.DXL;
    }

    public void SetProtocolDXL2()
    {
        protocol = PROTOCOL.DXL2;
    }

    public static CommProtocol instance
    {
        get
        {
            return _instance;
        }
    }

    public void Ping(byte id)
    {
        if (socket == null || !_socketOpen)
        {
            Result result = new Result();
            result.query = QUERY.Ping;
            result.id = id;
            result.txFail = true;
            OnResult.Invoke(result);
            return;
        }

        if (protocol == PROTOCOL.CM)
        {

        }
        else if (protocol == PROTOCOL.DXL)
        {
            Context context = new Context();
            context.query = QUERY.Ping;
            context.id = id;
            context.instruction = 0x01; //ping
            _contexts.Add(context);
        }
        else if (protocol == PROTOCOL.DXL2)
        {
            Context context = new Context();
            context.query = QUERY.Ping;
            context.id = id;
            context.instruction = 0x01; //ping
            _contexts.Add(context);
        }
    }

    public void AskWho(byte id)
    {
        if (socket == null || !_socketOpen)
        {
            Result result = new Result();
            result.query = QUERY.AskWho;
            result.id = id;
            result.txFail = true;
            OnResult.Invoke(result);
            return;
        }

        if (protocol == PROTOCOL.CM)
        {

        }
        else if (protocol == PROTOCOL.DXL)
        {
            Context context = new Context();
            context.query = QUERY.AskWho;
            context.id = id;
            context.instruction = 0x02; //read
            context.parameters.Add(0); // address 0
            context.parameters.Add(3); // length 3
            _contexts.Add(context);
        }
        else if (protocol == PROTOCOL.DXL2)
        {
            Context context = new Context();
            context.query = QUERY.AskWho;
            context.id = id;
            context.instruction = 0x01; //ping
            _contexts.Add(context);
        }
    }

    public void Read(byte id, ushort address, ushort length)
    {
        if (socket == null || !_socketOpen || length == 0)
        {
            Result result = new Result();
            result.query = QUERY.Read;
            result.id = id;
            result.txFail = true;
            OnResult.Invoke(result);
            return;
        }

        if (protocol == PROTOCOL.CM)
        {

        }
        else if (protocol == PROTOCOL.DXL)
        {
            Context context = new Context();
            context.query = QUERY.Read;
            context.id = id;
            context.instruction = 0x02; //read
            context.parameters.Add((byte)address);
            context.parameters.Add((byte)length);
            _contexts.Add(context);
        }
        else if (protocol == PROTOCOL.DXL2)
        {
            Context context = new Context();
            context.query = QUERY.Read;
            context.id = id;
            context.instruction = 0x02; //read
            context.parameters.AddRange(Word2Bytes(address));
            context.parameters.AddRange(Word2Bytes(length));
            _contexts.Add(context);
        }
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
        if (socket == null || !_socketOpen || parameters == null)
        {
            Result result = new Result();
            result.query = QUERY.Write;
            result.id = id;
            result.txFail = true;
            OnResult.Invoke(result);
            return;
        }

        if (protocol == PROTOCOL.CM)
        {

        }
        else if (protocol == PROTOCOL.DXL)
        {
            Context context = new Context();
            context.query = QUERY.Write;
            context.id = id;
            context.instruction = 0x03; //write
            context.parameters.Add((byte)address);
            context.parameters.AddRange(parameters);
            _contexts.Add(context);
        }
        else if (protocol == PROTOCOL.DXL2)
        {
            Context context = new Context();
            context.query = QUERY.Write;
            context.id = id;
            context.instruction = 0x03; //write
            context.parameters.AddRange(Word2Bytes(address));
            context.parameters.AddRange(parameters);
            _contexts.Add(context);
        }  
    }

    private void OnSocketOpen()
    {
        _socketOpen = true;
        _processCoroutine = StartCoroutine(Process());
    }

    private void OnSocketClose()
    {
        _socketOpen = false;
        if(_processCoroutine != null)
        {
            StopCoroutine(_processCoroutine);
            _processCoroutine = null;
        }

        if(_contexts.Count > 0)
        {
            Result result = new Result();
            result.query = _contexts[0].query;
            result.id = _contexts[0].id;
            result.txFail = true;
            OnResult.Invoke(result);

            _contexts.Clear();
        }
    }

    private IEnumerator Process()
    {
        List<byte> sendPacket = new List<byte>();
        List<byte> rcvBytes = new List<byte>();

        while(true)
        {
            yield return new WaitForEndOfFrame(); // waste one frame

            while (_contexts.Count > 0)
            {
                float time = 0f;
                sendPacket.Clear();
                rcvBytes.Clear();

                Result result = new Result();
                result.query = _contexts[0].query;
                result.id = _contexts[0].id;

                if (protocol == PROTOCOL.CM)
                {
                }
                else if (protocol == PROTOCOL.DXL)
                {
                    sendPacket.AddRange(new byte[] { 0xff, 0xff });
                    sendPacket.Add(_contexts[0].id);
                    sendPacket.Add((byte)(_contexts[0].parameters.Count + 2));
                    sendPacket.Add(_contexts[0].instruction);
                    if (_contexts[0].parameters.Count > 0)
                        sendPacket.AddRange(_contexts[0].parameters.ToArray());
                    sendPacket.Add(GetChecksum(sendPacket.GetRange(2, _contexts[0].parameters.Count + 3).ToArray()));
                }
                else if (protocol == PROTOCOL.DXL2)
                {
                    sendPacket.AddRange(new byte[] { 0xff, 0xff, 0xfd, 0x00 });
                    sendPacket.Add(_contexts[0].id);
                    sendPacket.AddRange(Word2Bytes((ushort)(_contexts[0].parameters.Count + 3)));
                    sendPacket.Add(_contexts[0].instruction);
                    if (_contexts[0].parameters.Count > 0)
                        sendPacket.AddRange(_contexts[0].parameters.ToArray());
                    sendPacket.AddRange(Word2Bytes(GetCRC(sendPacket.ToArray())));
                }

                socket.Write(sendPacket.ToArray());
                if (debug)
                {
                    string debugString = "TX:";
                    for (int i = 0; i < sendPacket.Count; i++)
                    {
                        if (protocol == PROTOCOL.CM)
                            debugString += string.Format(" {0:s}", sendPacket[i]);
                        else
                            debugString += string.Format(" {0:X}", sendPacket[i]);
                    }
                    Debug.Log(debugString);
                }

                if (_contexts[0].id != BROADCAST_ID)
                {
                    while (true)
                    {
                        byte[] data = socket.Read();
                        if (data != null)
                        {
                            if (data.Length > 0)
                            {
                                rcvBytes.AddRange(data);
                                if (protocol == PROTOCOL.CM)
                                {
                                }
                                else if (protocol == PROTOCOL.DXL)
                                {
                                    while (rcvBytes.Count >= 2)
                                    {
                                        if (rcvBytes[0] == 0xff && rcvBytes[1] == 0xff)
                                            break;

                                        rcvBytes.RemoveAt(0);
                                    }

                                    if (rcvBytes.Count >= 6)
                                    {
                                        byte id = rcvBytes[2];
                                        byte length = rcvBytes[3];
                                        if (rcvBytes.Count >= (length + 4))
                                        {
                                            byte checksum = rcvBytes[length + 3];
                                            byte checksum2 = GetChecksum(rcvBytes.GetRange(2, length + 1).ToArray());
                                            if (checksum == checksum2)
                                            {
                                                if (_contexts[0].id == id)
                                                {
                                                    if (debug)
                                                    {
                                                        string debugString = "RX:";
                                                        for (int i = 0; i < (length + 4); i++)
                                                            debugString += string.Format(" {0:X}", rcvBytes[i]);
                                                        Debug.Log(debugString);
                                                    }

                                                    if (_contexts[0].query == QUERY.AskWho)
                                                    {
                                                        if (_contexts[0].instruction == 0x02) // read
                                                        {
                                                            result.address = 0;
                                                            result.parameters.AddRange(rcvBytes.GetRange(5, 3).ToArray());
                                                        }
                                                    }
                                                    else if (_contexts[0].query == QUERY.Read)
                                                    {
                                                        if (_contexts[0].instruction == 0x02) // read
                                                        {
                                                            result.address = _contexts[0].parameters[0];
                                                            result.parameters.AddRange(rcvBytes.GetRange(5, length - 2).ToArray());
                                                        }
                                                    }

                                                    result.statusError = rcvBytes[4];
                                                    result.rxFail = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    result.rxFail = true;
                                                    rcvBytes.RemoveRange(0, length + 4);
                                                }
                                            }
                                            else
                                            {
                                                result.rxFail = true;
                                                rcvBytes.RemoveRange(0, length + 4);
                                            }
                                        }
                                    }
                                }
                                else if (protocol == PROTOCOL.DXL2)
                                {
                                    while (rcvBytes.Count >= 4)
                                    {
                                        if (rcvBytes[0] == 0xff && rcvBytes[1] == 0xff && rcvBytes[2] == 0xfd && rcvBytes[3] == 0x00)
                                            break;

                                        rcvBytes.RemoveAt(0);
                                    }

                                    if (rcvBytes.Count >= 10)
                                    {
                                        byte id = rcvBytes[4];
                                        ushort length = Bytes2Word(rcvBytes[5], rcvBytes[6]);
                                        if (rcvBytes.Count >= (length + 7))
                                        {
                                            ushort crc = Bytes2Word(rcvBytes[length + 5], rcvBytes[length + 6]);
                                            ushort crc2 = GetCRC(rcvBytes.GetRange(0, length + 5).ToArray());
                                            if (crc == crc2)
                                            {
                                                if (_contexts[0].id == id && rcvBytes[7] == 0x55)
                                                {
                                                    if (debug)
                                                    {
                                                        string debugString = "RX:";
                                                        for (int i = 0; i < (length + 7); i++)
                                                            debugString += string.Format(" {0:X}", rcvBytes[i]);
                                                        Debug.Log(debugString);
                                                    }

                                                    if (_contexts[0].query == QUERY.AskWho)
                                                    {
                                                        if (_contexts[0].instruction == 0x01) // ping
                                                        {
                                                            result.address = 0;
                                                            result.parameters.AddRange(rcvBytes.GetRange(9, 3).ToArray());
                                                        }
                                                    }
                                                    else if (_contexts[0].query == QUERY.Read)
                                                    {
                                                        if (_contexts[0].instruction == 0x02) // read
                                                        {
                                                            result.address = Bytes2Word(_contexts[0].parameters[0], _contexts[0].parameters[1]);
                                                            result.parameters.AddRange(rcvBytes.GetRange(9, length - 4).ToArray());
                                                        }
                                                    }

                                                    result.statusError = rcvBytes[8];
                                                    result.rxFail = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    result.rxFail = true;
                                                    rcvBytes.RemoveRange(0, length + 7);
                                                }
                                            }
                                            else
                                            {
                                                result.rxFail = true;
                                                rcvBytes.RemoveRange(0, length + 7);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (time > _contexts[0].timeout)
                        {
                            result.timeout = true;
                            if (rcvBytes.Count > 0)
                                result.rxFail = true;

                            break;
                        }
                        else
                            time += Time.deltaTime;

                        yield return new WaitForEndOfFrame();
                    }
                }

                _contexts.RemoveAt(0);
                while (_contexts.Count > 0)
                {
                    if (result.rxFail || result.timeout)
                    {
                        if (_contexts[0].errorRetry)
                            break;
                        else
                            _contexts.RemoveAt(0);
                    }
                    else
                    {
                        if (_contexts[0].errorRetry)
                            _contexts.RemoveAt(0);
                        else
                            break;
                    }
                }

                OnResult.Invoke(result);
            }
        }
    }

    public static byte[] Word2Bytes(ushort word)
    {
        return new byte[] { (byte)(word & 0xff), (byte)(word >> 8) };
    }

    public static ushort Bytes2Word(byte low, byte high)
    {
        return (ushort)((high << 8) + low);
    }

    private static byte GetChecksum(byte[] bytes)
    {
        byte checksum = 0;
        for (int i = 0; i < bytes.Length; i++)
            checksum += bytes[i];

        return (byte)~checksum;
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
