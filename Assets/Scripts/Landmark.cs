using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//public class LandMarkData
//{
//    string ID;
//    //position
//    string type;
//    int[] upgradeStatus;
//    int maxCount;
//    float maxTime;
//    float bunusRate;
//    float bunusMultiplier;

//    public void LoadInfo(string myID)
//    {
//        ID = myID;
//        type = PlayerPrefs.GetString(myID + "_TYPE");
//        upgradeStatus = new int[4];
//        upgradeStatus[0] = PlayerPrefs.GetInt(myID + "_UPGRADE0");
//        upgradeStatus[1] = PlayerPrefs.GetInt(myID + "_UPGRADE1");
//        upgradeStatus[2] = PlayerPrefs.GetInt(myID + "_UPGRADE2");
//        upgradeStatus[3] = PlayerPrefs.GetInt(myID + "_UPGRADE3");
//        upgradeStatus[4] = PlayerPrefs.GetInt(myID + "_UPGRADE4");
//    }

//    public void SaveInfo()
//    {
//        PlayerPrefs.SetString(ID + "_TYPE", type);
//        PlayerPrefs.SetInt(ID + "_UPGRADE0", upgradeStatus[0]);
//        PlayerPrefs.SetInt(ID + "_UPGRADE1", upgradeStatus[1]);
//        PlayerPrefs.SetInt(ID + "_UPGRADE2", upgradeStatus[2]);
//        PlayerPrefs.SetInt(ID + "_UPGRADE3", upgradeStatus[3]);
//        PlayerPrefs.SetInt(ID + "_UPGRADE3", upgradeStatus[4]);
//    }
//}

public class Landmark : MonoBehaviour
{
    public string ID;
    //public int placeID;

    public GameObject pigi;
    public List<GameObject> pigis = new List<GameObject>();
    public List<GameObject> grownPigis = new List<GameObject>();
    public List<GameObject> growSpots = new List<GameObject>();

    public TextMeshPro guideText;
    public LandmarkPlaceSelector PlaceSelector;
    public UpgradePanel upgradePanel;
    [SerializeField] HarvestAllCtrl harvestAllCtrl;

    public int pigiCount;

    public int maxUprade;

    public LocationObject locationObject;

    public Price sellPrice = new Price(1, "a");
    public float growTime = 10;

    private Price defaultPrice;
    private float defaultGrowTime;

    private float SpeedMultiplier, PriceMultiplier;

