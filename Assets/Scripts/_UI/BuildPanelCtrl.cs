using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildPanelCtrl : MonoBehaviour
{
    [SerializeField] BuildBtnSet buildBtnSet_prefab;
    [SerializeField] float row_height = 140f;
    [SerializeField] GameObject btnsHolder;
    private bool initiated = false;
    private List<BuildBtnSet> buildBtnSets;

    private async void Initiate()
    {
        buildBtnSets = new List<BuildBtnSet>();
        initiated = true;

        foreach (LandmarkItem landmarkItem in InfoDataManager.Instance.LandmarkItems)
        {
            if (CSVReader.Instance.GetDataList(landmarkItem.ID) == null) return;
            BuildBtnSet buildBtn = Instantiate(buildBtnSet_prefab, btnsHolder.transform);
            Vector2 buildBtnPos = buildBtnSet_prefab.GetComponent<RectTransform>().anchoredPosition;
            buildBtnPos.y -= row_height * buildBtnSets.Count;
            buildBtn.GetComponent<RectTransform>().anchoredPosition = buildBtnPos;
            await buildBtn.Init(landmarkItem.ID);
            buildBtn.gameObject.SetActive(true);
            buildBtnSets.Add(buildBtn);
        }

        btnsHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(btnsHolder.GetComponent<RectTransform>().sizeDelta.x, row_height * buildBtnSets.Count + 100f);
    }

    private void OnEnable()
    {
        if (!initiated) Initiate();
        else UpdateBtns();
    }

    private void UpdateBtns()
    {
        foreach (BuildBtnSet buildBtn in buildBtnSets)
        {
            buildBtn.UpdateBuildBtn();
        }
    }

    //[SerializeField] UpgradePanel upgradePanel;
    //[SerializeField] PigiInfoPanel pigiInfoPanel;
    //[SerializeField] LocationManger locationManger;
    //[SerializeField] BuildBtnSet[] buildBtns = new BuildBtnSet[2];
    //[SerializeField] string[] familyIds = new string[2];

    //private int[] counts = new int[3];
    //private bool started = false;

    public void OpenPanel()
    {
        //pigiInfoPanel.ClosePanel(false);
        gameObject.SetActive(true);
        //UpdatePriceCount();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    //public void BuildPigiBtnClicked(int idx)
    //{
    //    switch(idx)
    //    {
    //        case 0:
    //            locationManger.BuildNewLandmark("farm0", 0);
    //            break;
    //        case 1:
    //            locationManger.BuildNewLandmark("tree0", 0);
    //            break;
    //        case 2:
    //            locationManger.BuildNewLandmark("oilfall0", 0);
    //            break;
    //        default:
    //            break;
    //    }

    //    //UpdatePriceCount();
    //    gameObject.SetActive(false);
    //}

    //public void UpdatePriceCount()
    //{
    //    for(int i = 0; i< familyIds.Length; i++)
    //    {
    //        //count
    //        counts[i] = locationManger.CountObj(familyIds[i]);
    //        buildBtns[i].count.text = counts[i] + "/3";

    //        //price
    //        LocationObject locationObj = locationManger.FindAvailableObj(familyIds[i] + "0").GetComponent<LocationObject>();
    //        print(locationObj.gameObject.name);
    //        if (locationObj == null) continue;

    //        locationObj.ReadCSV();
    //        Price price = locationObj.buildPrice;
    //        int buildTime = locationObj.buildTime;
    //        buildBtns[i].btn_text.text = price.GetString();
    //        buildBtns[i].buildTime.text = buildTime.ToString() + "초";
    //        print(price.GetString());
    //    }
    //}
}