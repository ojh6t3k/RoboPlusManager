using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	public string key;

	private Text _text;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Refresh()
	{
		if(LocalizationManager.manager == null)
			return;

		string text = LocalizationManager.manager.GetLocalizedText(key);
		Font font = LocalizationManager.manager.font;

		if(_text == null)
			_text = GetComponent<Text>();

		if(text != null)
			_text.text = text;

		if(font != null)
			_text.font = font;
	}

	public void Refresh(string text, Font font)
	{
		if(_text == null)
			_text = GetComponent<Text>();

		_text.text = text;
		if(font != null)
			_text.font = font;
	}
}
