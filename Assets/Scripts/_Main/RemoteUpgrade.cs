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

    private LocationObject locationObject = null;
    
    public void GetAvailableUpgrades() {
        List<GameObject> allocatedObj = locationManger.allocatedObj;
        Price lowPrice = new Price(0, "z");
        
        locationObject = null;
        for(int i = 0; i<allocatedObj.Count; i++) {
            LocationObject location = allocatedObj[i].GetComponent<LocationObject>();
            if (location == null) continue;
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

        //Has enoguh money
        gameObject.GetComponent<Button>().interactable = money.HasEnoughMoney(locationObject.GetUpgradePrice());
    } 

    public void UpgradeBtnClicked() {
        if(locationObject == null) return;
        if(!money.SubtractMoney(locationObject.GetUpgradePrice())) return;
        
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
        FXManager.Instance.CreateFX(FXType.UpgradeParticleFX, locationObject.gameObject.transform);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }

    void Update()
    {
        if(Time.frameCount % 60 == 0) {
            GetAvailableUpgrades();
        }
    }
}
