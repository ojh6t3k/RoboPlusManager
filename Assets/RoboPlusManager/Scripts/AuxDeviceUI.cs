using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class AuxDeviceUI : ControlUI
{
    public Dropdown uiPorts;
    public ListView uiDevices;
    public ListItem uiDevice;

    private List<string[]> _avaliableDevices = new List<string[]>();
    private string[] _DEVICES = {
        "AuxMotor",
        "AuxServo",
        "AuxIR",
        "AuxDMS",
        "AuxTouch",
        "AuxLED",
        "AuxTemperature",
        "AuxUltraSonic",
        "AuxMagnetic",
        "AuxMotion",
        "AuxColor",
        "AuxCustom"
    };
    private bool _preventEvent = false;

    void Start()
    {

    }

    protected override void OnSetUiInfo()
    {
        ControlUIInfo info = uiInfo;

        _avaliableDevices.Clear();
        for (int i = 0; i < info.uiParameters.Length; i++)
            _avaliableDevices.Add(null);

        for (int i=0; i<info.uiParameters.Length; i++)
        {
            string[] tokens = info.uiParameters[i].Split(new char[] { '[', ']' }, System.StringSplitOptions.RemoveEmptyEntries);
            int n = int.Parse(tokens[0]);
            List<string> devices = new List<string>();
            if (tokens[1].StartsWith("-"))
                devices.AddRange(_DEVICES);
            tokens[1] = tokens[1].TrimStart(new char[] { '-' });
            tokens = tokens[1].Split(new char[] { '|' });
            for(int j=0; j<tokens.Length; j++)
            {
                if (devices.Contains(tokens[j]))
                    devices.Remove(tokens[j]);
                else
                    devices.Add(tokens[j]);
            }
            _avaliableDevices[n-1] = devices.ToArray();
        }

        _preventEvent = true;

        uiPorts.options.Clear();
        for (int i = 0; i < _avaliableDevices.Count; i++)
            uiPorts.options.Add(new Dropdown.OptionData((i + 1).ToString()));
        uiPorts.value = 0;

        RefreshDeviceList();

        _preventEvent = false;
    }

    private void RefreshDeviceList()
    {
        uiDevices.ClearItem();
        string[] list = _avaliableDevices[uiPorts.value];
        for (int i=0; i< list.Length; i++)
        {
            ListItem item = GameObject.Instantiate(uiDevice);
            item.textList[0].text = list[i];
            uiDevices.AddItem(item);
        }

        uiDevices.selectedIndex = 0;
    }

    public void OnChangedPort()
    {        
        if (_preventEvent)
            return;

        _preventEvent = true;

        RefreshDeviceList();

        _preventEvent = false;
    }
}
