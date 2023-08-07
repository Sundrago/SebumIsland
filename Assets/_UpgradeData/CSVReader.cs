using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct UpgradeDataList
{
    public List<UpgradeData> data;
    public string ID;
    public string charCode;
    public int defaultGrowTime;
    public int maxUpdateIdx;
    public int pigiAmout;
    public Price defaultPrice;
    public int buildTime;
    public Price buildPrice;
}

public class UpgradeData
{
    public Price price;
    public float value;
    public float speed;

    public UpgradeData(Price _price, float _value, float _speed) {
        price = _price;
        value = _value;
        speed = _speed;
    }
}

public class CSVReader : MonoBehaviour
{
    [SerializeField] TextAsset CSVData;
    public List<UpgradeDataList> importedData = new List<UpgradeDataList>();

    public bool started = false;
    const int WIDTH = 12;

    public void Start()
    {
       if (started) return;
       ReadCSV();
       started = true;
    }

    void ReadCSV()
    {
        string[] dataSet = CSVData.text.Split(new string[] { "*" }, StringSplitOptions.None);

            for(int i = 0; i<dataSet.Length; i++) {
                UpgradeDataList dataList = new UpgradeDataList();
                string[] data = dataSet[i].Split(',', '\n');
                dataList.ID = data[GetIdx(3,1)];
                print("!" + dataList.ID);
                dataList.charCode = data[GetIdx(12,5)];
                dataList.defaultGrowTime = int.Parse(data[GetIdx(5,1)]);
                dataList.maxUpdateIdx = int.Parse(data[GetIdx(7,1)]);
                dataList.pigiAmout = int.Parse(data[GetIdx(9,1)]);
                dataList.defaultPrice = new Price(int.Parse(data[GetIdx(10,2)]), dataList.charCode);
                dataList.buildTime = int.Parse(data[GetIdx(11,2)]);
                dataList.buildPrice = new Price(int.Parse(data[GetIdx(12,2)]), dataList.charCode);

                dataList.data = new List<UpgradeData>();
                for(int j=0; j<dataList.maxUpdateIdx; j++) {
                    Price price = new Price(int.Parse(data[GetIdx(2,3+j)]), dataList.charCode);
                    UpgradeData _data = new UpgradeData(price, float.Parse(data[GetIdx(3,3+j)]), float.Parse(data[GetIdx(4,3+j)]));
                    dataList.data.Add(_data);
                }

                importedData.Add(dataList);
           }
    }

    int GetIdx(int x, int y) {
        return (y-1) * WIDTH + (x-1);
    }

    public UpgradeDataList GetDataList(string id)
    {
        int idx = -1;
        for(int i = 0; i<importedData.Count; i++) {
            if(importedData[i].ID == id) {
                idx = i; 
                break;
            }
        }

        if(idx == -1) {
            print("[CSVREADER.CS] UpgradeDataList | Can't find CSV data with id : " + id);
            return null;
        } else return importedData[idx];
    }
}
