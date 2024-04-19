using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     A panel that displays information and controls for upgrading a landmark.
/// </summary>
public class UpgradePanel : PanelObject
{
    [FormerlySerializedAs("myAudio")] [SerializeField]
    private AudioManager audioManager;

    [FormerlySerializedAs("remoteUpgrade")] [SerializeField]
    private RemoteUpgradeManager remoteUpgradeManager;

    [SerializeField] private GameObject bg_btn;
    [SerializeField] private Animator panelAnimator;

    public TextMeshProUGUI title_ui, level_ui, btn_ui, info1, info2;
    public Slider slider;
    public LandmarkController targetLandmark;

    public GameObject upgrade_btn_ui;

    public LocationManger locationManger;

    public Vector2 targetPos;

    public MoneyManager money;
    private Price levelUpPrice = new();
    private LocationObject locationObject;

    public void Update()
    {
        if (targetLandmark != null)
        {
            UpdatePanelPositions();

            if (Time.frameCount % 30 == 0) DeactivateIfNotEnoughMoney();
        }
    }

    private void UpdatePanelPositions()
    {
        var worldPoint = targetLandmark.transform.position;
        worldPoint.y += 12f;
        targetPos = Camera.main.WorldToScreenPoint(worldPoint);
        targetPos.y += Mathf.Lerp(-100f, 250f, Mathf.InverseLerp(5, 65, Camera.main.orthographicSize));
        if (Vector2.Distance(targetPos, gameObject.transform.position) > 0.1f)
        {
            Vector2 newPos = gameObject.transform.position;
            newPos.x += (targetPos.x - newPos.x) / 2f;
            newPos.y += (targetPos.y - newPos.y) / 2f;
            gameObject.transform.position = newPos;
        }
    }

    public void UpgradeBtnClicked()
    {
        if (targetLandmark == null) return;
        if (!money.SubtractMoney(locationObject.GetUpgradePrice())) return;

        if (locationObject.ReadyForLevelUp())
        {
            targetLandmark = locationManger.LevelUPLandmark(targetLandmark.gameObject);
            OpenPanel(targetLandmark.gameObject);
            remoteUpgradeManager.GetAvailableUpgrades();
            return;
        }

        SetupUIAnimationAndSoundFX();
        FXManager.Instance.CreateFX(FXType.UpgradeParticleFX, gameObject.transform);
        remoteUpgradeManager.GetAvailableUpgrades();
        DeactivateIfNotEnoughMoney();

        if (locationObject.LandMarkID == "farm0-0") QuestTutorialManager.Instance.Addfarm0UpdateCount();
    }

    private void SetupUIAnimationAndSoundFX()
    {
        locationObject.UpgradeStatus += 1;
        UpdateUI();
        targetLandmark.GetComponent<LandmarkController>().UpdateData();

        AudioManager.Instance.PlaySFXbyTag(SFX_tag.Upgrade);
        if (DOTween.IsTweening(upgrade_btn_ui.transform)) DOTween.Kill(upgrade_btn_ui.transform);
        upgrade_btn_ui.transform.localScale = new Vector3(1f, 1f, 1f);
        upgrade_btn_ui.transform.DOShakeScale(0.3f);
    }

    private void UpdateUI()
    {
        level_ui.text = "레벨 " + (locationObject.UpgradeStatus + 1);
        if (DOTween.IsTweening(slider)) DOTween.Kill(slider);
        slider.DOValue(locationObject.UpgradeStatus / (float)(locationObject.Data.maxUpdateIdx - 1), 0.2f);

        levelUpPrice = locationObject.GetUpgradePrice();
        btn_ui.text = levelUpPrice.GetString();

        if (locationObject.ReadyForLevelUp())
        {
            upgrade_btn_ui.GetComponent<Image>().color = Color.yellow;
            info1.text = "레벨업 준비";
        }
        else
        {
            upgrade_btn_ui.GetComponent<Image>().color = Color.white;

            var PriceMultiplier = locationObject.Data.data[locationObject.UpgradeStatus].value;
            var SpeedMultiplier = locationObject.Data.data[locationObject.UpgradeStatus].speed;
            info1.text = "" +
                         new Price(Mathf.RoundToInt(locationObject.Data.defaultPrice.amount * PriceMultiplier / 100f),
                             locationObject.Data.defaultPrice.charCode).GetString();
            info2.text = "" + locationObject.Data.defaultGrowTime * SpeedMultiplier / 100f;
        }
    }

    public void OpenPanel(GameObject gameObject)
    {
        var landmark = gameObject.GetComponent<LandmarkController>();
        InitiatePanelUI(landmark);
        UpdatePanelPosition();
        UpdateUI();
        DeactivateIfNotEnoughMoney();
        bg_btn.SetActive(true);
    }

    private void InitiatePanelUI(LandmarkController landmark)
    {
        PanelManager.Instance.CloseOtherPanels(gameObject);
        gameObject.SetActive(true);
        panelAnimator.ResetTrigger("shrink");
        panelAnimator.SetTrigger("grow");
        targetLandmark = landmark;
        locationObject = landmark;
        title_ui.text = GetLocalizedString("LandmarkController", landmark.ScriptableObjet.ModelID + "_title");
    }

    private void UpdatePanelPosition()
    {
        var worldPoint = targetLandmark.transform.position;
        worldPoint.y += 10f;
        targetPos = Camera.main.WorldToScreenPoint(worldPoint);
        gameObject.transform.position = targetPos;
    }

    public override void ClosePanel()
    {
        if (!gameObject.activeSelf) return;
        panelAnimator.ResetTrigger("grow");
        panelAnimator.SetTrigger("shrink");
        targetLandmark = null;
        bg_btn.SetActive(false);
    }

    public void DeactiveSelf()
    {
        gameObject.SetActive(false);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }

    // Deactivate if not enough money
    private void DeactivateIfNotEnoughMoney()
    {
        if (targetLandmark == null) return;

        if (targetLandmark.GetComponent<LandmarkController>().IsInBuildingState)
        {
            upgrade_btn_ui.GetComponent<Button>().interactable = false;
            return;
        }

        if (locationObject.UpgradeStatus == locationObject.Data.maxUpdateIdx - 1)
        {
            if (locationObject.LevelUpTime == -1)
            {
                upgrade_btn_ui.GetComponent<Button>().interactable = false;
                btn_ui.text = "max";
                return;
            }

            upgrade_btn_ui.GetComponent<Button>().interactable = money.HasEnoughMoney(locationObject.LevelUpPrice);
        }
        else
        {
            upgrade_btn_ui.GetComponent<Button>().interactable = money.HasEnoughMoney(levelUpPrice);
        }
    }

    public void MoveBtnClicked()
    {
        if (targetLandmark == null) return;
        Camera.main.GetComponent<TouchEventManager>().StartCamTransition(Camera.main.transform.position, 50);
        locationManger.MoveBtnClicked(targetLandmark);
        ClosePanel();
    }

    public void BgBtnClicked()
    {
        ClosePanel();
    }
}