using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
///     Manages remote upgrades for a location object.
/// </summary>
public class RemoteUpgradeManager : MonoBehaviour
{
    [SerializeField] private LocationManger locationManger;
    [SerializeField] private MoneyManager money;

    [SerializeField] private TextMeshProUGUI short_title, short_upgCount, short_price, short_upgName;
    [SerializeField] private Image btn_bg, btn_frame, landmark_icon, lvUp_icon;
    private readonly Color color_Available = new(1, 1, 1, 1);

    private readonly Color color_notAvailable = new(0.8f, 0.8f, 0.8f, 0.5f);

    private LocationObject locationObject;

    private void Start()
    {
        btn_bg.color = color_notAvailable;
        btn_frame.gameObject.SetActive(false);
        lvUp_icon.gameObject.SetActive(false);
        landmark_icon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Time.frameCount % 60 != 0) return;
        GetAvailableUpgrades();
    }

    public void GetAvailableUpgrades()
    {
        var lowPrice = new Price(0, "z");
        var allocatedObj = GetAvailableUpgrades(lowPrice);

        if (locationObject == null)
        {
            NoUpdateAvailableUI();
            return;
        }

        CheckAvailability();
        SetBtnActive();
    }

    private void NoUpdateAvailableUI()
    {
        short_title.text = "가능 업그레이드 없음";
        short_price.text = "";
        short_upgCount.text = "";
        short_upgName.text = "";

        //set btns
        btn_bg.color = color_notAvailable;
        btn_frame.gameObject.SetActive(false);
        lvUp_icon.gameObject.SetActive(false);
        landmark_icon.gameObject.SetActive(false);
    }

    private List<LandmarkController> GetAvailableUpgrades(Price lowPrice)
    {
        var allocatedObj = locationManger.AllocatedObj;
        locationObject = null;
        for (var i = 0; i < allocatedObj.Count; i++)
        {
            var location = allocatedObj[i].GetComponent<LocationObject>();
            if (location == null) continue;
            if (location.buildCompleteTime > DateTime.Now) continue;
            var upgradePrice = location.GetUpgradePrice();
            if (upgradePrice.amount == -1) continue;

            if (lowPrice.idx > upgradePrice.idx ||
                (lowPrice.idx == upgradePrice.idx && lowPrice.amount > upgradePrice.amount))
            {
                lowPrice = upgradePrice;
                locationObject = location;
            }
        }

        return allocatedObj;
    }

    private void CheckAvailability()
    {
        var landmarkController = locationObject.gameObject.GetComponent<LandmarkController>();
        short_title.text =
            GetLocalizedString("Names",
                "title_" + landmarkController.GetComponent<LocationObject>().ScriptableObjet.ModelID) + "-" +
            landmarkController.GetComponent<LocationObject>().CopyN;
        short_price.text = locationObject.GetUpgradePrice().GetString();
        short_upgCount.text = locationObject.UpgradeStatus.ToString();

        if (locationObject.ReadyForLevelUp())
            short_upgName.text = "레벨업!";
        else short_upgName.text = "";
        SetButtonActive(landmarkController);

        gameObject.GetComponent<Button>().interactable = money.HasEnoughMoney(locationObject.GetUpgradePrice());
    }

    private void SetButtonActive(LandmarkController landmarkController)
    {
        landmark_icon.sprite = InfoDataManager.Instance
            .GetLandmarkItemByID(landmarkController.GetComponent<LocationObject>().ScriptableObjet.ModelID).Img;
        btn_frame.gameObject.SetActive(true);
        lvUp_icon.gameObject.SetActive(true);
        landmark_icon.gameObject.SetActive(true);
    }

    public void UpgradeBtnClicked()
    {
        if (locationObject == null)
        {
            GetAvailableUpgrades();
            return;
        }

        if (locationObject.GetComponent<LandmarkController>().buildCompleteTime > DateTime.Now)
        {
            GetAvailableUpgrades();
            return;
        }

        if (!money.SubtractMoney(locationObject.GetUpgradePrice())) return;

        if (locationObject.ReadyForLevelUp())
        {
            locationManger.LevelUPLandmark(locationObject.gameObject);
            return;
        }

        locationObject.UpgradeStatus += 1;

        GetAvailableUpgrades();
        if (locationObject == null) return;

        locationObject.gameObject.GetComponent<LandmarkController>().UpdateData();
        SetupAnimationAndFX();

        //tutorial 05
        QuestTutorialManager.Instance.Addfarm0UpdateCount();
        QuestTutorialManager.Instance.AddFastUpgradeClickCount();
    }

    private void SetupAnimationAndFX()
    {
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.Upgrade);
        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.transform.DOShakeScale(0.3f);
        FXManager.Instance.CreateFX(FXType.UpgradeParticleFX, locationObject.gameObject.transform,
            locationObject.ScriptableObjet.Width == 2 ? 4f : 5.5f);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }

    public void SetBtnActive()
    {
        if (money.HasEnoughMoney(locationObject.GetUpgradePrice()))
        {
            btn_bg.color = color_Available;
            btn_frame.color = color_Available;
            landmark_icon.color = color_Available;
            lvUp_icon.color = color_Available;
        }
        else
        {
            btn_bg.color = color_notAvailable;
            btn_frame.color = color_notAvailable;
            landmark_icon.color = color_notAvailable;
            lvUp_icon.color = color_notAvailable;
        }
    }
}