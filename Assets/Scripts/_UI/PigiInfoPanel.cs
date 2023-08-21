using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class PigiInfoPanel : MonoBehaviour
{
    public GameObject targetPigi = null;
    public Slider slider_ui;
    public TextMeshProUGUI progress_ui_text, price, time;

    public UpgradePanel upgradePanel;
    private Vector3 camPos;
    private float camZoom;

    public GameObject upgShort;
    public GameObject fastUpgradeBtnHolder;

    public TextMeshProUGUI title_ui, descr_ui;

    // Start is called before the first frame update
    private void Start()
    {
        if (targetPigi == null) gameObject.SetActive(false);
    }

    private void Update()
    {
        if(targetPigi != null & Time.frameCount % 10 == 0)
        {
            slider_ui.value = targetPigi.GetComponent<PigiCtrl>().progress;
            progress_ui_text.text = "" + Mathf.Round(targetPigi.GetComponent<PigiCtrl>().progress * 100) + "%";
        }
    }

    public void OpenPanel(GameObject target)
    {
        PanelManager.Instance.CloseOtherPanels(gameObject);
        gameObject.SetActive(true);
        targetPigi = target;
        UpdateInfo();
        // upgradePanel.ClosePanel(false);

        //store cam
        camPos = Camera.main.transform.position;
        camZoom = Camera.main.orthographicSize;

        //Set UIs
        string id = target.GetComponent<PigiCtrl>().ID;
        title_ui.text = GetLocalizedString("Pigi", "title_" + id);
        descr_ui.text = GetLocalizedString("Pigi", "descr_" + id);
    }

    public void ClosePanel(bool restoreCam = true)
    {
        if (!gameObject.activeSelf) return;
        
        SetActive();

        //restore cam
        if (!restoreCam) return;
        Camera.main.GetComponent<PinchZoom>().StartCamTransition(camPos, camZoom);

    }

    private void SetActive()
    {
        targetPigi = null;
        gameObject.SetActive(false);
    }

    public void UpdateInfo()
    {
        if (targetPigi == null) return;
        price.text = targetPigi.GetComponent<PigiCtrl>().sellPrice.GetString();
        time.text = Mathf.RoundToInt(targetPigi.GetComponent<PigiCtrl>().growTime)*10f/10f + "s";
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }
}
