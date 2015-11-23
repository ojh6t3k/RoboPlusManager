using UnityEngine;
using System;
using UnityEngine.UI;


public class RoboPlusManager : MonoBehaviour
{
    [Serializable]
    public class UI
    {
        public Dropdown language;
        public ListView deviceList;
        public ListItem deviceItem;
        public Button[] commShow;
        public Button[] commClose;
        public Button commSearch;
        public GameObject messageRoot;
        public GameObject messageConnecting;
        public GameObject messageOpenFailed;
        public GameObject messageErrorClose;
        public CommProductUI commProduct;
    }

    public UI ui;
    public CommSocket socket;
    public CommProtocol protocol;

    private bool _preventEvent = false;


    void Awake()
    {
        socket.OnOpen.AddListener(OnCommSocketOpen);
        socket.OnOpenFailed.AddListener(OnCommSocketOpenFailed);
        socket.OnClose.AddListener(OnCommSocketClose);
        socket.OnErrorClosed.AddListener(OnCommSocketErrorClose);
        socket.OnFoundDevice.AddListener(OnCommSocketFoundDevice);
        socket.OnSearchCompleted.AddListener(OnCommSocketSearchCompleted);

        ui.deviceList.OnChangedSelection.AddListener(OnChangedCommDevice);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void InitSettings()
    {
        LanguageInfo[] languages = LocalizationManager.manager.languages;
        ui.language.options.Clear();
        for (int i = 0; i < languages.Length; i++)
            ui.language.options.Add(new Dropdown.OptionData(languages[i].name, languages[i].image));

        string currentLanguage = LocalizationManager.manager.currentLanguage;
        int index = 0;
        for (int i = 0; i < languages.Length; i++)
        {
            if (languages[i].iso3code.Equals(currentLanguage))
            {
                index = i;
                break;
            }
        }
        ui.language.value = index;
        ui.language.captionImage.sprite = languages[index].image;
        ui.language.captionText.text = languages[index].name;
        ui.language.captionImage.enabled = true;
    }

    public void ApplySettings()
    {
        LanguageInfo[] languages = LocalizationManager.manager.languages;
        int index = ui.language.value;
        LocalizationManager.manager.currentLanguage = languages[index].iso3code;
        LocalizationManager.manager.ApplyLanguage();
    }

    public void CommSocketSearch()
    {
        ui.deviceList.ClearItem();
        ui.commSearch.interactable = false;
        socket.Search();        
    }

    public void CommSocketOpen()
    {
        ui.messageConnecting.SetActive(true);
        ui.messageRoot.SetActive(true);
        socket.Open();        
    }

    public void CommSocketClose()
    {
        ui.commProduct.CommClose();
        socket.Close();
        OnCommSocketClose();
    }

    private void OnCommSocketOpen()
    {
        ui.messageRoot.SetActive(false);
        ui.messageConnecting.SetActive(false);        

        foreach (Button btn in ui.commShow)
        {
            if (btn != null)
                btn.gameObject.SetActive(false);
        }
        foreach (Button btn in ui.commClose)
        {
            if (btn != null)
                btn.gameObject.SetActive(true);
        }

        ui.commProduct.CommOpen();
    }

    private void OnCommSocketOpenFailed()
    {
        ui.messageConnecting.SetActive(false);
        ui.messageOpenFailed.SetActive(true);
    }

    private void OnCommSocketClose()
    {
        foreach (Button btn in ui.commShow)
        {
            if (btn != null)
                btn.gameObject.SetActive(true);
        }
        foreach (Button btn in ui.commClose)
        {
            if (btn != null)
                btn.gameObject.SetActive(false);
        }
    }

    private void OnCommSocketErrorClose()
    {
        OnCommSocketClose();
        ui.messageErrorClose.SetActive(true);
        ui.messageRoot.SetActive(true);
    }

    private void OnCommSocketFoundDevice()
    {
        for (int i = ui.deviceList.itemCount; i < socket.foundDevices.Count; i++)
        {
            ListItem item = GameObject.Instantiate(ui.deviceItem);
            item.textList[0].text = socket.foundDevices[i].type.ToString();
            item.textList[1].text = socket.foundDevices[i].name;
            item.data = socket.foundDevices[i];
            ui.deviceList.AddItem(item);
        }

        _preventEvent = true;
        for (int i = 0; i < socket.foundDevices.Count; i++)
        {
            if (socket.device.Equals(socket.foundDevices[i]))
            {
                ui.deviceList.selectedIndex = i;
                break;
            }
        }
        _preventEvent = false;
    }

    private void OnCommSocketSearchCompleted()
    {
        ui.commSearch.interactable = true;
    }

    private void OnChangedCommDevice()
    {
        if (_preventEvent)
            return;

        ListItem selected = ui.deviceList.selectedItem;
        if (selected != null)
            socket.device = new CommDevice((CommDevice)selected.data);
    }
}
