using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum TerrainType
{
    Null,
    Sand,
    Ground,
    Grass,
    Water
}

public class LocationData
{
    public bool available;
    public GameObject bgtile;
    public int idx;
    public GameObject locationMark;
    public GameObject parent;
    public TerrainType TerrainType;
    public int x;
    public int y;

    public LocationData(int myX, int myY, GameObject parentObj, int myIdx, bool isAvailable = true)
    {
        x = myX;
        y = myY;
        parent = parentObj;
        locationMark = parent.gameObject.transform.Find("LOCATION_MARK").gameObject;
        bgtile = parent.gameObject.transform.Find("BG_TILE").gameObject;
        available = isAvailable;
        idx = myIdx;
        parent.gameObject.transform.position = GetCnerterPos();
        parent.SetActive(true);
        locationMark.SetActive(false);
        TerrainType = (TerrainType)1;
    }

    public Vector3 GetCnerterPos()
    {
        return new Vector3(5f * x, 0, 5f * y);
    }
}

public class LocationManger : MonoBehaviour
{
    [SerializeField] private TerrainManager terrainManager;
    [SerializeField] private Material defaultMaterial, occupiedMaterial, possibleMaterial, notPossibleMaterial;
    [SerializeField] private GameObject locationmarkPrefab, upBtn, downBtn, leftBtn, rightBtn;
    [SerializeField] private GameObject finishBtn;
    [SerializeField] private GameObject buildBtnSet;
    private Price buildPrice;
    private int colCount, rowCount;
    private int move_originalPosX, move_originalPosY;
    private bool moveMode;
    private bool targetAvialable;
    private int targetIdxDelta = -1;
    private LandmarkController targetObj;
    private int targetX, targetY, targetW, targetH;

    private int touchCountDelta = -1;
    public static LocationManger Instance { get; private set; }

    public List<LocationData> Locations { get; private set; } = new();
    public List<LandmarkController> AllocatedObj { get; } = new();
    public bool SettingMode { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetDefaultUIState();
        SetLocations(9, 9);
        terrainManager.UpdateTerrain();
    }

    private void Update()
    {
        if (!SettingMode) return;

        if (Input.touchCount == 0)
        {
            if (touchCountDelta != 0)
            {
                Camera.main.GetComponent<TouchEventManager>().IsInBuildMode = false;
                touchCountDelta = 0;
            }
        }
        else if (Input.touchCount == 1)
        {
            if (touchCountDelta == -1)
            {
                touchCountDelta = 0;
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            var hits = Physics.RaycastAll(ray);

            if (touchCountDelta == 0)
                foreach (var hit in hits)
                    if (hit.collider.gameObject.tag == "location")
                    {
                        var idx = hit.collider.gameObject.GetComponent<LocationMarkClickEventHandler>().idx;
                        Camera.main.GetComponent<TouchEventManager>().IsInBuildMode = true;
                        touchCountDelta = 1;
                        targetIdxDelta = idx;
                        return;
                    }

            //return if UI element is clicked
            foreach (var hit in hits)
                if (hit.collider.gameObject.tag == "location")
                {
                    var idx = hit.collider.gameObject.GetComponent<LocationMarkClickEventHandler>().idx;
                    if (idx == targetIdxDelta) return;
                    targetIdxDelta = idx;
                    LocationMarkClicked(idx);
                    touchCountDelta = 1;
                }
        }
    }

    private void SetDefaultUIState()
    {
        locationmarkPrefab.SetActive(false);
        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);
    }

    private void SetLocations(int row, int col)
    {
        colCount = col;
        rowCount = row;

        foreach (var location in Locations) Destroy(location.locationMark);
        Locations = new List<LocationData>();

        for (var i = 0; i < rowCount; i++)
        for (var j = 0; j < colCount; j++)
        {
            var x = i - (rowCount - 1) / 2;
            var y = j - (colCount - 1) / 2;

            var mark = Instantiate(locationmarkPrefab, gameObject.transform);
            mark.transform.Find("LOCATION_MARK").gameObject.GetComponent<LocationMarkClickEventHandler>().idx =
                Locations.Count;
            var location = new LocationData(x, y, mark, Locations.Count);
            Locations.Add(location);

            mark.transform.DOLocalMoveY(-10f, 1.5f)
                .SetDelay((i + 1) * (j + 1) / 150f)
                .From()
                .SetEase(Ease.OutCubic);
        }
    }

