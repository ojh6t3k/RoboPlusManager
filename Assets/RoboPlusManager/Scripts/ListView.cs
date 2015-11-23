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
		get
		{
			return _selectedItem;
		}
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
			if(_selectedItem != null)
				_selectedItem.selected = true;

			if(changed == true)
				OnChangedSelection.Invoke();
		}
	}

	public int selectedIndex
	{
		get
		{
			if(_selectedItem == null)
				return -1;

			return _selectedItem.index;
		}
		set
		{
			if(value < 0 || value >= itemCount)
				return;

			selectedItem = itemPanel.transform.GetChild(value).GetComponent<ListItem>();
		}
	}

    public ListItem[] items
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            foreach (Transform item in itemPanel.transform)
            {
                ListItem i = item.GetComponent<ListItem>();
                if(i != null)
                    list.Add(i);
            }

            return list.ToArray();
        }
    }

	public void ClearItem()
	{
		_selectedItem = null;

		List<GameObject> list = new List<GameObject>();
		foreach(Transform item in itemPanel.transform)
			list.Add(item.gameObject);

		for(int i=0; i<list.Count; i++)
			GameObject.DestroyImmediate(list[i]);

		_itemNum = 0;
	}

	public void AddItem(ListItem item)
	{
		if(item == null)
		{
			_createdItem = null;
			return;
		}

		_createdItem = item;
		_createdItem.transform.SetParent(itemPanel.transform);
		_createdItem.transform.localScale = Vector3.one;
		_createdItem.owner = this;
		_itemNum++;
	}

	public void InsertItem(ListItem item)
	{
		if(_selectedItem == null)
			return;

		int index = _selectedItem.index;
		AddItem(item);
		if(_createdItem != null)
			_createdItem.transform.SetSiblingIndex(index);
	}

	public void RemoveItem()
	{
		if(_selectedItem == null)
			return;

		DestroyImmediate(_selectedItem.gameObject);

		_selectedItem = null;
		_itemNum--;
	}

    public void RemoveItem(ListItem item)
    {
        if (_selectedItem == null)
            return;

        if (_selectedItem.Equals(item))
            selectedItem = null;

        DestroyImmediate(item.gameObject);
        _itemNum--;
    }
}
