using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CommSocket))]
public class CommSocketUI : MonoBehaviour
{
    public ListView uiDeviceList;
    public ListItem uiDeviceItem;

    private CommSocket _socket;
    private bool _preventEvent = false;
    

    void Awake()
    {
        _socket = GetComponent<CommSocket>();
        _socket.OnFoundDevice.AddListener(OnFoundDevice);
        _socket.OnSearchCompleted.AddListener(OnSearchCompleted);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnWriteTest()
    {
        _socket.Write(new byte[] { 0, 1, 2, 3, 4, 5 });
    }

    public void OnSearch()
    {
        uiDeviceList.ClearItem();
        _socket.Search();
    }

    public void OnChangedDevice()
    {
        if (_preventEvent)
            return;

        ListItem selected = uiDeviceList.selectedItem;
        if(selected != null)
            _socket.device = new CommDevice((CommDevice)selected.data);
    }

    private void OnFoundDevice()
    {
        for(int i=uiDeviceList.itemCount; i<_socket.foundDevices.Count; i++)
        {
            ListItem item = GameObject.Instantiate(uiDeviceItem);
            item.textList[0].text = _socket.foundDevices[i].type.ToString();
            item.textList[1].text = _socket.foundDevices[i].name;
            item.data = _socket.foundDevices[i];
            uiDeviceList.AddItem(item);
        }

        _preventEvent = true;
        for(int i=0; i<_socket.foundDevices.Count; i++)
        {
            if(_socket.device.Equals(_socket.foundDevices[i]))
            {
                uiDeviceList.selectedIndex = i;
                break;
            }
        }
        _preventEvent = false;
    }

    private void OnSearchCompleted()
    {

    }
}