    private LocationData GetLocation(int x, int y)
    {
        for (var i = 0; i < Locations.Count; i++)
            if ((Locations[i].x == x) & (Locations[i].y == y))
                return Locations[i];
        Debug.LogError($"[LocationManager : GetLocation] Can't find locationsMark on x : {x}, y : {y}");
        return null;
    }

    private bool CheckLocationAvailability(int x, int y)
    {
        var location = GetLocation(x, y);
        if (location == null) return false;
        return location.available;
    }

    private bool CheckLandmarkAvailability(int x, int y, int width, int height)
    {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (CheckLocationAvailability(x + i, y + j) == false)
                return false;
        return true;
    }

    public void BuildNewLandmark(string modelId, int copyN, Price _buildPrice)
    {
        if (SettingMode) return; //building mode ongoing..

        var originalObj = FindAvailableObj(modelId);
        buildPrice = _buildPrice;

        if (originalObj == null)
            ////print("[LocationManager : BuildNewLandmark] Can't find modelId : " + modelId);
            return;

        targetObj = Instantiate(originalObj.GetComponent<LandmarkController>(), gameObject.transform);
        targetObj.LandMarkID = modelId;
        targetObj.CopyN = copyN;
        targetObj.enabled = false;
        targetObj.gameObject.SetActive(true);

        targetW = targetObj.GetComponent<LocationObject>().ScriptableObjet.Width;
        targetH = targetObj.GetComponent<LocationObject>().ScriptableObjet.Height;

        SettingMode = true;
        ToggleLocactionActive(true);

        targetX = -1;
        targetY = -1;

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(true);

        LocationMarkClicked(Mathf.RoundToInt(Locations.Count / 2f));
    }

