using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

public class BuildBtnSet : MonoBehaviour
{
    [SerializeField] public Image img;
    [SerializeField] public TextMeshProUGUI title, descr, count_text, btn_text, buildTime_text;
    [SerializeField] public Button buyBtn;

    public Price price;
    private string ID;
    private int count;

    public async Task Init(string _ID)
    {
        ID = _ID;
        LandmarkItem landmarkItem = InfoDataManager.Instance.GetLandmarkItemByID(ID);
        UpgradeDataList upgradeData = CSVReader.Instance.GetDataList(ID);

        if (landmarkItem == null || upgradeData == null) await Task.Yield();

        img.sprite = landmarkItem.Img;

        count = LocationManger.Instance.CountObj(ID);
        count_text.text = count + "/3";

        title.text = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_title");
        descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_descr");

        price = upgradeData.buildPrice;
        int buildTime = upgradeData.buildTime;
        btn_text.text = price.GetString();
        buildTime_text.text = buildTime.ToString() + "초";

        buyBtn.onClick.AddListener(OnBtnClicked);
        buyBtn.interactable = MoneyUI.Instance.HasEnoughMoney(price);
    }

    public void UpdateBuildBtn()
    {
        int count = LocationManger.Instance.CountObj(ID);
        count_text.text = count + "/3";
        buyBtn.interactable = MoneyUI.Instance.HasEnoughMoney(price);
    }

    public void OnBtnClicked()
    {
        if (MoneyUI.Instance.HasEnoughMoney(price))
        {
            LocationManger.Instance.BuildNewLandmark(ID, count, price);
            gameObject.transform.GetComponentInParent<BuildPanelCtrl>().ClosePanel();
        }
    }
}