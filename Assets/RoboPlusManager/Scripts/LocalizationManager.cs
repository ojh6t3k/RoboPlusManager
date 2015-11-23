using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;


[Serializable]
public class LanguageInfo
{
    public string name;
	public string iso3code;
	public Font font;
	public Sprite image;

	[HideInInspector]
	public Hashtable table;
	[HideInInspector]
	public int index;
}


public class LocalizationManager : MonoBehaviour
{
	public TextAsset table;
	public string newLineText = "{/n}";
	public LanguageInfo[] languages;

	[HideInInspector]
	public static LocalizationManager manager;

	private LanguageInfo _currentLanguage;

	void Awake()
	{
		Load();
		manager = this;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void Load()
	{
		if(table == null)
			return;

		StringReader reader = new StringReader(table.text);
		string line = reader.ReadLine();
		string[] tokens = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
		for(int i=0; i<tokens.Length; i++)
		{
			foreach(LanguageInfo lang in languages)
			{
				if(lang.iso3code.Equals(tokens[i]) == true)
				{
					lang.table = new Hashtable();
					lang.index = i;
				}
			}
		}

		while(true)
		{
			line = reader.ReadLine();
			if(line == null)
				break;

			tokens = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
			foreach(LanguageInfo lang in languages)
			{
				if(lang.table != null)
					lang.table.Add(tokens[0], tokens[lang.index].Replace(newLineText, Environment.NewLine));
			}
		}

		foreach(LanguageInfo lang in languages)
            lang.name = (string)lang.table["Language"];

        if (languages.Length > 0)
            currentLanguage = languages[0].iso3code;

        ApplyLanguage();
    }

	public string currentLanguage
	{
		get
		{
			if(_currentLanguage != null)
				return _currentLanguage.iso3code;

			return null;
		}
		set
		{
			if(value == null)
				return;

			if(_currentLanguage != null)
			{
				if(_currentLanguage.iso3code.Equals(value) == true)
					return;
			}

			foreach(LanguageInfo lang in languages)
			{
				if(lang.iso3code.Equals(value) == true)
				{
					_currentLanguage = lang;
					break;
				}
			}

			if(_currentLanguage == null)
				return;
		}
	}

    public void ApplyLanguage()
    {
        if (_currentLanguage == null)
            return;


        LocalizedText[] textList = GameObject.FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText lt in textList)
            lt.ApplyLanguage();
    }

	public string GetLocalizedText(string key)
	{
		if(_currentLanguage == null)
			return null;

		if(_currentLanguage.table.ContainsKey(key) == false)
			return null;

		return (string)_currentLanguage.table[key];
	}

	public Font font
	{
		get
		{
			if(_currentLanguage == null)
				return null;

			return _currentLanguage.font;
		}
	}
}
