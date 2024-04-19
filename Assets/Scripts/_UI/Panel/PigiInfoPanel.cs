using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
///     Rrepresents a panel containing information about a "Pigi" game object.
/// </summary>
public class PigiInfoPanel : PanelObject
{
    [SerializeField] private Slider slider_ui;
    [SerializeField] private TextMeshProUGUI progress_ui_text, price, time;
    [SerializeField] private TextMeshProUGUI title_ui, descr_ui;

    private Vector3 camPos;
    private float camZoom;
    private PigiController targetPigi;

    private void Start()
    {
        if (targetPigi == null) gameObject.SetActive(false);
    }

    private void Update()
    {
        if ((targetPigi != null) & (Time.frameCount % 10 == 0))
        {
            slider_ui.value = targetPigi.progress;
            progress_ui_text.text = "" + Mathf.Round(targetPigi.progress * 100) + "%";
        }
    }

    public void OpenPanel(PigiController target)
    {
        targetPigi = target;
        UpdateInfo();
        StoreCameraData();
        UpdateUI(target);
    }

    private void UpdateUI(PigiController target)
    {
        var id = target.ID;
        title_ui.text = GetLocalizedString("Pigi", "title_" + id);
        descr_ui.text = GetLocalizedString("Pigi", "descr_" + id);
    }

    private void StoreCameraData()
    {
        camPos = Camera.main.transform.position;
        camZoom = Camera.main.orthographicSize;
    }

    public void ClosePanel(bool restoreCam = true)
    {
        if (!gameObject.activeSelf) return;
        ClosePanel();
        RestoreCameraPosition(restoreCam);
    }

    private void RestoreCameraPosition(bool restoreCam)
    {
        if (!restoreCam) return;
        Camera.main.GetComponent<TouchEventManager>().StartCamTransition(camPos, camZoom);
    }

    public override void ClosePanel()
    {
        targetPigi = null;
        gameObject.SetActive(false);
    }

    public void UpdateInfo()
    {
        if (targetPigi == null) return;
        price.text = targetPigi.sellPrice.GetString();
        time.text = Mathf.RoundToInt(targetPigi.growTime) * 10f / 10f + "s";
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }
}