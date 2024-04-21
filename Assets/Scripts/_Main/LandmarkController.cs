using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LandmarkController : LocationObject
{
    [SerializeField] private List<Transform> growSpots = new();
    [SerializeField] private TextMeshPro guideText;

    private HarvestAllButton harvestAllButton;
    private bool initiated;
    private readonly List<GameObject> pigis = new();

    public List<GameObject> GrownPigis { get; } = new();
    public Price SellPrice { get; private set; } = new(1);
    public float GrowTime { get; private set; } = 10;
    public float SpeedMultiplier { get; private set; }
    public float PriceMultiplier { get; private set; }
    public bool IsInBuildingState { get; private set; }


    public void Start()
    {
        if (initiated) return;
        initiated = true;

        harvestAllButton = HarvestAllButton.Instance;
        UpdateData();
        UpdateBuildingState();
    }

    private void Update()
    {
        if (!IsInBuildingState) return;
        if (Time.frameCount % 30 != 0) return;

        var totalSeconds = GetLeftTimeInSeconds();
        var outputString = GetOutputString(totalSeconds);

        guideText.text = outputString;
        if (totalSeconds <= 0) CompleteBuilding();
    }

    private void UpdateBuildingState()
    {
        if (buildCompleteTime > DateTime.Now)
        {
            IsInBuildingState = true;
            guideText.gameObject.SetActive(true);
        }
        else
        {
            IsInBuildingState = false;
            SetupPigi();
        }
    }

    private void CreatePigi(int amount)
    {
        var pos = GetDefaultPosition(amount);
        var pigiWeight = GetPigiWeight();

        DestroyPigiIfExsits(amount);
        InstantiatePigi(amount, pigiWeight, pos);
    }

    private void InstantiatePigi(int amount, float bonusWeight, Vector3[] pos)
    {
        for (var i = pigis.Count; i < amount; i++)
        {
            var random = Random.Range(0f, bonusWeight);
            var pigiID = "pigi_default";
            var multiplier = 1f;

            foreach (var data in ScriptableObjet.PigiSpawnDatas)
                if (random >= data.minWeightRatio && random < data.maxWeightRatio)
                {
                    pigiID = data.ID;
                    multiplier = data.multiplier;
                    break;
                }

            var newPigi = Instantiate(InfoDataManager.Instance.GetPigiItemByID(pigiID).prefab,
                gameObject.transform);
            newPigi.GetComponentInChildren<PigiController>().Init(this, pos[i], pigiID, multiplier);
            pigis.Add(newPigi);
        }
    }

    private float GetPigiWeight()
    {
        float bonusWeight = 0;
        foreach (var data in ScriptableObjet.PigiSpawnDatas)
        {
            data.minWeightRatio = bonusWeight;
            bonusWeight += data.weightRatio;
            data.maxWeightRatio = bonusWeight;
        }

        return bonusWeight;
    }

    private void DestroyPigiIfExsits(int amount)
    {
        if (pigis.Count > amount)
            for (var i = 0; i > pigis.Count - amount; i++)
            {
                Destroy(pigis[pigis.Count - 1 - i]);
                pigis.Remove(pigis[pigis.Count - 1 - i]);
            }
    }

    private Vector3[] GetDefaultPosition(int amount)
    {
        var pos = GenerateDefaultPositions();
        if (growSpots.Count != 0)
            for (var i = 0; i < amount; i++)
            {
                if (i >= growSpots.Count)
                {
                    Debug.LogWarning($"ERROR : CreatePigi : IDX = {i} exceed boundary");
                    continue;
                }

                pos[i] = growSpots[i].localPosition;
            }

        return pos;
    }

    private Vector3[] GenerateDefaultPositions()
    {
        return new Vector3[]
        {
            new(-4f, 0, -4f), //0
            new(-1.33f, 0, -4f), //1
            new(1.33f, 0, -4f), //2
            new(4f, 0, -4f), //3
            new(-4f, 0, -1.33f), //4
            new(4f, 0, -1.33f), //5
            new(-4f, 0, 1.33f), //6
            new(4f, 0, 1.33f), //7
            new(-4f, 0, 4f), //8
            new(-1.3f, 0, 4f), //9
            new(1.3f, 0, 4f), //10
            new(4f, 0, 4f) //11
        };
    }

    public void HarvestAll()
    {
        if (GrownPigis.Count != pigis.Count) return;
        var count = GrownPigis.Count;
        for (var i = 0; i < count; i++)
        {
            GrownPigis[count - 1 - i].GetComponent<PigiController>().AutoHarvest();
            GrownPigis.Remove(GrownPigis[count - 1 - i]);
        }
    }

    public void UpdateData()
    {
        if(!initiated) ReadCSV();
        PriceMultiplier = Data.data[UpgradeStatus].value;
        SpeedMultiplier = Data.data[UpgradeStatus].speed;

        SellPrice = new Price(Mathf.RoundToInt(Data.defaultPrice.amount * PriceMultiplier / 100f),
            Data.defaultPrice.charCode);
        GrowTime = Data.defaultGrowTime * SpeedMultiplier / 100f;
    }

    public void SetupPigi()
    {
        CreatePigi(Data.pigiAmout);

        foreach (var obj in pigis)
        {
            var pigi = obj.transform.Find("Pigi").GetComponent<PigiController>();
            pigi.Start();
        }
    }

    public void PigiIsReady(GameObject obj)
    {
        if (GrownPigis.Contains(gameObject)) return;

        GrownPigis.Add(obj);
        if (GrownPigis.Count == pigis.Count) //| grownPigis.Count == 0
            harvestAllButton.AddReadyLandmark(gameObject);
    }

    public void DestroyLandmark()
    {
        if (GrownPigis.Count == pigis.Count) harvestAllButton.RemoveReadyLandmark(gameObject);
    }

    public void RemovePigiIsReady(GameObject pigi)
    {
        if (GrownPigis.Count == pigis.Count) DestroyLandmark();
        if (GrownPigis.Contains(pigi)) GrownPigis.Remove(pigi);
    }

    private void CompleteBuilding()
    {
        IsInBuildingState = false;
        guideText.gameObject.SetActive(false);
        SetupPigi();
    }

    private int GetLeftTimeInSeconds()
    {
        TimeSpan timeSpan;
        timeSpan = buildCompleteTime - DateTime.Now;
        var totalSeconds = Mathf.FloorToInt((float)timeSpan.TotalSeconds);
        return totalSeconds;
    }

    private string GetOutputString(int totalSeconds)
    {
        var outputString = "건설중! ";


        if (totalSeconds > 3600)
        {
            var hr = Mathf.FloorToInt(totalSeconds / 3600f);
            totalSeconds -= hr * 3600;
            outputString += hr + "시간 ";
        }

        if (totalSeconds > 60)
        {
            var mn = Mathf.FloorToInt(totalSeconds / 60);
            totalSeconds -= mn * 60;
            outputString += mn + "분 ";
        }

        if (totalSeconds > 0) outputString += totalSeconds + "초 ";
        outputString += "남음";
        return outputString;
    }
}