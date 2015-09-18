using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class CommunicationUI : ControlUI
{
	public Button uiSave;
	public UpdownValue uiBaudrate;
    public Dropdown uiBaudrate2;
    public UpdownValue uiID;
	public UpdownValue uiReturnDelay;
	public Toggle uiReturnLevel0;
    public Toggle uiReturnLevel1;
    public Toggle uiReturnLevel2;
    public Text uiBPS;
	public Text uiDelayTime;

	private ControlItemInfo _baudrate;
	private ControlItemInfo _id;
	private ControlItemInfo _returnDelay;
	private ControlItemInfo _returnLevel;

	private float[] _standardBps = {
		921600,
		460800,
		256000,
		230400,
		153600,
		128000,
		115200,
		57600,
		38400,
		28800,
		19200,
		14400,
		9600,
		4800,
		2400,
		1200,
		600,
		300,
		110
	};

	void Start()
	{
	}
	
	protected override void OnUpdateUIInfo()
	{
		ControlUIInfo info = uiInfo;
		
		_baudrate = info.GetUIItem("Baudrate");
		_id = info.GetUIItem("ID");
		_returnDelay = info.GetUIItem("ReturnDelay");
		_returnLevel = info.GetUIItem("ReturnLevel");		

		uiID.minValue = _id.minValue;
		uiID.maxValue = _id.maxValue;
		uiID.unitValue = 1;
		uiID.format = "f0";
		uiID.Value = _id.value;

		uiReturnDelay.minValue = _returnDelay.minValue;
		uiReturnDelay.maxValue = _returnDelay.maxValue;
		uiReturnDelay.unitValue = 1;
		uiReturnDelay.format = "f0";
		uiReturnDelay.Value = _returnDelay.value;
        OnChangedReturnDelay();

        if (_returnLevel.value == 0)
            uiReturnLevel0.isOn = true;
        else if(_returnLevel.value == 1)
            uiReturnLevel1.isOn = true;
        else if (_returnLevel.value == 2)
            uiReturnLevel2.isOn = true;

        if(info.version == 1)
        {
            uiBaudrate.gameObject.SetActive(true);
            uiBPS.gameObject.SetActive(true);
            uiBaudrate2.gameObject.SetActive(false);

            uiBaudrate.minValue = _baudrate.minValue;
            uiBaudrate.maxValue = _baudrate.maxValue;
            uiBaudrate.unitValue = 1;
            uiBaudrate.format = "f0";
            uiBaudrate.Value = _baudrate.value;
            OnChangedBaudrate();
        }
        else if(info.version == 2)
        {
            uiBaudrate.gameObject.SetActive(false);
            uiBPS.gameObject.SetActive(false);
            uiBaudrate2.gameObject.SetActive(true);

            uiBaudrate2.value = _baudrate.value;
        }		
		
		uiSave.interactable = false;
	}

	public void OnReset()
	{
		_baudrate.Reset();
		_id.Reset();
		_returnDelay.Reset();
		_returnLevel.Reset();

		uiID.Value = _id.value;
		uiReturnDelay.Value = _returnDelay.value;
        if (_returnLevel.value == 0)
            uiReturnLevel0.isOn = true;
        else if (_returnLevel.value == 1)
            uiReturnLevel1.isOn = true;
        else if (_returnLevel.value == 2)
            uiReturnLevel2.isOn = true;

        if (uiInfo.version == 1)
        {
            uiBaudrate.Value = _baudrate.value;
        }
        else if (uiInfo.version == 2)
        {
            uiBaudrate2.value = _baudrate.value;
        }
    }

	public void OnSave()
	{
		uiSave.interactable = false;
	}

    public void OnChangedBaudrate()
	{
		float curBps = 2000000f / (uiBaudrate.Value + 1f);
		string dispText = string.Format("{0:f0} bps", curBps);

		List<float> similarBps = new List<float>();
		for(int i=0; i<_standardBps.Length; i++)
		{
			if(Mathf.Abs(1f - curBps / _standardBps[i]) < 0.03f)
				similarBps.Add(_standardBps[i]);
		}

		if(similarBps.Count > 0)
		{
			for(int i=0; i<similarBps.Count; i++)
			{
				for(int j=i; j<(similarBps.Count - 1); j++)
				{
					if(Mathf.Abs(1f - curBps / _standardBps[j]) > Mathf.Abs(1f - curBps / _standardBps[j+1]))
					{
						float temp = similarBps[j+1];
						similarBps.RemoveAt(j+1);
						similarBps.Insert(j, temp);
					}
				}
			}

			dispText += string.Format(" ({0:f0}, {1:f2} %)", similarBps[0], Mathf.Abs(1f - curBps / similarBps[0]));
		}

		_baudrate.value = (int)uiBaudrate.Value;
		uiBPS.text = dispText;
		uiSave.interactable = true;
	}

    public void OnChangedBaudrate2()
    {
        _baudrate.value = uiBaudrate2.value;

        uiSave.interactable = true;
    }

    public void OnChangedID()
	{
		_id.value = (int)uiID.Value;
		uiSave.interactable = true;
	}

    public void OnChangedReturnDelay()
	{
		uiDelayTime.text = string.Format("{0:f0} usec", uiReturnDelay.Value * 2f);

		_returnDelay.value = (int)uiReturnDelay.Value;
		uiSave.interactable = true;
	}

    public void OnChangedReturnLevel()
	{
        if (uiReturnLevel0.isOn)
            _returnLevel.value = 0;
        if (uiReturnLevel1.isOn)
            _returnLevel.value = 1;
        if (uiReturnLevel2.isOn)
            _returnLevel.value = 2;

        uiSave.interactable = true;
	}
}
