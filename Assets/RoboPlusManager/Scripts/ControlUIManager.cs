using UnityEngine;
using System.Collections;

public class ControlUIManager : MonoBehaviour
{
	public ControlUI defaultUI;
	public ControlUI[] uiList;

	private ControlUI _selectedUI;

	// Use this for initialization
	void Start ()
	{
		selectedUI = null;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public ControlItemInfo selectedUI
	{
		get
		{
			if(_selectedUI == null)
				return null;

			return _selectedUI.uiInfo;
		}
		set
		{
			_selectedUI = null;
			defaultUI.active = false;
			foreach(ControlUI ui in uiList)
				ui.active = false;

			if(value != null)
			{
				foreach(ControlUI ui in uiList)
				{
					if(ui.uiName.Equals(value.uiName) == true)
					{
						ui.active = true;
						ui.uiInfo = value;
						_selectedUI = ui;
						break;
					}
				}
			}

			if(_selectedUI == null)
			{
				defaultUI.active = true;
				defaultUI.uiInfo = value;
			}
		}
	}
}
