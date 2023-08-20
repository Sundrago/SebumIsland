using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using System;

public enum LocaleCode { Korean, English, Japanese, ChineseSimplified, ChineseTraditional, Spanish };
public class Localization : MonoBehaviour
{
    [SerializeField] TMP_FontAsset font_kr, font_jp, font_zh_simplified, font_zh_traditional, font_es;

    private void Start()
    {
        //Get LocaleInfo from system language if first launch
        if(!PlayerPrefs.HasKey("settings_localeCode"))
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
        string localeCodeInString = PlayerPrefs.GetString("settings_localeCode");
        print(localeCodeInString);


        //Change localizationSettings
        LocaleIdentifier localeCode = new LocaleIdentifier(localeCodeInString);
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;
            if (anIdentifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = aLocale;
            }
        }

        //Change fonts
        TextMeshProUGUI[] texts = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
        TMP_FontAsset font;
        LocaleCode localeCodeEnum = (LocaleCode)Enum.Parse(typeof(LocaleCode), localeCodeInString);

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
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.tag != "fixedLocale")
                text.font = font;
        }
    }
}
