using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Localization.Settings;

public class InfoDataManager : MonoBehaviour
{
    public static InfoDataManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [TabGroup("Landmark")]
    [TableList(ShowIndexLabels = true, ShowPaging = true)]
    public List<LandmarkItem> LandmarkItems = new List<LandmarkItem>();

    [TabGroup("Pigi")]
    [TableList(ShowIndexLabels = true, ShowPaging = true)]
    public List<PigiItem> PigiItems = new List<PigiItem>();

    public PigiItem GetPigiItemByID(string ID)
    {
        foreach(PigiItem item in PigiItems)
        {
            if (item.ID == ID) return item;
        }

        Debug.Log("cannot find PigiItem with id : " + ID);
        return null;
    }

    public LandmarkItem GetLandmarkItemByID(string ID)
    {
        foreach(LandmarkItem item in LandmarkItems)
        {
            if (item.ID == ID) return item;
        }

        Debug.Log("cannot find LandmarkItem with id : " + ID);
        return null;
    }

    
}

[Serializable]
public class PigiItem
{
    [TableColumnWidth(57, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite Img;

    [InlineButton("LoadData", "load")]

    [VerticalGroup("ID"), LabelWidth(50)]
    public string ID;

    [VerticalGroup("ID"), LabelWidth(50)]
    public GameObject prefab;

    [ReadOnly]
    [VerticalGroup("Localized Info"), LabelWidth(30)]
    public string Name, Descr;

#if UNITY_EDITOR // Editor-related code must be excluded from builds
    [OnInspectorInit]
    private void LoadData()
    {
        Name = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "title_" + ID);
        Descr = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "descr_" + ID);
    }
#endif
}

[Serializable]
public class LandmarkItem
{
    [TableColumnWidth(57, Resizable = false)]
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite Img;

    [InlineButton("LoadData", "load")]

    [VerticalGroup("ID"), LabelWidth(50)]
    public string ID;

    [VerticalGroup("ID"), LabelWidth(50)]
    public GameObject prefab;

    [ReadOnly]
    [VerticalGroup("Localized Info"), LabelWidth(50)]
    public string Name, Descr;

#if UNITY_EDITOR // Editor-related code must be excluded from builds
    [OnInspectorInit]
    private void LoadData()
    {
        Name = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_title");
        Descr = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_descr");

        if (CSVReader.Instance.GetDataList(ID) != null) Name += "(로드완료)";
        else Name += "(로드실패)";
    }
#endif
}