using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class Landmark : MonoBehaviour
{
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required, SerializeField]
    public string ID;
    //public int placeID;
    [GUIColor(0.4f, 0.8f, 0.8f, 1f), Required, SerializeField]
    private string pigiID;

    [GUIColor(0.7f, 0.8f, 0.8f, 1f)]
    public List<Transform> growSpots = new List<Transform>();

    [ReadOnly]
    public List<GameObject> pigis = new List<GameObject>();
    [ReadOnly]
    public List<GameObject> grownPigis = new List<GameObject>();

    [GUIColor(0.7f, 0.8f, 0.8f, 1f), Required, SerializeField]
    public TextMeshPro guideText;

    private HarvestAllCtrl harvestAllCtrl;

    private int pigiCount;

    private int maxUprade;

    [ReadOnly]
    public LocationObject locationObject;

    [ReadOnly]
    public Price sellPrice = new Price(1, "a");
    [ReadOnly]
    public float growTime = 10;

    private Price defaultPrice;
    private float defaultGrowTime;

    private float SpeedMultiplier, PriceMultiplier;

    private bool started = false;

    [ReadOnly]
    public bool isBuilding = false;

    [ReadOnly]
    public System.DateTime buildCompleteTime;

    // Start is called before the first frame update
    public void Start()
    {
        if (started) return;
        started = true;

        harvestAllCtrl = HarvestAllCtrl.Instance;
        if (pigiID == "") pigiID = "pigi_default";

        //Load Data from locationObj.
        locationObject = gameObject.GetComponent<LocationObject>();
        locationObject.ReadCSV();
        ID = locationObject.landMarkID;
        defaultGrowTime = locationObject.data.defaultGrowTime;
        defaultPrice = locationObject.data.defaultPrice;
        pigiCount = locationObject.data.pigiAmout;
        maxUprade = locationObject.data.maxUpdateIdx;
        buildCompleteTime = locationObject.buildCompleteTime;

        //Load Saved Data.
        // LoadData();
        UpdateData();

        //GetLevelUPInfo
        if(buildCompleteTime > System.DateTime.Now)
        {
            isBuilding = true;
            guideText.gameObject.SetActive(true);
        } else
        {
            isBuilding = false;
            SetupPigi();
        }
    }

    public void CreatePigi(int amount)
    {
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

        if (growSpots.Count != 0)
        {
            for(int i = 0; i<amount; i++)
            {
                if(i >= growSpots.Count)
                {
                    print(string.Format("ERROR : CreatePigi : IDX = {0} exceed bounary", i));
                    continue;
                }
                pos[i] = growSpots[i].position;
            }
        }

        if(pigis.Count > amount)
        {
            for(int i = 0; i > pigis.Count - amount; i++)
            {
                Destroy(pigis[pigis.Count - 1 - i]);
                pigis.Remove(pigis[pigis.Count - 1 - i]);
            }
        }

        for(int i = pigis.Count; i<amount; i++)
        {
            GameObject newPigi = Instantiate(InfoDataManager.Instance.GetPigiItemByID(pigiID).prefab, gameObject.transform);
            newPigi.GetComponentInChildren<PigiCtrl>().Init(this, pos[i], pigiID);
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

    private void Update()
    {
        if (!isBuilding | Time.frameCount % 30 != 0) return;

        System.TimeSpan timeSpan;
        timeSpan = buildCompleteTime - System.DateTime.Now;
        string outputString = "건설중! ";

        int totalSeconds = Mathf.FloorToInt((float)timeSpan.TotalSeconds);

        if(totalSeconds > 3600)
        {
            int hr = Mathf.FloorToInt(totalSeconds / 3600f);
            totalSeconds -= hr * 3600;
            outputString += hr + "시간 ";
        }

        if (totalSeconds > 60)
        {
            int mn = Mathf.FloorToInt(totalSeconds / 60);
            totalSeconds -= mn * 60;
            outputString += mn + "분 ";
        }

        if(totalSeconds > 0)
        {
            outputString += totalSeconds + "초 ";
        }
        outputString += "남음";

        guideText.text = outputString;

        if (timeSpan.TotalSeconds <= 0)
        {
            isBuilding = false;
            guideText.gameObject.SetActive(false);
            SetupPigi();
        }
    }
}