    public void LocationMarkClicked(int idx)
    {
        UpdateLocations();

        var x = Locations[idx].x;
        var y = Locations[idx].y;

        var width = targetW;
        var height = targetH;
        targetX = x;
        targetY = y;

        if (width >= 3) x -= 1;
        if (height >= 3) y -= 1;

        if (CheckLandmarkAvailability(x, y, width, height))
        {
            targetAvialable = true;
            finishBtn.GetComponent<Button>().interactable = true;
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = possibleMaterial;
        }
        else
        {
            //Wrong Position
            targetAvialable = false;
            finishBtn.GetComponent<Button>().interactable = false;
            Debug.LogError(string.Format(
                "[LocationManager: LocationMarkClicked] Cannot Place in that location. x : {0} y : {1}", x, y));
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                if (GetLocation(x + i, y + j) != null)
                    GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = occupiedMaterial;
        }

        //move location
        if ((GetLocation(x, y) != null) & (GetLocation(x + width - 1, y + height - 1) != null))
        {
            targetObj.transform.position = Vector3.Lerp(GetLocation(x, y).GetCnerterPos(),
                GetLocation(x + width - 1, y + height - 1).GetCnerterPos(), 0.5f);
            targetObj.gameObject.SetActive(true);
            buildBtnSet.transform.position = Camera.main.WorldToScreenPoint(targetObj.transform.position);
        }
        else
        {
            targetObj.gameObject.SetActive(false);
        }

        //Setting MovePosBtn
        var left = true;
        var right = true;
        var up = true;
        var down = true;

        for (var i = 0; i < height; i++)
        {
            if (!CheckLocationAvailability(x + width, y + i)) right = false;
            if (!CheckLocationAvailability(x - 1, y + i)) left = false;
        }

        for (var i = 0; i < width; i++)
        {
            if (!CheckLocationAvailability(x + i, y + height)) up = false;
            if (!CheckLocationAvailability(x + i, y - 1)) down = false;
        }

        //rightBtn
        if (right)
        {
            rightBtn.transform.position = Vector3.Lerp(GetLocation(x + width, y).GetCnerterPos(),
                GetLocation(x + width, y + height - 1).GetCnerterPos(), 0.5f);
            rightBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        //leftBtn
        if (left)
        {
            leftBtn.transform.position = Vector3.Lerp(GetLocation(x - 1, y).GetCnerterPos(),
                GetLocation(x - 1, y + height - 1).GetCnerterPos(), 0.5f);
            leftBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        //upBtn
        if (up)
        {
            upBtn.transform.position = Vector3.Lerp(GetLocation(x, y + height).GetCnerterPos(),
                GetLocation(x + width - 1, y + height).GetCnerterPos(), 0.5f);
            upBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        //downBtn
        if (down)
        {
            downBtn.transform.position = Vector3.Lerp(GetLocation(x, y - 1).GetCnerterPos(),
                GetLocation(x + width - 1, y - 1).GetCnerterPos(), 0.5f);
            downBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        rightBtn.SetActive(right);
        leftBtn.SetActive(left);
        upBtn.SetActive(up);
        downBtn.SetActive(down);
    }

    public void LocationMarkMoveClicked(string direction)
    {
        switch (direction)
        {
            case "up":
                LocationMarkClicked(terrainManager.GetLocationIdx(targetX, targetY + 1));
                break;
            case "down":
                LocationMarkClicked(terrainManager.GetLocationIdx(targetX, targetY - 1));
                break;
            case "left":
                LocationMarkClicked(terrainManager.GetLocationIdx(targetX - 1, targetY));
                break;
            case "right":
                LocationMarkClicked(terrainManager.GetLocationIdx(targetX + 1, targetY));
                break;
        }
    }

    public void FinishBuilBtnPressed()
    {
        if (!targetAvialable)
        {
            Debug.LogError("[FinishBuilBtnPressed] targetAvialable = false");
            return;
        }

        AudioManager.Instance.PlaySFXbyTag(SFX_tag.BuildComplete);
        MoneyManager.Instance.SubtractMoney(buildPrice);

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);

        SettingMode = false;
        ToggleLocactionActive(false);

        if (targetW >= 3) targetX -= 1;
        if (targetH >= 3) targetY -= 1;

        AllocateLandmark(targetObj.gameObject);
        Camera.main.GetComponent<TouchEventManager>().IsInBuildMode = false;
        moveMode = false;
    }

    public void MoveBtnClicked(LandmarkController target)
    {
        buildPrice = new Price();
        moveMode = true;
        SettingMode = true;
        targetObj = target;

        targetX = targetObj.X;
        targetY = targetObj.Y;
        targetW = targetObj.ScriptableObjet.Width;
        targetH = targetObj.ScriptableObjet.Height;

        if (targetW >= 3) targetX += 1;
        if (targetH >= 3) targetY += 1;

        move_originalPosX = targetX;
        move_originalPosY = targetY;

        //
        AllocatedObj.Remove(targetObj);

        //
        ToggleLocactionActive(true);

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(true);

        touchCountDelta = -1;
        LocationMarkClicked(terrainManager.GetLocationIdx(targetX, targetY));
    }

    public void CancelBtnClicked()
    {
        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);
        SettingMode = false;
        ToggleLocactionActive(false);


        if (moveMode)
        {
            LocationMarkClicked(terrainManager.GetLocationIdx(move_originalPosX, move_originalPosY));
            upBtn.SetActive(false);
            rightBtn.SetActive(false);
            leftBtn.SetActive(false);
            downBtn.SetActive(false);
            AllocatedObj.Add(targetObj);
        }
        else
        {
            Destroy(targetObj);
        }

        moveMode = false;
        Camera.main.GetComponent<TouchEventManager>().IsInBuildMode = false;
    }

    private void AllocateLandmark(GameObject gameObject)
    {
        var obj = gameObject.GetComponent<LandmarkController>();
        var x = targetX;
        var y = targetY;
        var w = obj.ScriptableObjet.Width;
        var h = obj.ScriptableObjet.Height;


        if ((w != obj.ScriptableObjet.Width) | (h != obj.ScriptableObjet.Height))
        {
            Debug.LogError(
                $"[LocationManager : AllocateLandmark] Failed to allocate. W/H dismathced. w : {w}, h : {h}");
            return;
        }

        obj.X = x;
        obj.Y = y;
        obj.transform.position = Vector3.Lerp(GetLocation(x, y).GetCnerterPos(),
            GetLocation(x + w - 1, y + h - 1).GetCnerterPos(), 0.5f);

        targetObj.enabled = true;
        targetObj.Start();
        AllocatedObj.Add(obj);
        UpdateLocations();

        QuestTutorialManager.Instance.OilFarm0Build();
    }

    private void DislocateLandmark(GameObject gameObject)
    {
        var obj = gameObject.GetComponent<LandmarkController>();
        if (!AllocatedObj.Contains(obj)) return;
        AllocatedObj.Remove(obj);
        UpdateLocations();
    }

    public void UpdateLocations()
    {
        foreach (var location in Locations)
        {
            location.available = true;
            location.locationMark.GetComponent<MeshRenderer>().material = defaultMaterial;
            location.TerrainType = TerrainType.Sand;
        }

        foreach (var obj in AllocatedObj)
        {
            var x = obj.X;
            var y = obj.Y;
            var w = obj.ScriptableObjet.Width;
            var h = obj.ScriptableObjet.Height;

            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
            {
                GetLocation(x + i, y + j).available = false;
                GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = notPossibleMaterial;

                if (obj.ScriptableObjet.ModelIDFamily == "tree")
                    GetLocation(x + i, y + j).TerrainType = (TerrainType)3;
                else if (obj.ScriptableObjet.ModelIDFamily == "farm")
                    GetLocation(x + i, y + j).TerrainType = (TerrainType)2;
                else if (obj.ScriptableObjet.ModelIDFamily == "oilfall")
                    GetLocation(x + i, y + j).TerrainType = (TerrainType)3;
                else if (obj.ScriptableObjet.ModelIDFamily == "storm")
                    GetLocation(x + i, y + j).TerrainType = (TerrainType)2;
            }
        }

        terrainManager.UpdateTerrain();
    }

    private void ToggleLocactionActive(bool show)
    {
        foreach (var location in Locations) location.locationMark.SetActive(show);
    }

    public LandmarkController LevelUPLandmark(GameObject gameObject)
    {
        var landmark = gameObject.GetComponent<LandmarkController>();
        var newLandmark = Instantiate(FindAvailableObj(landmark.GetComponent<LocationObject>().NextLevelId))
            .GetComponent<LandmarkController>();

        if (newLandmark == null)
        {
            Debug.LogError("[LocationManager : LevelUPLandmark] Can't find LVUP model of : " + landmark.name);
            return null;
        }

        AllocatedObj.Remove(landmark);

        landmark.DestroyLandmark();
        newLandmark.transform.position = landmark.transform.position;
        newLandmark.ReadCSV();
        newLandmark.CopyN = landmark.GetComponent<LocationObject>().CopyN;
        newLandmark.X = landmark.GetComponent<LocationObject>().X;
        newLandmark.Y = landmark.GetComponent<LocationObject>().Y;
        newLandmark.gameObject.SetActive(true);
        newLandmark.enabled = true;
        newLandmark.Start();

        Destroy(landmark);

        AllocatedObj.Add(newLandmark);

        AudioManager.Instance.PlaySFXbyTag(SFX_tag.LevelUp);
        return newLandmark;
    }

    public GameObject FindAvailableObj(string modelID)
    {
        var landmarkItem = InfoDataManager.Instance.GetLandmarkItemByID(modelID);

        if (landmarkItem == null)
            ////print("[LocationManger - FindAvailableObj] Cannot find obj. modelID = " + modelID);
            return null;

        return landmarkItem.prefab;
    }

    public int CountObj(string id)
    {
        var count = 0;
        foreach (var obj in AllocatedObj)
            if (obj.ScriptableObjet.ModelIDFamily == id)
                count += 1;

        return count;
    }

    public void ResetAllocatedObj()
    {
        for (var i = AllocatedObj.Count - 1; i >= 0; i--)
        {
            Destroy(AllocatedObj[i]);
            AllocatedObj.RemoveAt(i);
        }
    }

    public void AddAllocatedObj(LocationObjData objData)
    {
        var originalObj = FindAvailableObj(objData.modelId);

        if (originalObj == null)
        {
            Debug.LogError("[LocationManager : BuildNewLandmark] Can't find modelId : " + objData.modelId);
            return;
        }

        var newObj = Instantiate(originalObj, gameObject.transform);
        var locationObj = newObj.GetComponent<LandmarkController>();

        locationObj.LandMarkID = objData.modelId;
        locationObj.CopyN = objData.copyN;
        locationObj.X = objData.x;
        locationObj.Y = objData.y;
        locationObj.ReadCSV(objData.buildTime);
        locationObj.transform.position = Vector3.Lerp(GetLocation(objData.x, objData.y).GetCnerterPos(),
            GetLocation(objData.x + locationObj.ScriptableObjet.Width - 1,
                objData.y + locationObj.ScriptableObjet.Height - 1).GetCnerterPos(), 0.5f);
        locationObj.gameObject.SetActive(true);
        locationObj.enabled = true;
        AllocatedObj.Add(locationObj);
    }
}