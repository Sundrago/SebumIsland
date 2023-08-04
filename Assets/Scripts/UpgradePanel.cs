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

public class UpgradePanel : MonoBehaviour
{
//     public AudioCtrl myAudio;
//     public GameObject balloon;

//     public Button[] upgradeBtns_ui;
//     public Image[] upgradeBtns_image_ui;
//     public TextMeshProUGUI[] upgradeBtns_count_ui;
//     public Image[] upgradeBtns_coung_image_ui;
//     public GameObject[] avilablemark_ui;

//     public Image upgrade_img_ui;
//     public TextMeshProUGUI upgrade_title_ui;
//     public TextMeshProUGUI upgrade_descr_ui;
//     public TextMeshProUGUI upgrade_info_ui;
//     public TextMeshProUGUI upgrade_price_ui;
//     public Button upgrade_btn_ui;

//     public TextMeshProUGUI landmark_title_ui;
//     public TextMeshProUGUI landmark_descr_ui;

//     public Button levelUp_btn_ui;
//     public TextMeshProUGUI levelUp_price_ui;

//     public GameObject targetLandmark = null;

//     public PigiInfoPanel pigiInfoPanel;
//     public CSVReader CSV;
//     public GameObject particleFx;
//     public GameObject cone_indicator;
//     public LandmarkPlaceSelector placeSelector;
//     public ParamsData paramsData;

//     int currentSubBtnSelected = -1;

//     public TextMeshProUGUI short_title, short_upgName, short_upgCount, short_price;

//     private Price upgradePrice;

//     private Landmark short_landmark = null;
//     private int short_upgIdx = -1;
//     private bool started = false;

//     private float camZoom;
//     private Vector3 camPos;

//     public PinchZoom pinch;

//     public GameObject pos_info_top, pos_info_btm, pos_short_upg, pos_short_pigi, pos_short_btm;
//     public GameObject upgShort;
//     public GameObject fastUpgradeBtnHolder;

//     public LocationManger locationManger;

//     public Slider mainSlider;

//     private bool activeSelf;

//     // Start is called before the first frame update
//     private void Start()
//     {
//         if (started) return;

//         activeSelf = gameObject.activeSelf;
//         if (targetLandmark == null)
//         {
//             ClosePanel(false);
//             fastUpgradeBtnHolder.transform.DOMoveY(pos_short_btm.transform.position.y, 1.5f)
//                 .SetEase(Ease.OutExpo);
//         }
//             UpdateUpgradeShortcut();
//         started = true;
//     }

//     private void Update()
//     {
//         if (Time.frameCount % 60 == 1)
//         {
//             UpdateUpgradeShortcut();
//             if (short_landmark != null & short_upgIdx != -1)
//             {
//                 //print(short_landmark.upgradePrice[short_upgIdx].GetString());
//                 //print(Camera.main.GetComponent<MoneyUI>().HasEnoughMoney(short_landmark.upgradePrice[short_upgIdx]));
//                 upgShort.GetComponent<Button>().interactable = Camera.main.GetComponent<MoneyUI>().HasEnoughMoney(short_landmark.upgradePrice[short_upgIdx]);
//             }
//             if (targetLandmark != null) CheckAvailability();
//         }
//     }

//     public void UpgradeSubBtnClicked(int idx)
//     {
//         if (currentSubBtnSelected == idx)
//         {
//             UpgradBtnClicked();
//         }

//         UpdateSubBtnUI(idx);
//         currentSubBtnSelected = idx;
//         CheckAvailability();
//     }

//     private void UpdateSubBtnUI(int idx)
//     {
//         int status = targetLandmark.GetComponent<Landmark>().upgradeStatus[idx];
//         UpgradeDataList upgData = targetLandmark.GetComponent<Landmark>().upgData;

//         switch (idx)
//         {
//             case 0:
//                 upgrade_title_ui.text = GetLocalizedString("UpgradeUI", "AddPigi");
//                 upgrade_descr_ui.text = GetLocalizedString("UpgradeUI", "AddPigi_descr");

//                 if (upgData.data[status + 1].Amount == -1)
//                     upgrade_info_ui.text = upgData.data[status].Amount.ToString();
//                 else
//                     upgrade_info_ui.text = upgData.data[status].Amount + "+" + (upgData.data[status + 1].Amount - upgData.data[status].Amount);
//                 break;
//             case 1:
//                 upgrade_title_ui.text = GetLocalizedString("UpgradeUI", "UpgradePrice");
//                 upgrade_descr_ui.text = GetLocalizedString("UpgradeUI", "UpgradePrice_descr");

//                 if (upgData.data[status + 1].Price == -1)
//                     upgrade_info_ui.text = upgData.data[status].Price + "%";
//                 else
//                     upgrade_info_ui.text = upgData.data[status].Price + "% +" + Mathf.Round((upgData.data[status + 1].Price - upgData.data[status].Price) * 10) / 10 + "%";
//                 break;
//             case 2:
//                 upgrade_title_ui.text = GetLocalizedString("UpgradeUI", "UpgradeTime");
//                 upgrade_descr_ui.text = GetLocalizedString("UpgradeUI", "UpgradeTime_descr");

//                 if (upgData.data[status + 1].Speed == -1)
//                     upgrade_info_ui.text = upgData.data[status].Speed + "%";
//                 else
//                     upgrade_info_ui.text = upgData.data[status].Speed + "% " + Mathf.Round((upgData.data[status + 1].Speed - upgData.data[status].Speed) * 10) / 10f + "%";
//                 break;
//             case 3:
//                 upgrade_title_ui.text = GetLocalizedString("UpgradeUI", "UpgradeBonus");
//                 upgrade_descr_ui.text = GetLocalizedString("UpgradeUI", "UpgradeBonus_descr");

//                 if (upgData.data[status + 1].Speed == -1)
//                     upgrade_info_ui.text = upgData.data[status].Speed + "%";
//                 else
//                     upgrade_info_ui.text = upgData.data[status].Bonus_rate + "% 확률로" + upgData.data[status + 1].Bonus_amount + "%를 추가로 획득합니다.";
//                 break;
//             case 4:
//                 upgrade_title_ui.text = GetLocalizedString("UpgradeUI", "UpgradeAuto");
//                 upgrade_descr_ui.text = GetLocalizedString("UpgradeUI", "UpgradeAuto_descr");

//                 if (upgData.data[status + 1].Amount == -1)
//                     upgrade_info_ui.text = upgData.data[status].Auto.ToString();
//                 else
//                     upgrade_info_ui.text = upgData.data[status].Auto + "+" + (upgData.data[status + 1].Auto - upgData.data[status].Auto);
//                 break;
//             default:
//                 print($"Boudary Error on UpgradeSubBtnClicked | idx : " + idx);
//                 return;
//         }

//         for (int i = 0; i < 5; i++)
//         {
//             Color defaultColor = Color.white;
//             defaultColor.a = 0.2f;

//             if (i == idx) upgradeBtns_ui[i].gameObject.GetComponent<Image>().color = Color.white;
//             else upgradeBtns_ui[i].gameObject.GetComponent<Image>().color = defaultColor;
//         }
//         upgrade_img_ui.sprite = upgradeBtns_image_ui[idx].sprite;
//     }

//     Price GetUpgradePrice(int idx, GameObject landmark = null)
//     {
//         if (landmark == null) landmark = targetLandmark;

//         UpgradeDataList upgData = landmark.GetComponent<Landmark>().upgData;
//         int status = landmark.GetComponent<Landmark>().upgradeStatus[idx];
//         switch (idx)
//         {
//             case 0:
//                 return(upgData.data[status + 1].Amount_price);
//             case 1:
//                 return (upgData.data[status + 1].Price_price);
//             case 2:
//                 return (upgData.data[status + 1].speed_price);
//             case 3:
//                 return (upgData.data[status + 1].Bonus_price);
//             case 4:
//                 return (upgData.data[status + 1].Auto_price);
//         }

//         print("Not vailid idx of " + idx + " on GetUpgradePrice");
//         return new Price();
//     }

//     private static string GetLocalizedString(string table, string name)
//     {
//         return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
//     }

//     public void OpenPanel(GameObject landmark)
//     {
//         //set anim
//         if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
//         Vector3 pos = gameObject.transform.position;
//         pos.y = pos_info_btm.transform.position.y;
//         gameObject.transform.position = pos;
//         gameObject.transform.DOMoveY(pos_info_top.transform.position.y, 1f)
//             .SetEase(Ease.OutExpo);

//         if (DOTween.IsTweening(fastUpgradeBtnHolder)) DOTween.Kill(fastUpgradeBtnHolder);
//         fastUpgradeBtnHolder.transform.DOMoveY(pos_short_upg.transform.position.y, 1f)
//             .SetEase(Ease.OutExpo);

//         //store cam
//         camPos = Camera.main.transform.position;
//         camZoom = Camera.main.orthographicSize;

//         gameObject.SetActive(true);

//         targetLandmark = landmark;
//         if(currentSubBtnSelected == -1)
//         {
//             UpgradeSubBtnClicked(0);
//         }

//         CheckAvailability();
//         pigiInfoPanel.ClosePanel(false);

//         cone_indicator.transform.position = landmark.transform.position;
//         cone_indicator.SetActive(true);

//         landmark_title_ui.text = GetLocalizedString("Names", "title_"+landmark.GetComponent<LocationObject>().modelID) + "-" + (landmark.GetComponent<LocationObject>().copyN).ToString();
//         landmark_descr_ui.text = GetLocalizedString("Names", "descr_"+landmark.GetComponent<LocationObject>().modelID);

//         activeSelf = true;
// }

//     public void ClosePanel(bool restoreCam = true)
//     {
//         if (!activeSelf) return;

//         activeSelf = false;
//         //set anim
//         if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
//         Vector3 pos = gameObject.transform.position;
//         pos.y = pos_info_top.transform.position.y;
//         gameObject.transform.position = pos;
//         gameObject.transform.DOMoveY(pos_info_btm.transform.position.y, 1.5f)
//             .SetEase(Ease.OutExpo)
//             .OnComplete(SetActive);


//         //restore cam
//         if (!restoreCam) return;
//         Camera.main.GetComponent<PinchZoom>().StartCamTransition(camPos, camZoom);

//         if (DOTween.IsTweening(fastUpgradeBtnHolder)) DOTween.Kill(fastUpgradeBtnHolder);
//         fastUpgradeBtnHolder.transform.DOMoveY(pos_short_btm.transform.position.y, 1.5f)
//             .SetEase(Ease.OutExpo);
//     }

//     private void SetActive()
//     {
//         targetLandmark = null;
//         //gameObject.SetActive(false);
//         cone_indicator.SetActive(false);
//     }

//     public void UpgradBtnClicked()
//     {
//         if (!Camera.main.GetComponent<MoneyUI>().SubtractMoney(upgradePrice)) return;
//         if (upgradePrice.amount == -1) return;

//         myAudio.PlaySFX(1);
//         if (DOTween.IsTweening(upgrade_btn_ui.transform)) DOTween.Kill(upgrade_btn_ui.transform);
//         upgrade_btn_ui.transform.localScale = new Vector3(1f, 1f, 1f);
//         upgrade_btn_ui.transform.DOShakeScale(0.3f);
//         targetLandmark.GetComponent<Landmark>().upgradeStatus[currentSubBtnSelected] += 1;
//         targetLandmark.GetComponent<Landmark>().SetupPigi();

//         GameObject particle = Instantiate(particleFx);
//         particle.transform.position = targetLandmark.transform.position;
//         particle.SetActive(true);

//         UpdateSubBtnUI(currentSubBtnSelected);
//         UpdateLandmarkUpPrice(targetLandmark);
//         CheckAvailability();
//     }

//     private void CheckAvailability()
//     {
//         for(int i = 0; i<5; i++)
//         {
//             if(GetUpgradePrice(i).amount > 0 & Camera.main.GetComponent<MoneyUI>().HasEnoughMoney(GetUpgradePrice(i))) {
//                 avilablemark_ui[i].SetActive(true);
//             } else
//             {
//                 avilablemark_ui[i].SetActive(false);
//             }

//             upgradeBtns_count_ui[i].text = targetLandmark.GetComponent<Landmark>().upgradeStatus[i].ToString();
//         }

//         upgradePrice = GetUpgradePrice(currentSubBtnSelected);

//         if (upgradePrice.amount != -1)
//         {
//             upgrade_price_ui.text = upgradePrice.GetString();

//             if(Camera.main.GetComponent<MoneyUI>().HasEnoughMoney(upgradePrice))
//             {
//                 upgrade_btn_ui.interactable = true;
//             } else upgrade_btn_ui.interactable = false;
//         }
//         else
//         {
//             upgrade_price_ui.text = "MAX";
//             upgrade_btn_ui.interactable = false;
//         }

//         int totalUpCount = 0;
//         for (int i = 0; i < 5; i++)
//         {
//             totalUpCount += targetLandmark.GetComponent<Landmark>().upgradeStatus[i];
//         }
//         int totalMaxCount = targetLandmark.GetComponent<Landmark>().totalMaxCount;

//         //print(string.Format("totalUpCount = {0}, totalMaxCount = {1}", totalUpCount, totalMaxCount));
//         if(mainSlider.value!= totalUpCount / (totalMaxCount - 5f))
//         mainSlider.DOValue(totalUpCount / (totalMaxCount - 5f), 0.2f);

//         Price levelUpPrice = targetLandmark.GetComponent<LocationObject>().levelUpPrice;
//         if (levelUpPrice.amount != -1)
//         {
//             levelUp_price_ui.text = levelUpPrice.GetString();
//             levelUp_btn_ui.interactable = (Camera.main.GetComponent<MoneyUI>().HasEnoughMoney(levelUpPrice) & totalUpCount == totalMaxCount - 5);
//         } else
//         {
//             levelUp_price_ui.text = "MAX";
//             levelUp_btn_ui.interactable = false;
//         }
//     }

//     public void MoveBtnClicked()
//     {
//         Camera.main.GetComponent<PinchZoom>().StartCamTransition(Camera.main.transform.position, 50);
//         locationManger.MoveBtnClicked(targetLandmark);
//         ClosePanel();
//     }

//     public void UpdateLandmarkUpPrice(GameObject landmark)
//     {
//         for(int i = 0; i<5; i++)
//         {
//             landmark.GetComponent<Landmark>().upgradePrice[i] = GetUpgradePrice(i, landmark);
//         }
//     }

//     public void RemoteUpgrade()
//     {
//         if (!started) Start();
//         UpdateUpgradeShortcut();

//         if (short_landmark == null | short_upgIdx == -1) return;

//         upgradePrice = short_landmark.upgradePrice[short_upgIdx];

//         if (!Camera.main.GetComponent<MoneyUI>().SubtractMoney(upgradePrice)) return;
//         if (upgradePrice.amount == -1) return;

//         myAudio.PlaySFX(1);
//         short_landmark.upgradeStatus[short_upgIdx] += 1;
//         short_landmark.upgradePrice[short_upgIdx] = GetUpgradePrice(short_upgIdx, short_landmark.gameObject);
//         short_landmark.SetupPigi();

//         GameObject particle = Instantiate(particleFx);
//         particle.transform.position = short_landmark.gameObject.transform.position;
//         particle.SetActive(true);

//         if(targetLandmark == short_landmark.gameObject) CheckAvailability();

//         short_landmark = null;
//         short_upgIdx = -1;
//         UpdateUpgradeShortcut();
//     }

//     public void UpdateUpgradeShortcut()
//     {
//         int landmarkIdx = -1;
//         int upgIdx = -1;
//         Price lowPrice = new Price(0, "zz");

//         for(int i = 0; i<paramsData.landmarks.Count; i++)
//         {
//             if (paramsData.landmarks[i] == null) continue;
//             for(int j = 0; j<5; j++)
//             {
//                 Price price = paramsData.landmarks[i].GetComponent<Landmark>().upgradePrice[j];
//                 if (price.amount == -1) continue;
//                 if(lowPrice.ConvertCodeToInt(lowPrice.charCode) > price.ConvertCodeToInt(price.charCode)
//                     | (lowPrice.ConvertCodeToInt(lowPrice.charCode) == price.ConvertCodeToInt(price.charCode)
//                     & lowPrice.amount > price.amount))
//                 {
//                     lowPrice = price;
//                     landmarkIdx = i;
//                     upgIdx = j;
//                     continue;
//                 }
//             }
//         }

//         if(landmarkIdx!=-1 & upgIdx != -1)
//         {
//             //public TextMeshProUGUI short_title, short_upgName, short_upgCount, short_price;
//             Landmark landmark = paramsData.landmarks[landmarkIdx].GetComponent<Landmark>();
//             short_title.text = GetLocalizedString("Names", "title_" + landmark.GetComponent<LocationObject>().modelID) + "-" + (landmark.GetComponent<LocationObject>().copyN).ToString();

//             switch (upgIdx)
//             {
//                 case 0:
//                     short_upgName.text = GetLocalizedString("UpgradeUI", "AddPigi");
//                     break;
//                 case 1:
//                     short_upgName.text = GetLocalizedString("UpgradeUI", "UpgradePrice");
//                     break;
//                 case 2:
//                     short_upgName.text = GetLocalizedString("UpgradeUI", "UpgradeTime");
//                     break;
//                 case 3:
//                     short_upgName.text = GetLocalizedString("UpgradeUI", "UpgradeBonus");
//                     break;
//                 case 4:
//                     short_upgName.text = GetLocalizedString("UpgradeUI", "UpgradeAuto");
//                     break;
//             }

//             short_price.text = lowPrice.GetString();
//             short_upgCount.text = landmark.upgradeStatus[upgIdx].ToString();

//             short_landmark = landmark;
//             short_upgIdx = upgIdx;
//         } else
//         {
//             short_title.text = "가능 업그레이드 없음";
//             short_upgName.text = "";
//             short_price.text = "";
//             short_upgCount.text = "";
//         }
//     }

