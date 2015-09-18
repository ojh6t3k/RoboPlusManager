using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ControlUI : MonoBehaviour
{
	private ControlUIInfo _uiInfo;
	private bool _active = false;

	protected virtual void OnUpdateUIInfo() {}


	void Awake()
	{
		active = _active;
	}

	public string uiClass
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

	public ControlUIInfo uiInfo
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
}
