using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

/*
modelID_family


*/

public class LocationObject : MonoBehaviour
{
    private LocationManger locationManger;

    [Header("* 가로 세로 사이즈")]
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public int width;
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public int height;

    [Header("* 피지 건물일 경우 체크")]
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public bool isLandmark;

    [Header("* 피지건물 ID")]
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public string modelID;
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public string modelID_family;
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required]
    public int modelID_levelID;

    //public TextAsset DesertData;

    [Header("비워둬도 됨.")]
    [ReadOnly]
    public int copyN;
    [ReadOnly]
    public string landMarkID;
    [ReadOnly]
    public int x;
    [ReadOnly]
    public int y;
    [ReadOnly]
    public string charCode;
    [ReadOnly]
    public int defaultGrowTime;
    [ReadOnly]
    public int maxUpdateIdx;
    [ReadOnly]
    public int pigiAmout;
    [ReadOnly]
    public Price defaultPrice;
    [ReadOnly]
    public int buildTime;
    [ReadOnly]
    public Price buildPrice;
    [ReadOnly]
    public UpgradeDataList data;
    [ReadOnly]
    public Price levelUpPrice;
    [ReadOnly]
    public int levelUpTime;
    [ReadOnly]
    public string nextLevelId;
    [ReadOnly]
    public int upgradeStatus = 0;

    private bool started = false;

    const string format = "yyyy/MM/dd HH:mm:ss";
    System.IFormatProvider provider;
    public System.DateTime buildCompleteTime = System.DateTime.Now;

    public void ReadCSV(string buildCompleteTime_str = null)
    {
        if(started) return;
        started = true;

        locationManger = LocationManger.Instance;

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

        //setup build time
        if (buildCompleteTime_str == null)
        {
            buildCompleteTime = System.DateTime.Now.AddSeconds(buildTime);
        }
        else
        {
            buildCompleteTime = System.DateTime.ParseExact(buildCompleteTime_str, format, provider);
        }
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
            UpgradeDataList nextLevel = CSVReader.Instance.GetDataList(nextLevelId);
            nextLandmark.GetComponent<LocationObject>().ReadCSV();
            levelUpPrice = nextLevel.buildPrice;
            levelUpTime = nextLevel.buildTime;
        }
        else
        {
            nextLevelId = "";
            print("UpgradePanel : FAIL TO PARS nextLevelId");
        }
    }

    public bool ReadyForLevelUp() {
        if(upgradeStatus >= maxUpdateIdx - 1) return true;
        else return false; 
    }

    public Price GetUpgradePrice() {
        if(upgradeStatus >= maxUpdateIdx - 1)
        {
            if (nextLevelId == "") return new Price(-1); 
            return levelUpPrice;
        }
        return(data.data[upgradeStatus].price);
    }

    public string GetBuildTime()
    {
        return buildCompleteTime.ToString(format);
    }
}

