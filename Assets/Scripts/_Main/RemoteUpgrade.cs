using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class RemoteUpgrade : MonoBehaviour
{
    [SerializeField] LocationManger locationManger;
    [SerializeField] MoneyUI money;
    [SerializeField] TextMeshProUGUI short_title, short_upgCount, short_price, short_upgName;

    [SerializeField] Image btn_bg, btn_frame, landmark_icon, lvUp_icon;

    Color color_notAvailable = new Color(0.8f, 0.8f, 0.8f, 0.5f);
    Color color_Available = new Color(1, 1, 1, 1);

    private LocationObject locationObject = null;

    private void Start()
    {
        btn_bg.color = color_notAvailable;
        btn_frame.gameObject.SetActive(false);
        lvUp_icon.gameObject.SetActive(false);
        landmark_icon.gameObject.SetActive(false);
    }

    public void GetAvailableUpgrades() {
        List<GameObject> allocatedObj = locationManger.allocatedObj;
        Price lowPrice = new Price(0, "z");
        
        locationObject = null;
        for(int i = 0; i<allocatedObj.Count; i++) {
            LocationObject location = allocatedObj[i].GetComponent<LocationObject>();
            if (location == null) continue;
            if (location.GetComponent<Landmark>().buildCompleteTime > System.DateTime.Now) continue;
            Price upgradePrice = location.GetUpgradePrice();

            if( (lowPrice.idx > upgradePrice.idx) || (lowPrice.idx == upgradePrice.idx && lowPrice.amount > upgradePrice.amount)) {
                lowPrice = upgradePrice;
                locationObject = location;
            } 
        }
        
        //No available upgrade
        if(locationObject == null) {
            short_title.text = "가능 업그레이드 없음";
            short_price.text = "";
            short_upgCount.text = "";
            short_upgName.text = "";

            //set btns
            btn_bg.color = color_notAvailable;
            btn_frame.gameObject.SetActive(false);
            lvUp_icon.gameObject.SetActive(false);
            landmark_icon.gameObject.SetActive(false);
            return;
        }

        //Has available upgrade
        Landmark landmark = locationObject.gameObject.GetComponent<Landmark>();
        short_title.text = GetLocalizedString("Names", "title_" + landmark.GetComponent<LocationObject>().modelID) + "-" + (landmark.GetComponent<LocationObject>().copyN).ToString();
        short_price.text = locationObject.GetUpgradePrice().GetString();
        short_upgCount.text = locationObject.upgradeStatus.ToString();

        if(locationObject.ReadyForLevelUp()) {
            short_upgName.text = "레벨업!";
        } else short_upgName.text = "";


        //set btns
        landmark_icon.sprite = InfoDataManager.Instance.GetLandmarkItemByID(landmark.GetComponent<LocationObject>().modelID).Img;
        btn_frame.gameObject.SetActive(true);
        lvUp_icon.gameObject.SetActive(true);
        landmark_icon.gameObject.SetActive(true);

        //Has enoguh money
        gameObject.GetComponent<Button>().interactable = money.HasEnoughMoney(locationObject.GetUpgradePrice());

        SetBtnActive();
    } 

    public void UpgradeBtnClicked() {
        if (locationObject == null)
        {
            GetAvailableUpgrades();
            return;
        }

        if (locationObject.GetComponent<Landmark>().buildCompleteTime > System.DateTime.Now)
        {
            GetAvailableUpgrades();
            return;
        }

        if (!money.SubtractMoney(locationObject.GetUpgradePrice())) return;
        
        if(locationObject.ReadyForLevelUp()) {
            locationManger.LevelUPLandmark(locationObject.gameObject);
            return;
        } 

        locationObject.upgradeStatus += 1;
        GetAvailableUpgrades();
        locationObject.gameObject.GetComponent<Landmark>().UpdateData();

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.upgrade);

        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.transform.DOShakeScale(0.3f);

        //Instantiate particle
        FXManager.Instance.CreateFX(FXType.UpgradeParticleFX, locationObject.gameObject.transform, locationObject.width == 2 ? 4f : 5.5f);

        //tutorial 05
        QuestTutorialManager.Instance.Addfarm0UpdateCount();
        QuestTutorialManager.Instance.AddFastUpgradeClickCount();
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }

    public void SetBtnActive()
    {
        if(money.HasEnoughMoney(locationObject.GetUpgradePrice()))
        {
            btn_bg.color = color_Available;
            btn_frame.color = color_Available;
            landmark_icon.color = color_Available;
            lvUp_icon.color = color_Available;
        } else
        {
            btn_bg.color = color_notAvailable;
            btn_frame.color = color_notAvailable;
            landmark_icon.color = color_notAvailable;
            lvUp_icon.color = color_notAvailable;
        }
    }

    private void Update()
    {
        if (Time.frameCount % 60 != 0) return;
        GetAvailableUpgrades();
        return;
    }
}
