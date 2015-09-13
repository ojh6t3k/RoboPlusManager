using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class CommunicationUI : ControlUI
{
	public UpdownValue uiBaudrate;
	public UpdownValue uiID;
	public UpdownValue uiReturnDelay;
	public ListView uiReturnLevel;
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
		uiBaudrate.OnChangedValue.AddListener(DisplayBPS);
		uiReturnDelay.OnChangedValue.AddListener(DisplayDelayTime);
	}
	
	public override void OnUpdateUIInfo()
	{
		ControlUIInfo info = uiInfo;
		
		_baudrate = info.GetUIItem("Baudrate");
		_id = info.GetUIItem("ID");
		_returnDelay = info.GetUIItem("ReturnDelay");
		_returnLevel = info.GetUIItem("ReturnLevel");

		uiBaudrate.minValue = 0;
		uiBaudrate.maxValue = 254;
		uiBaudrate.unitValue = 1;
		uiBaudrate.format = "f0";
		uiBaudrate.initValue = (float)_baudrate.defaultValue;
		uiBaudrate.Value = (float)_baudrate.defaultValue;
		DisplayBPS();

		uiID.minValue = 0;
		uiID.maxValue = 253;
		uiID.unitValue = 1;
		uiID.format = "f0";
		uiID.initValue = (float)_id.defaultValue;
		uiID.Value = (float)_id.defaultValue;

		uiReturnDelay.minValue = 0;
		uiReturnDelay.maxValue = 254;
		uiReturnDelay.unitValue = 1;
		uiReturnDelay.format = "f0";
		uiReturnDelay.initValue = (float)_returnDelay.defaultValue;
		uiReturnDelay.Value = (float)_returnDelay.defaultValue;
		DisplayDelayTime();
	}

	private void DisplayBPS()
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

		uiBPS.text = dispText;
	}

	private void DisplayDelayTime()
	{
		uiDelayTime.text = string.Format("{0:f0} usec", uiReturnDelay.Value * 2f);
	}
}
