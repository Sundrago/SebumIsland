using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
///     Represents a button set used for building landmarks in a build panel.
/// </summary>
public class BuildButtonManager : MonoBehaviour
{
    [SerializeField] public Image img;
    [SerializeField] public TextMeshProUGUI title, descr, count_text, btn_text, buildTime_text;
    [SerializeField] public Button buyBtn;
    private int count;
    private string ID;

    public Price price;

    public void Init(string ID)
    {
        this.ID = ID;
        var landmarkItem = InfoDataManager.Instance.GetLandmarkItemByID(ID);
        var upgradeData = CSVReader.Instance.GetDataList(ID);

        if (landmarkItem == null || upgradeData == null) return;

        InitiateUIElements(ID, landmarkItem, upgradeData);

        buyBtn.onClick.AddListener(OnBtnClicked);
        buyBtn.interactable = MoneyManager.Instance.HasEnoughMoney(price);
    }

    private void InitiateUIElements(string ID, LandmarkItem landmarkItem, UpgradeDataList upgradeData)
    {
        img.sprite = landmarkItem.Img;
        count = LocationManger.Instance.CountObj(ID);
        count_text.text = count + "/3";
        title.text = LocalizationSettings.StringDatabase.GetLocalizedString("LandmarkController", ID + "_title");
        descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("LandmarkController", ID + "_descr");
        price = upgradeData.buildPrice;
        var buildTime = upgradeData.buildTime;
        btn_text.text = price.GetString();
        buildTime_text.text = buildTime + "초";
    }

    public void UpdateBuildBtn()
    {
        var count = LocationManger.Instance.CountObj(ID);
        count_text.text = count + "/3";
        buyBtn.interactable = MoneyManager.Instance.HasEnoughMoney(price);
    }

    public void OnBtnClicked()
    {
        if (MoneyManager.Instance.HasEnoughMoney(price))
        {
            LocationManger.Instance.BuildNewLandmark(ID, count, price);
            gameObject.transform.GetComponentInParent<BuildPanel>().ClosePanel();
        }
    }
}