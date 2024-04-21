using System;
using UnityEngine;

/// <summary>
///     Represents a location object in the game.
/// </summary>
public class LocationObject : MonoBehaviour
{
    private const string format = "yyyy/MM/dd HH:mm:ss";

    [field: SerializeField] public LandmarkScriptableObjet ScriptableObjet { get; private set; }

    public DateTime buildCompleteTime = DateTime.Now;

    private bool initiated;

    private LocationManger locationManger;
    private IFormatProvider provider;


    public int CopyN { get; set; }
    public string LandMarkID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public UpgradeDataList Data { get; private set; }
    public Price LevelUpPrice { get; private set; }
    public int LevelUpTime { get; private set; }
    public string NextLevelId { get; private set; }
    public int UpgradeStatus { get; set; } = 0;


    public void ReadCSV(string buildCompleteTime = null)
    {
        if (initiated) return;
        initiated = true;

        locationManger = LocationManger.Instance;
        LandMarkID = ScriptableObjet.ModelID + "-" + CopyN;

        var csv = locationManger.gameObject.GetComponent<CSVReader>();
        csv.Start();
        Data = csv.GetDataList(ScriptableObjet.ModelID);
        GetLevelUpInfo();
        
        if (buildCompleteTime == null)
            this.buildCompleteTime = DateTime.Now.AddSeconds(Data.buildTime);
        else
            this.buildCompleteTime = DateTime.ParseExact(buildCompleteTime, format, provider);
    }

    private void GetLevelUpInfo()
    {
        UpdateLevelUpData();
        var nextLandmark = locationManger.FindAvailableObj(NextLevelId);
        if (nextLandmark != null)
        {
            var nextLevel = CSVReader.Instance.GetDataList(NextLevelId);
            nextLandmark.GetComponent<LocationObject>().ReadCSV();
            LevelUpPrice = nextLevel.buildPrice;
            LevelUpTime = nextLevel.buildTime;
        }
        else
        {
            NextLevelId = "";
        }
    }

    private void UpdateLevelUpData()
    {
        LevelUpPrice = new Price(-1);
        LevelUpTime = -1;
        NextLevelId = ScriptableObjet.ModelIDFamily + (ScriptableObjet.ModelID + 1);
    }

    public bool ReadyForLevelUp()
    {
        if (UpgradeStatus >= Data.maxUpdateIdx - 1) return true;
        return false;
    }

    public Price GetUpgradePrice()
    {
        if (UpgradeStatus >= Data.maxUpdateIdx - 1)
        {
            if (NextLevelId == "") return new Price(-1);
            return LevelUpPrice;
        }

        return Data.data[UpgradeStatus].price;
    }

    public string GetBuildTime()
    {
        return buildCompleteTime.ToString(format);
    }
}