using UnityEngine;
using System;
using UnityEngine.UI;


public class RoboPlusManager : MonoBehaviour
{
    [Serializable]
    public class UI
    {
        public Dropdown language;
    }

    public UI ui;


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

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void InitSettings()
    {
        LanguageInfo[] languages = LocalizationManager.manager.languages;
        ui.language.options.Clear();
        for (int i = 0; i < languages.Length; i++)
            ui.language.options.Add(new Dropdown.OptionData(languages[i].name, languages[i].image));

        string currentLanguage = LocalizationManager.manager.currentLanguage;
        int index = 0;
        for (int i = 0; i < languages.Length; i++)
        {
            if (languages[i].iso3code.Equals(currentLanguage))
            {
                index = i;
                break;
            }
        }
        ui.language.value = index;
        ui.language.captionImage.sprite = languages[index].image;
        ui.language.captionText.text = languages[index].name;
        ui.language.captionImage.enabled = true;
    }

    public void ApplySettings()
    {
        LanguageInfo[] languages = LocalizationManager.manager.languages;
        int index = ui.language.value;
        LocalizationManager.manager.currentLanguage = languages[index].iso3code;
        LocalizationManager.manager.ApplyLanguage();
    }
}