    public ParamsData paramsData;
    private bool started = false;
    // Start is called before the first frame update
    public void Start()
    {
        if (started) return;
        started = true;

        //Deactive Obj if exits.
        foreach (GameObject obj in growSpots)
        {
            obj.SetActive(false);
        }

        //Load Data from locationObj.
        locationObject = gameObject.GetComponent<LocationObject>();
        locationObject.ReadCSV();
        ID = locationObject.landMarkID;
        defaultGrowTime = locationObject.data.defaultGrowTime;
        defaultPrice = locationObject.data.defaultPrice;
        pigiCount = locationObject.data.pigiAmout;
        maxUprade = locationObject.data.maxUpdateIdx;

        //Load Saved Data.
        // LoadData();
        UpdateData();

        //PlaceSelector.SetLandmark(gameObject, placeID);
        paramsData.landmarks.Add(gameObject);
        pigi.SetActive(false);

        //GetLevelUPInfo
        // GetComponent<LocationObject>().GetLevelUpInfo();
        SetupPigi();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (grownPigis.Count == pigis.Count | grownPigis.Count == 0)
    //     {
    //         HarvestAll();
    //     }
    // }

    public void CreatePigi(int amount)
    {
        if(growSpots.Count != 0)
        {
            for(int i = 0; i<amount; i++)
            {
                if(i >= growSpots.Count)
                {
                    print(string.Format("ERROR : CreatePigi : IDX = {0} exceed bounary", i));
                    continue;
                }
                growSpots[i].SetActive(true);
                if (!pigis.Contains(growSpots[i])) pigis.Add(growSpots[i]);
            }
            return;
        }

        if(pigis.Count > amount)
        {
            for(int i = 0; i > pigis.Count - amount; i++)
            {
                Destroy(pigis[pigis.Count - 1 - i]);
                pigis.Remove(pigis[pigis.Count - 1 - i]);
            }
        }

        Vector3[] pos = new Vector3[12];
        pos[8] = new Vector3(-4f, 0, 4f); //0
        pos[6] = new Vector3(-4f, 0, 1.33f); //4
        pos[4] = new Vector3(-4f, 0, -1.33f); //6
        pos[0] = new Vector3(-4f, 0, -4f); //8
        pos[1] = new Vector3(-1.33f, 0, -4f); //1
        pos[2] = new Vector3(1.33f, 0, -4f); //2
        pos[3] = new Vector3(4f, 0, -4f); //3
        pos[5] = new Vector3(4f, 0, -1.33f); //5
        pos[7] = new Vector3(4f, 0, 1.33f); //7
        pos[11] = new Vector3(4f, 0, 4f); //11
        pos[10] = new Vector3(1.3f, 0, 4f); //10
        pos[9] = new Vector3(-1.3f, 0, 4f); //9

        for(int i = pigis.Count; i<amount; i++)
        {
            GameObject newPigi = Instantiate(pigi, gameObject.transform);
            newPigi.SetActive(true);
            newPigi.transform.localPosition = pos[i];
            pigis.Add(newPigi);
        }

    }

    public void HarvestAll()
    {
        if (grownPigis.Count != pigis.Count) return;
        int count = grownPigis.Count;
        for(int i = 0; i<count; i++)
        {
            grownPigis[count - 1 - i].GetComponent<PigiCtrl>().AutoHarvest();
            grownPigis.Remove(grownPigis[count - 1 - i]);
        }
    }

    public void UpdateData()
    {
        int upgradeStatus = locationObject.upgradeStatus;
        PriceMultiplier = locationObject.data.data[locationObject.upgradeStatus].value;
        SpeedMultiplier = locationObject.data.data[locationObject.upgradeStatus].speed;

        sellPrice = new Price(Mathf.RoundToInt(defaultPrice.amount * PriceMultiplier / 100f), defaultPrice.charCode);
        growTime = defaultGrowTime * SpeedMultiplier / 100f;
    }

    public void SetupPigi()
    {
        print("SetupPigi");
        CreatePigi(pigiCount);

        foreach (GameObject obj in pigis)
        {
            PigiCtrl pigi = obj.transform.Find("Pigi").GetComponent<PigiCtrl>();
            pigi.Start();
            pigi.StartTimer();
            print("starttimer");
        }
    }

    public void PigiIsReady(GameObject obj) {

        if(grownPigis.Contains(gameObject)) return;

        grownPigis.Add(obj);
        if (grownPigis.Count == pigis.Count) { //| grownPigis.Count == 0
            harvestAllCtrl.AddReadyLandmark(gameObject);
        }
    }

    public void DestroyLandmark() {
        if (grownPigis.Count == pigis.Count) harvestAllCtrl.RemoveReadyLandmark(gameObject);
    }


    // void SaveData()
    // {
    //     //print("data saved");
    //     //PlayerPrefs.SetInt(ID + "_UPGRADE0", upgradeStatus[0]);
    //     //PlayerPrefs.SetInt(ID + "_UPGRADE1", upgradeStatus[1]);
    //     //PlayerPrefs.SetInt(ID + "_UPGRADE2", upgradeStatus[2]);
    //     //PlayerPrefs.SetInt(ID + "_UPGRADE3", upgradeStatus[3]);
    //     //PlayerPrefs.SetInt(ID + "_UPGRADE4", upgradeStatus[4]);
    //     //PlayerPrefs.Save();
    // }

    // void LoadData()
    // {
    //     // print("data loaded : " + ID);
    // }

    // private void OnApplicationPause(bool pause)
    // {
    //     //if(pause) SaveData();

    //     PlayerPrefs.DeleteAll();
    // }

    // private void OnApplicationQuit()
    // {
    //     //SaveData();
    //     PlayerPrefs.DeleteAll();
    // }

    

}
