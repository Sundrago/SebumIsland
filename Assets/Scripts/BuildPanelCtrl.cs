using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildPanelCtrl : MonoBehaviour
{
    public UpgradePanel upgradePanel;
    public PigiInfoPanel pigiInfoPanel;
    public LocationManger locationManger;

    public BuildBtnSet[] buildBtns = new BuildBtnSet[2];
    public string[] familyIds = new string[2];

    public int[] counts = new int[2];


    private bool started = false;

    private void Start()
    {
        if (started) return;
        if (gameObject.activeSelf) gameObject.SetActive(false);
        started = true;

        familyIds[0] = "farm";
        familyIds[1] = "tree";
    }

    public void OpenPanel()
    {
        Start();
        //upgradePanel.ClosePanel(false);
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
                locationManger.BuildNewLandmark("storm0", 0);
                break;
            case 3:
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
            print(price.GetString());
        }
    }
}
