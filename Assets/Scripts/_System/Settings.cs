using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Settings : MonoBehaviour
{
    [SerializeField] Button[] languageBtns = new Button[6];
    [SerializeField] Slider bgmVolumeSlider, sfxVolumeSlider;
    [SerializeField] Localization localize;

    public void ToggleSettingsPanel()
    {
        if (gameObject.activeSelf) Hide();
        else Show();
    }

    public void Show()
    {
        if (gameObject.activeSelf) return;
        PanelManager.Instance.CloseOtherPanels(gameObject);

        //SHOW ANIM
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;

        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(-2500, -500, 0), 0.5f)
            .SetEase(Ease.OutExpo)
            .From();
        gameObject.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.5f)
            .SetEase(Ease.OutBack)
            .From();

        //Init Btns
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("settings_bgm_voulume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("settings_sfx_voulume");
        UpdateLanguageBtn();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        //HIDE ANIM
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;

        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(2500, 500, 0), 0.5f)
            .SetEase(Ease.InQuint);
        gameObject.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void LanguageChangeBtnClicked(int idx)
    {
        LocaleCode localeCode;

        //change language
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

        //update btnActiveness
        UpdateLanguageBtn();
    }

    public void VolumeSliderValueChanged()
    {
        PlayerPrefs.SetFloat("settings_sfx_voulume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("settings_bgm_voulume", bgmVolumeSlider.value);

        AudioCtrl.Instance.SetVolume();
    }

    private void UpdateLanguageBtn()
    {
        string localeCodeInString = PlayerPrefs.GetString("settings_localeCode");
        LocaleCode localeCode = (LocaleCode)Enum.Parse(typeof(LocaleCode), localeCodeInString);

        for(int i=0; i<languageBtns.Length; i++)
        {
            languageBtns[i].interactable = !(i == (int)localeCode);
        }
    }
}
