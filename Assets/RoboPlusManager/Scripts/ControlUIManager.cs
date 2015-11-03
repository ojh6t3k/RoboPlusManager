using UnityEngine;
using System.Collections;

public class ControlUIManager : MonoBehaviour
{
    public CommProduct commProduct;
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

	public ControlUIInfo selectedUI
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
            {
                ui.active = false;
                ui.commProduct = null;
            }

			if(value != null)
			{
				foreach(ControlUI ui in uiList)
				{
					if(ui.uiClass.Equals(value.uiClass) == true)
					{
						ui.active = true;                        
                        ui.uiInfo = value;
                        ui.commProduct = commProduct;
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
