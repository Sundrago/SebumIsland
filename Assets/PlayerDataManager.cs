using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class LocationObjData
{
	public string modelId;
	public int copyN, x, y;
	public string buildTime;
}

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] LocationManger locationManger;
	[SerializeField] MoneyUI money;

	[TableList(ShowIndexLabels = true, ShowPaging = true)]
	public List<LocationObjData> locationObjDatas = new List<LocationObjData>();	

	[Button]
	public void ReadDataToList()
    {
		locationObjDatas = new List<LocationObjData>();

		foreach (GameObject obj in locationManger.allocatedObj)
        {
			LocationObjData data = new LocationObjData();
			LocationObject locationObj = obj.GetComponent<LocationObject>();

			data.modelId = locationObj.modelID;
			data.copyN = locationObj.copyN;
			data.x = locationObj.x;
			data.y = locationObj.y;
			data.buildTime = locationObj.GetBuildTime();

			locationObjDatas.Add(data);
		}
    }

	public void SetupLandmarkObjs()
    {

    }

	[Button]
	public void SaveData()
    {
		//location data
		ReadDataToList();
		ES3.Save<List<LocationObjData>>("locationObjData", locationObjDatas);

		//money data
		Price myBalance = money.GetMyBalance();
		PlayerPrefs.SetInt("myBalanceAmount", myBalance.amount);
		PlayerPrefs.SetString("myBalanceChar", myBalance.charCode);
		PlayerPrefs.Save();
	}

	[Button]
	public void LoadData()
    {
		//location data
		locationObjDatas = new List<LocationObjData>();
		locationObjDatas = ES3.Load<List<LocationObjData>>("locationObjData");

		locationManger.ResetAllocatedObj();

		foreach(LocationObjData data in locationObjDatas)
        {
			locationManger.AddAllocatedObj(data);
        }

		locationManger.UpdateLocations();


		//money data
		if (PlayerPrefs.HasKey("myBalanceAmount"))
		{
			money.ResetMoney();
			Price myBalance = new Price(PlayerPrefs.GetInt("myBalanceAmount"), PlayerPrefs.GetString("myBalanceChar"));
			money.AddMoney(myBalance);
			print("data loaded - balance : " + myBalance.GetString());
		}
	}


	[Button]
	public void ResetData()
    {
		//location
		locationManger.ResetAllocatedObj();
		locationObjDatas = new List<LocationObjData>();
		ES3.Save<List<LocationObjData>>("locationObjData", locationObjDatas);
		locationManger.UpdateLocations();

		//money
		PlayerPrefs.DeleteAll();
		money.ResetMoney();
	}


    void OnApplicationQuit()
    {
		SaveData();
    }

    /* We also call OnApplicationPause, which is called when an app goes into the background. */
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            OnApplicationQuit();
    }

    private void Start()
    {
		LoadData();
    }
}


/*
 * Data type
 * 
 * landMarkID = modelID + copyN
 * landMarkID_
 * 
 */