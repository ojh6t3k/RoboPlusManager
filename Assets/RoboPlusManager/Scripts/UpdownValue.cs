using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


public class UpdownValue : MonoBehaviour
{
	public bool interactable = true;
	public InputField inputField;
	public float initValue;
	public float unitValue;
	public float minValue;
	public float maxValue;
	public string format;

	public UnityEvent OnChangedValue;

	private float _value;

    void Awake()
    {
        if (inputField != null)
        {
            inputField.text = initValue.ToString(format);
            inputField.onEndEdit.AddListener(OnEndEdit);
        }

        _value = initValue;
    }

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if(inputField != null)
			inputField.interactable = interactable;
	}

	public void OnIncrease()
	{
		Value = _value + unitValue;
	}

	public void OnDecrease()
	{
		Value = _value - unitValue;
	}

	private void OnEndEdit(string text)
	{
		try
		{
			Value = float.Parse(text);
		}
		catch(Exception e)
		{
			Debug.LogError(e);
		}
	}

	public float Value
	{
		set
		{
			if(interactable == false)
				return;

			float v = Mathf.Clamp(value, minValue, maxValue);
			if(Mathf.Approximately(_value, v))
				return;

			_value = v;
			OnChangedValue.Invoke();

			if(inputField == null)
				return;
			try
			{
				inputField.text = _value.ToString(format);
			}
			catch(Exception e)
			{
				Debug.LogError(e);
			}
		}
		get
		{
			return _value;
		}
	}
}
