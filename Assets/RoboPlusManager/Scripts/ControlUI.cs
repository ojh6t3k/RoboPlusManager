using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControlUI : UIBehaviour
{
	public RectTransform uiReset;
	public RectTransform uiSave;

	protected int defaultValue;
	protected float unit;

	private ControlItemInfo _uiInfo;
	private bool _active = false;

	public virtual void OnUpdateUIInfo() {}
	public virtual int Value
	{
		get
		{
			return 0;
		}
		set	{}
	}
	public virtual void Reset()	{}


	void Awake()
	{
		active = _active;
	}

	public string uiName
	{
		get
		{
			return this.GetType().Name;
		}
	}

	public bool active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
			this.gameObject.SetActive(_active);
		}
	}

	public ControlItemInfo uiInfo
	{
		get
		{
			return _uiInfo;
		}
		set
		{
			_uiInfo = value;
			OnUpdateUIInfo();
		}
	}

	public void SaveROM()
	{

	}
}
