using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using TMPro;
using DG.Tweening;

public class NewUpgPanel : MonoBehaviour
{
    [SerializeField] AudioCtrl myAudio;
    [SerializeField] RemoteUpgrade remoteUpgrade;
    [SerializeField] GameObject bg_btn;

    public TextMeshProUGUI title_ui, level_ui, btn_ui, info1, info2;
    public Slider slider;
    private Price levelUpPrice = new Price();
    public GameObject targetLandmark;

    public GameObject upgrade_btn_ui;

    public LocationManger locationManger;

    public Vector2 targetPos;
    LocationObject locationObject;

    public MoneyUI money;

    public void UpgradeBtnClicked()
    {
        if(targetLandmark == null) return;
        if(!money.SubtractMoney(locationObject.GetUpgradePrice())) return;

        if(locationObject.ReadyForLevelUp()) {
            targetLandmark = locationManger.LevelUPLandmark(targetLandmark);
            OpenPanel(targetLandmark);
            remoteUpgrade.GetAvailableUpgrades();
            return;
        }

        locationObject.upgradeStatus += 1;
        UpdateUI();
        targetLandmark.GetComponent<Landmark>().UpdateData();

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.upgrade);
        if (DOTween.IsTweening(upgrade_btn_ui.transform)) DOTween.Kill(upgrade_btn_ui.transform);
        upgrade_btn_ui.transform.localScale = new Vector3(1f, 1f, 1f);
        upgrade_btn_ui.transform.DOShakeScale(0.3f);

        //Instantiate particle
        FXManager.Instance.CreateFX(FXType.UpgradeParticleFX, gameObject.transform);

        remoteUpgrade.GetAvailableUpgrades();
        CheckEnoughMoney();

        //tutorial 03
        if (locationObject.landMarkID == "farm0-0")
        {
            QuestTutorialManager.Instance.Addfarm0UpdateCount();
        }
    }

    void UpdateUI()
    {
        level_ui.text = "레벨 " + (locationObject.upgradeStatus + 1);
        if(DOTween.IsTweening(slider)) DOTween.Kill(slider);
        slider.DOValue((float)locationObject.upgradeStatus / (float) (locationObject.maxUpdateIdx-1), 0.2f);
        
        levelUpPrice = locationObject.GetUpgradePrice();
        btn_ui.text = levelUpPrice.GetString();

        if(locationObject.ReadyForLevelUp()) {
            upgrade_btn_ui.GetComponent<Image>().color = Color.yellow;
            info1.text = "레벨업 준비";
        } else {
            upgrade_btn_ui.GetComponent<Image>().color = Color.white;

            float PriceMultiplier = locationObject.data.data[locationObject.upgradeStatus].value;
            float SpeedMultiplier = locationObject.data.data[locationObject.upgradeStatus].speed;
            info1.text = "" + new Price(Mathf.RoundToInt(locationObject.defaultPrice.amount * PriceMultiplier / 100f), locationObject.defaultPrice.charCode).GetString();
            info2.text = "" + locationObject.defaultGrowTime * SpeedMultiplier / 100f;
        }
    }

    public void OpenPanel(GameObject landmark)
    {
        PanelManager.Instance.CloseOtherPanels(gameObject);
        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().ResetTrigger("shrink");
        gameObject.GetComponent<Animator>().SetTrigger("grow");
        targetLandmark = landmark;
        locationObject = landmark.GetComponent<LocationObject>();

        title_ui.text = GetLocalizedString("Landmark", landmark.GetComponent<LocationObject>().modelID + "_title");

        Vector3 worldPoint = targetLandmark.transform.position;
        worldPoint.y += 10f;
        targetPos = Camera.main.WorldToScreenPoint(worldPoint);
        gameObject.transform.position = targetPos;

        UpdateUI();
        CheckEnoughMoney();
        bg_btn.SetActive(true);
    }

    public void ClosePanel()
    {
        if (!gameObject.activeSelf) return;
        gameObject.GetComponent<Animator>().ResetTrigger("grow");
        gameObject.GetComponent<Animator>().SetTrigger("shrink");
        //gameObject.SetActive(false);
        targetLandmark = null;
        bg_btn.SetActive(false);
        //targetPos = Vector2.zero;
    }

    public void DeactiveSelf()
    {
        gameObject.SetActive(false);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }

    public void Update()
    {
        if(targetLandmark != null)
        {
            Vector3 worldPoint = targetLandmark.transform.position;
            worldPoint.y += 12f;
            targetPos = Camera.main.WorldToScreenPoint(worldPoint);
            targetPos.y += Mathf.Lerp(-100f, 250f, Mathf.InverseLerp(5, 65, Camera.main.orthographicSize));
            if(Vector2.Distance(targetPos, gameObject.transform.position) > 0.1f)
            {
                Vector2 newPos = gameObject.transform.position;
                newPos.x += (targetPos.x - newPos.x) / 2f;
                newPos.y += (targetPos.y - newPos.y) / 2f;
                gameObject.transform.position = newPos;
            }

            // Deactivate if not enough money
            if(Time.frameCount % 30 == 0) {
                CheckEnoughMoney();
            }
        }
    }

    // Deactivate if not enough money
    private void CheckEnoughMoney() {
        if(targetLandmark == null) return;

        if (targetLandmark.GetComponent<Landmark>().isBuilding)
        {
            upgrade_btn_ui.GetComponent<Button>().interactable = false;
            return;
        }
        
        if(locationObject.upgradeStatus == locationObject.maxUpdateIdx - 1) {
            if(locationObject.levelUpTime == -1) {
                upgrade_btn_ui.GetComponent<Button>().interactable = false;
                btn_ui.text = "max";
                return;
            }
            upgrade_btn_ui.GetComponent<Button>().interactable = money.HasEnoughMoney(locationObject.levelUpPrice);
        } else {
            upgrade_btn_ui.GetComponent<Button>().interactable = money.HasEnoughMoney(levelUpPrice);
        }
        
    }

    public void MoveBtnClicked()
    {
        if(targetLandmark == null) return;
        Camera.main.GetComponent<PinchZoom>().StartCamTransition(Camera.main.transform.position, 50);
        locationManger.MoveBtnClicked(targetLandmark);
        ClosePanel();
    }

    public void BgBtnClicked() {
        ClosePanel();
    }

    // 5-65 
    float SuperLerp (float from, float to, float from2, float to2, float value) {
        if (value <= from2)
            return from;
        else if (value >= to2)
            return to;
        return (to - from) * ((value - from2) / (to2 - from2)) + from;
    }

}