using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ListItem : MonoBehaviour, IPointerClickHandler
{
	public ListView owner;
	public Image image;
	public Text text;
	public Object data;

	private Toggle _toggle;

	// Use this for initialization
	void Start ()
	{
		_toggle = GetComponent<Toggle>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public int index
	{
		get
		{
			return this.transform.GetSiblingIndex();
		}
	}

	public bool selected
	{
		set
		{
			_toggle.isOn = value;
		}
		get
		{
			return _toggle.isOn;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_toggle.isOn = true;
		owner.selectedItem = this;
	}
}
