using UnityEngine;
using UnityEngine.UI;


public class CommSocketUI : MonoBehaviour
{
    public CommSocket socket;
    public ListView uiDeviceList;
    public ListItem uiDeviceItem;
    public Button uiOpen;
    public Button uiSearch;
    public GameObject messageRoot;
    public GameObject messageConnecting;
    public GameObject messageConnectionFailed;
//    public GameObject message

    private bool _preventEvent = false;
    

    void Awake()
    {
        socket.OnFoundDevice.AddListener(OnFoundDevice);
        socket.OnSearchCompleted.AddListener(OnSearchCompleted);

        uiDeviceList.OnChangedSelection.AddListener(OnChangedDevice);
        uiOpen.onClick.AddListener(OnClickOpen);
        uiSearch.onClick.AddListener(OnClickSearch);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ShowUI()
    {
        OnClickSearch();
    }

    private void OnChangedDevice()
    {
        if (_preventEvent)
            return;

        ListItem selected = uiDeviceList.selectedItem;
        if(selected != null)
            socket.device = new CommDevice((CommDevice)selected.data);
    }

    private void OnFoundDevice()
    {
        for(int i=uiDeviceList.itemCount; i<socket.foundDevices.Count; i++)
        {
            ListItem item = GameObject.Instantiate(uiDeviceItem);
            item.textList[0].text = socket.foundDevices[i].type.ToString();
            item.textList[1].text = socket.foundDevices[i].name;
            item.data = socket.foundDevices[i];
            uiDeviceList.AddItem(item);
        }

        _preventEvent = true;
        for(int i=0; i<socket.foundDevices.Count; i++)
        {
            if(socket.device.Equals(socket.foundDevices[i]))
            {
                uiDeviceList.selectedIndex = i;
                break;
            }
        }
        _preventEvent = false;
    }

    private void OnSearchCompleted()
    {
        uiSearch.interactable = true;
    }

    private void OnClickOpen()
    {
        socket.Open();
    }

    private void OnClickSearch()
    {
        uiDeviceList.ClearItem();
        uiSearch.interactable = false;
        socket.Search();
    }
}