    // public void LevelUPBtnClicked()
    // {
    //     int totalUpCount = 0;
    //     for (int i = 0; i < 5; i++)
    //     {
    //         totalUpCount += targetLandmark.GetComponent<Landmark>().upgradeStatus[i];
    //     }
    //     int totalMaxCount = targetLandmark.GetComponent<Landmark>().totalMaxCount;

    //     if(totalUpCount != totalMaxCount-5)
    //     {
    //         GameObject ballonMsg = Instantiate(balloon, gameObject.transform.parent.transform);
    //         ballonMsg.GetComponent<BalloonMsg>().Show("모든 업그레이드를 완료해주세요!");
    //     } else
    //     {
    //         if(Camera.main.GetComponent<MoneyUI>().SubtractMoney(targetLandmark.GetComponent<LocationObject>().levelUpPrice)) {

    //             GameObject ballonMsg = Instantiate(balloon, gameObject.transform.parent.transform);
    //             ballonMsg.GetComponent<BalloonMsg>().Show("업그레이드!!!!!!!!");

    //             targetLandmark = locationManger.LevelUPLandmark(targetLandmark);
    //             CheckAvailability();
    //         } else
    //         {
    //             GameObject ballonMsg = Instantiate(balloon, gameObject.transform.parent.transform);
    //             ballonMsg.GetComponent<BalloonMsg>().Show("돈이 부족합니다.");
    //         }
    //     }
    // }
}
