using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ListItem : MonoBehaviour, IPointerClickHandler
{
	public ListView owner;
	public Image image;
	public Text[] textList;
	public System.Object data;

	private Toggle _toggle;

	void Awake()
	{
	}

	// Use this for initialization
	void Start ()
	{
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

	// This property is for ListView
	public bool selected
	{
		get
		{
			if(_toggle == null)
				_toggle = GetComponent<Toggle>();

			return _toggle.isOn;
		}
		set
		{
			if(_toggle == null)
				_toggle = GetComponent<Toggle>();

			_toggle.isOn = value;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		owner.selectedItem = this;
	}
}
