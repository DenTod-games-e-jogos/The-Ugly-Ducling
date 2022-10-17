using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField]
    Dropdown languageDropdown;

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        
        PopulateLanguageDropdown();
    }

    void PopulateLanguageDropdown()
    {
        var options = new List<Dropdown.OptionData>();

        int selected = 0;

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];

            if (LocalizationSettings.SelectedLocale == locale)
            {
                selected = i;
            }

            options.Add(new Dropdown.OptionData(locale.name));
        }

        languageDropdown.options = options;

        languageDropdown.value = selected;

        languageDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}