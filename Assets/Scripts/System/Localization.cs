using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public enum LocaleCode
{
    Korean,
    English,
    Japanese,
    ChineseSimplified,
    ChineseTraditional,
    Spanish
}

/// <summary>
///     Manages the localization of the game.
/// </summary>
public class Localization : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset font_kr, font_jp, font_zh_simplified, font_zh_traditional, font_es;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("settings_localeCode"))
        {
            LocaleCode localeCode;

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Korean:
                    localeCode = LocaleCode.Korean;
                    break;

                case SystemLanguage.English:
                    localeCode = LocaleCode.English;
                    break;

                case SystemLanguage.Japanese:
                    localeCode = LocaleCode.Japanese;
                    break;

                case SystemLanguage.Chinese:
                    localeCode = LocaleCode.ChineseSimplified;
                    break;

                case SystemLanguage.ChineseSimplified:
                    localeCode = LocaleCode.ChineseSimplified;
                    break;

                case SystemLanguage.ChineseTraditional:
                    localeCode = LocaleCode.ChineseTraditional;
                    break;

                case SystemLanguage.Spanish:
                    localeCode = LocaleCode.Spanish;
                    break;

                default:
                    localeCode = LocaleCode.English;
                    break;
            }

            PlayerPrefs.SetString("settings_localeCode", localeCode.ToString());
            PlayerPrefs.Save();
        }

        LoadLocale();
    }

    public void LoadLocale()
    {
        var localeCodeInString = PlayerPrefs.GetString("settings_localeCode");
        print(localeCodeInString);


        //Change localizationSettings
        var localeCode = new LocaleIdentifier(localeCodeInString);
        for (var i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            var anIdentifier = aLocale.Identifier;
            if (anIdentifier == localeCode) LocalizationSettings.SelectedLocale = aLocale;
        }

        //Change fonts
        var texts = FindObjectsOfType<TextMeshProUGUI>(true);
        TMP_FontAsset font;
        var localeCodeEnum = (LocaleCode)Enum.Parse(typeof(LocaleCode), localeCodeInString);

        switch (localeCodeEnum)
        {
            case LocaleCode.Korean:
                font = font_kr;
                break;
            case LocaleCode.English:
                font = font_kr;
                break;
            case LocaleCode.Japanese:
                font = font_jp;
                break;
            case LocaleCode.ChineseSimplified:
                font = font_zh_simplified;
                break;
            case LocaleCode.ChineseTraditional:
                font = font_zh_traditional;
                break;
            case LocaleCode.Spanish:
                font = font_es;
                break;
            default:
                font = font_kr;
                break;
        }

        foreach (var text in texts)
            if (text.gameObject.tag != "fixedLocale")
                text.font = font;
    }
}