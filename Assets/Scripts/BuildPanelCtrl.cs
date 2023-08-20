using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildPanelCtrl : MonoBehaviour
{
    [SerializeField] UpgradePanel upgradePanel;
    [SerializeField] PigiInfoPanel pigiInfoPanel;
    [SerializeField] LocationManger locationManger;
    [SerializeField] BuildBtnSet[] buildBtns = new BuildBtnSet[2];
    [SerializeField] string[] familyIds = new string[2];

    private int[] counts = new int[3];
    private bool started = false;

    public void OpenPanel()
    {
        pigiInfoPanel.ClosePanel(false);
        gameObject.SetActive(true);
        UpdatePriceCount();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void BuildPigiBtnClicked(int idx)
    {
        switch(idx)
        {
            case 0:
                locationManger.BuildNewLandmark("farm0", 0);
                break;
            case 1:
                locationManger.BuildNewLandmark("tree0", 0);
                break;
            case 2:
                locationManger.BuildNewLandmark("oilfall0", 0);
                break;
            default:
                break;
        }

        //UpdatePriceCount();
        gameObject.SetActive(false);
    }

    public void UpdatePriceCount()
    {
        for(int i = 0; i< familyIds.Length; i++)
        {
            //count
            counts[i] = locationManger.CountObj(familyIds[i]);
            buildBtns[i].count.text = counts[i] + "/3";

            //price
            LocationObject locationObj = locationManger.FindAvailableObj(familyIds[i] + "0").GetComponent<LocationObject>();
            print(locationObj.gameObject.name);
            if (locationObj == null) continue;

            locationObj.ReadCSV();
            Price price = locationObj.buildPrice;
            int buildTime = locationObj.buildTime;
            buildBtns[i].btn_text.text = price.GetString();
            buildBtns[i].buildTime.text = buildTime.ToString() + "초";
            print(price.GetString());
        }
    }
}
