using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
///     Class representing the data of a location object.
/// </summary>
[Serializable]
public class LocationObjData
{
    public string modelId;
    public int copyN, x, y;
    public string buildTime;
}

/// <summary>
///     Class responsible for managing player data.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private LocationManger locationManger;
    [SerializeField] private MoneyManager money;

    [TableList(ShowIndexLabels = true, ShowPaging = true)]
    public List<LocationObjData> locationObjDatas = new();

    /* We also call OnApplicationPause, which is called when an app goes into the background. */
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            OnApplicationQuit();
    }


    private void OnApplicationQuit()
    {
        SaveData();
    }

    [Button]
    public void ReadDataToList()
    {
        locationObjDatas = new List<LocationObjData>();

        foreach (var obj in locationManger.AllocatedObj)
        {
            var data = new LocationObjData();
            data.modelId = obj.ScriptableObjet.ModelID;
            data.copyN = obj.CopyN;
            data.x = obj.X;
            data.y = obj.Y;
            data.buildTime = obj.GetBuildTime();
            locationObjDatas.Add(data);
        }
    }

    public void SaveData()
    {
        SaveLocationData();
        SaveBalanceData();
    }

    private void SaveLocationData()
    {
        ReadDataToList();
        ES3.Save("locationObjData", locationObjDatas);
    }

    private void SaveBalanceData()
    {
        var myBalance = money.GetMyBalance();
        PlayerPrefs.SetInt("myBalanceAmount", myBalance.amount);
        PlayerPrefs.SetString("myBalanceChar", myBalance.charCode);

        var gemAmount = money.GetMyGemOil(CoinType.Gem);
        var oilAmount = money.GetMyGemOil(CoinType.Oil);
        PlayerPrefs.SetInt("gemAmount", gemAmount);
        PlayerPrefs.SetInt("oilAmount", oilAmount);

        PlayerPrefs.Save();
    }

    [Button]
    public void LoadData()
    {
        if (!ES3.KeyExists("locationObjData"))
        {
            ResetData();
            return;
        }

        LoadLocationData();
        LoadBalanceData();
    }

    private void LoadBalanceData()
    {
        if (PlayerPrefs.HasKey("myBalanceAmount"))
        {
            money.ResetMoney();
            var myBalance = new Price(PlayerPrefs.GetInt("myBalanceAmount"), PlayerPrefs.GetString("myBalanceChar"));
            money.AddMoney(myBalance);
            print("data loaded - balance : " + myBalance.GetString());
        }

        if (PlayerPrefs.HasKey("gemAmount"))
        {
            var gemAmount = PlayerPrefs.GetInt("gemAmount");
            money.AddGemOil(CoinType.Gem, gemAmount);
            print("data loaded - gemAmount : " + gemAmount);
        }

        if (PlayerPrefs.HasKey("oilAmount"))
        {
            var oilAmount = PlayerPrefs.GetInt("oilAmount");
            money.AddGemOil(CoinType.Oil, oilAmount);
            print("data loaded - oilAmount : " + oilAmount);
        }
    }

    private void LoadLocationData()
    {
        locationObjDatas = new List<LocationObjData>();
        locationObjDatas = ES3.Load<List<LocationObjData>>("locationObjData");
        locationManger.ResetAllocatedObj();
        foreach (var data in locationObjDatas) locationManger.AddAllocatedObj(data);
        locationManger.UpdateLocations();
    }

    public void ResetData()
    {
        //location
        locationManger.ResetAllocatedObj();
        locationObjDatas = new List<LocationObjData>();
        ES3.Save("locationObjData", locationObjDatas);
        locationManger.UpdateLocations();

        //money
        PlayerPrefs.DeleteAll();
        money.ResetMoney();
        money.AddMoney(new Price(5));
    }
}