using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     A panel for changing game settings.
/// </summary>
public class SettingsPanel : PanelObject
{
    [SerializeField] private Button[] languageBtns = new Button[6];
    [SerializeField] private Slider bgmVolumeSlider, sfxVolumeSlider;
    [SerializeField] private Localization localize;

    public void ToggleSettingsPanel()
    {
        if (gameObject.activeSelf) ClosePanel();
        else OpenPanel();
    }

    protected override void OnPanelOpen()
    {
        base.OnPanelOpen();

        bgmVolumeSlider.value = PlayerPrefs.GetFloat("settings_bgm_voulume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("settings_sfx_voulume");
        UpdateLanguageBtn();
    }

    public void LanguageChangeBtnClicked(int idx)
    {
        LocaleCode localeCode;
        switch (idx)
        {
            case 0:
                localeCode = LocaleCode.Korean;
                break;
            case 1:
                localeCode = LocaleCode.English;
                break;
            case 2:
                localeCode = LocaleCode.Japanese;
                break;
            case 3:
                localeCode = LocaleCode.ChineseSimplified;
                break;
            case 4:
                localeCode = LocaleCode.ChineseTraditional;
                break;
            case 5:
                localeCode = LocaleCode.Spanish;
                break;
            default:
                localeCode = LocaleCode.English;
                break;
        }

        PlayerPrefs.SetString("settings_localeCode", localeCode.ToString());
        PlayerPrefs.Save();

        localize.LoadLocale();
        UpdateLanguageBtn();
    }

    public void VolumeSliderValueChanged()
    {
        PlayerPrefs.SetFloat("settings_sfx_voulume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("settings_bgm_voulume", bgmVolumeSlider.value);

        AudioManager.Instance.SetVolume();
    }

    private void UpdateLanguageBtn()
    {
        var localeCodeInString = PlayerPrefs.GetString("settings_localeCode");
        var localeCode = (LocaleCode)Enum.Parse(typeof(LocaleCode), localeCodeInString);

        for (var i = 0; i < languageBtns.Length; i++) languageBtns[i].interactable = !(i == (int)localeCode);
    }
}