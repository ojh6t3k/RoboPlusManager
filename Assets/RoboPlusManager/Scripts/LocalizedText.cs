using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class LocalizedText : MonoBehaviour
{
	public string key;
	public Text uiText;

	private string _text;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public string text
	{
		get
		{
			return _text;
		}
	}


	public void Refresh()
	{
		if(LocalizationManager.manager == null)
			return;

		_text = LocalizationManager.manager.GetLocalizedText(key);
		Font font = LocalizationManager.manager.font;

		if(uiText != null)
		{
			uiText.text = text;
			if(font != null)
				uiText.font = font;
		}
	}

	public void Refresh(string text, Font font)
	{
		_text = text;

		if(uiText != null)
		{
			uiText.text = text;
			if(font != null)
				uiText.font = font;
		}
	}
}
