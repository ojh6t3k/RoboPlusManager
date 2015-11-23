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

    void OnEnable()
    {
        ApplyLanguage();
    }

	public string text
	{
		get
		{
			return _text;
		}
	}


    public void ApplyLanguage()
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
}
