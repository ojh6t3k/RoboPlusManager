using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;


public class ListView : MonoBehaviour
{
	public RectTransform itemPanel;

	public UnityEvent OnChangedSelection;

	private int _itemNum = 0;
	private ListItem _createdItem;
	private ListItem _selectedItem;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public int itemCount
	{
		get
		{
			return itemPanel.transform.childCount;
		}
	}

	public ListItem createdItem
	{
		get
		{
			return _createdItem;
		}
	}

	public ListItem selectedItem
	{
		set
		{
			bool changed = false;
			if(_selectedItem != null)
			{
				if(_selectedItem.Equals(value) == false)
					changed = true;
			}
			else
			{
				if(value != null)
					changed = true;
			}

			if(_selectedItem != null)
				_selectedItem.selected = false;

			_selectedItem = value;

			if(changed == true)
				OnChangedSelection.Invoke();
		}
		get
		{
			return _selectedItem;
		}
	}

	public void ClearItem()
	{
		List<GameObject> list = new List<GameObject>();
		foreach(Transform item in itemPanel.transform)
			list.Add(item.gameObject);

		for(int i=0; i<list.Count; i++)
			GameObject.DestroyImmediate(list[i]);

		_itemNum = 0;
		_selectedItem = null;
	}

	public void AddItem(ListItem item, Sprite image, string text, Object data)
	{
		if(item == null)
		{
			_createdItem = null;
			return;
		}

		_createdItem = GameObject.Instantiate(item);
		_createdItem.transform.SetParent(itemPanel.transform);
		_createdItem.owner = this;
		if(_createdItem.image != null)
		{
			if(image != null)
				_createdItem.image.sprite = image;
		}
		if(_createdItem.text != null)
			_createdItem.text.text = text;
		_createdItem.data = data;
		_itemNum++;
	}

	public void InsertItem(ListItem item, Sprite image, string text, Object data)
	{
		if(_selectedItem == null)
			return;

		int index = _selectedItem.index;
		AddItem(item, image, text, data);
		if(_createdItem != null)
			_createdItem.transform.SetSiblingIndex(index);
	}

	public void RemoveItem()
	{
		if(_selectedItem == null)
			return;

		GameObject.DestroyImmediate(_selectedItem.gameObject);

		_selectedItem = null;
		_itemNum--;
	}
}
