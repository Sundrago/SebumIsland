using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
modelID_family


*/

public class LocationObject : MonoBehaviour
{
    [SerializeField] LocationManger locationManger;

    [Header("* 가로 세로 사이즈")]
    public int width;
    public int height;

    [Header("* 피지 건물일 경우 체크")]
    public bool isLandmark;

    [Header("* 피지건물 ID")]
    public string modelID;
    public string modelID_family;
    public int modelID_levelID;

    //public TextAsset DesertData;


    [Header("비워둬도 됨.")]
    public int copyN;
    public string landMarkID;
    public int x;
    public int y;
    public string charCode;
    public int defaultGrowTime;
    public int maxUpdateIdx;
    public int pigiAmout;
    public Price defaultPrice;
    public int buildTime;
    public Price buildPrice;
    public UpgradeDataList data;
    public Price levelUpPrice;
    public int levelUpTime;
    public string nextLevelId;

    public int upgradeStatus = 0;


    private bool started = false;

    public void ReadCSV()
    {
        if(started) return;
        started = true;

        print("LOAD UPGDATA AT : " + modelID);
        landMarkID = modelID + "-" + copyN;

        CSVReader csv = locationManger.gameObject.GetComponent<CSVReader>();
        csv.Start();
        data = csv.GetDataList(modelID);

        charCode = data.charCode;
        defaultGrowTime = data.defaultGrowTime;
        maxUpdateIdx = data.maxUpdateIdx;
        pigiAmout = data.pigiAmout;
        defaultPrice = new Price(data.defaultPrice.amount, data.defaultPrice.charCode);
        buildTime = data.buildTime;
        buildPrice = data.buildPrice;

        GetLevelUpInfo();
    }

    private void GetLevelUpInfo()
    {
        //LevelUP Settings.
        levelUpPrice = new Price(-1, "a");
        levelUpTime = -1;
        nextLevelId = modelID_family + (modelID_levelID + 1);

        GameObject nextLandmark = locationManger.FindAvailableObj(nextLevelId);
        if(nextLandmark != null)
        {
            nextLandmark.GetComponent<LocationObject>().ReadCSV();
            levelUpPrice = nextLandmark.GetComponent<LocationObject>().buildPrice;
            levelUpTime = nextLandmark.GetComponent<LocationObject>().buildTime;
        } else print("UpgradePanel : FAIL TO PARS nextLevelId");
    }

    public bool ReadyForLevelUp() {
        if(upgradeStatus >= maxUpdateIdx - 1) return true;
        else return false; 
    }

    public Price GetUpgradePrice() {
        if(upgradeStatus >= maxUpdateIdx - 1) {
            return levelUpPrice;
        }
        return(data.data[upgradeStatus].price);
    }
}

