using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
// terrain idx 
0 : void
1 : sand
2 : ground
3 : grass
4 : water
*/

public class LocationManger : MonoBehaviour
{
    public class Location
    {
        public int x;
        public int y;
        public int terrain;
        public int idx;
        public bool available;
        public GameObject locationMark;
        public GameObject bgtile;
        public GameObject parent;


        public Location(int myX, int myY, GameObject parentObj, int myIdx, bool isAvailable = true)
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
            terrain = 1;
        }

        public Vector3 GetCnerterPos()
        {
            return new Vector3(5f * x, 0, 5f * y);
        }
    }

    public AudioCtrl myAudio;

    private List<Location> locations = new List<Location>();
    public List<GameObject> allocatedObj = new List<GameObject>();
    private bool settingMode = false;
    private int targetX, targetY, targetW, targetH;
    private bool targetAvialable;
    private GameObject targetObj;

    public int colCount, rowCount;

    public GameObject locationmarkPrefab, upBtn, downBtn, leftBtn, rightBtn;

    public Material defaultMaterial, occupiedMaterial, possibleMaterial, notPossibleMaterial;

    public List<GameObject> availableObj = new List<GameObject>();
    public GameObject finishBtn;
    public GameObject buildBtnSet;

    int touchCountDelta = -1;
    int targetIdxDelta = -1;
    private bool moveMode;
    int move_originalPosX, move_originalPosY;

    public Material[] mat_sand, mat_ground, mat_grass;

    //public GraphicRaycaster gr;

    // Start is called before the first frame update
    void Start()
    {
        locationmarkPrefab.SetActive(false);
        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);
        SetLocations(9, 9);
        UpdateTerrain();
        //BuildNewLandmark("oilfall", 0);
    }

    private void Update()
    {
        if (!settingMode) return;

        if (Input.touchCount == 0)
        {
            if(touchCountDelta!=0)
            {
                Camera.main.GetComponent<PinchZoom>().buildMode = false;
                touchCountDelta = 0;
            }
        }
        else if (Input.touchCount == 1)
        {
            if(touchCountDelta == -1)
            {
                touchCountDelta = 0;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if(touchCountDelta == 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "location")
                    {
                        int idx = hit.collider.gameObject.GetComponent<LocationMarkTouchEvent>().idx;
                        Camera.main.GetComponent<PinchZoom>().buildMode = true;
                        touchCountDelta = 1;
                        targetIdxDelta = idx;
                        print("drag begin : " + idx);
                        return;
                    }
                }
            }

            //return if UI element is clicked
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "location")
                {
                    int idx = hit.collider.gameObject.GetComponent<LocationMarkTouchEvent>().idx;
                    if(idx == targetIdxDelta) return;
                    targetIdxDelta = idx;
                    print("clcicked");
                    LocationMarkClicked(idx);
                    touchCountDelta = 1;
                }
            }
        }
    }

    private void SetLocations(int row, int col)
    {
        colCount = col;
        rowCount = row;

        foreach(Location location in locations)
        {
            Destroy(location.locationMark);
        }
        locations = new List<Location>();

        for (int i = 0; i < rowCount; i++)
        {
            for(int j = 0; j < colCount; j++)
            {
                int x = i - (rowCount - 1) / 2;
                int y = j - (colCount - 1) / 2;

                GameObject mark = Instantiate(locationmarkPrefab, gameObject.transform);
                mark.transform.Find("LOCATION_MARK").gameObject.GetComponent<LocationMarkTouchEvent>().idx = locations.Count;
                Location location = new Location(x, y,mark,locations.Count);
                locations.Add(location);
            }
        }
    }

    private void UpdateTerrain() {
        print("TerrainUpdated ");
        foreach(Location location in locations) {
            int[] shape = GetTerrainShape(location);

            if(location.terrain == 1) { 
                //sand
                location.bgtile.GetComponent<MeshRenderer>().material = mat_sand[shape[0]];

            } else if(location.terrain == 2) {
                location.bgtile.GetComponent<MeshRenderer>().material = mat_ground[shape[0]];
                //ground
            } else if(location.terrain == 3) {
                location.bgtile.GetComponent<MeshRenderer>().material = mat_grass[shape[0]];
                //grass
            } 

            location.bgtile.transform.localEulerAngles = new Vector3(0,shape[1],0);

            //terrain Side
        }
    }

    private int[] GetTerrainShape(Location location) {
        int shape = 0;
        int degree = 0;

        bool[] nearTerrain = new bool[4]{
            IsTerrainNear(location.x, location.y + 1, location.terrain) // 0 => down
            , IsTerrainNear(location.x - 1, location.y, location.terrain) // 1 => left
            , IsTerrainNear(location.x, location.y - 1, location.terrain) // 2 => up
            , IsTerrainNear(location.x + 1, location.y, location.terrain) // 3 => right
            }; 

        int count = 0; //nearTerrain[0] + nearTerrain[1] + nearTerrain[2] + nearTerrain[3];
        for(int i = 0; i<4; i++) {
            if(nearTerrain[i]) count += 1;
        }

        if(count == 3) {
            shape = 1;
            if(!nearTerrain[0]) degree = 0; 
            if(!nearTerrain[1]) degree = 270;
            if(!nearTerrain[2]) degree = 180;
            if(!nearTerrain[3]) degree = 90;
        } else if(count == 2) {
            shape = 2;
            if(!nearTerrain[0] & !nearTerrain[1]) degree = 270; 
            if(!nearTerrain[1] & !nearTerrain[2]) degree = 180;
            if(!nearTerrain[2] & !nearTerrain[3]) degree = 90;
            if(!nearTerrain[3] & !nearTerrain[0]) degree = 0;
        }
        
        int[] returnList = new int[2]{shape, degree};
        return returnList;
    }

    private bool IsTerrainNear(int x, int y, int targetTerrain) {
        int idx = GetLocationIdx(x, y);

        if(idx < 0) return false;
        if(idx >= locations.Count) return false;

        if (locations[idx].terrain == targetTerrain) return true;
        else return false;
    }

    private Location GetLocation(int x, int y)
    {
        for(int i = 0; i < locations.Count; i++)
        {
            if (locations[i].x == x & locations[i].y == y) return locations[i];
        }
        print(string.Format("[LocationManager : GetLocation] Can't find locationsMark on x : {0}, y : {1}", x, y));
        return null;
    }

    private int GetLocationIdx(int x, int y)
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].x == x & locations[i].y == y) return i;
        }
        print(string.Format("[LocationManager : GetLocationIdx] Can't find locationsIDX on x : {0}, y : {1}", x, y));
        return -1;
    }

    private bool CheckLocationAvailability(int x, int y)
    {
        Location location = GetLocation(x, y);
        if (location == null) return false;
        return location.available;
    }

    private bool CheckLandmarkAvailability(int x, int y, int width, int height)
    {
        for(int i = 0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                if(CheckLocationAvailability(x + i, y + j) == false) return false;
            }
        }
        return true;
    }

    public void BuildNewLandmark(string modelId, int copyN)
    {
        if (settingMode) return; //building mode ongoing..

        GameObject originalObj = FindAvailableObj(modelId);

        if(originalObj == null)
        {
            print("[LocationManager : BuildNewLandmark] Can't find modelId : " + modelId);
            return;
        }

        targetObj = Instantiate(originalObj, gameObject.transform);
        targetObj.GetComponent<LocationObject>().landMarkID = modelId;
        targetObj.GetComponent<LocationObject>().copyN = copyN;
        targetObj.GetComponent<Landmark>().enabled = false;
        targetObj.SetActive(true);

        targetW = targetObj.GetComponent<LocationObject>().width;
        targetH = targetObj.GetComponent<LocationObject>().height;

        settingMode = true;
        ToggleLocactionActive(true);

        targetX = -1;
        targetY = -1;

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(true);

        LocationMarkClicked(Mathf.RoundToInt(locations.Count / 2f));
        //ShowLandMarks

    }

    public void LocationMarkClicked(int idx)
    {
        UpdateLocations();

        int x = locations[idx].x;
        int y = locations[idx].y;

        print(string.Format("x : {0}, y : {1}", x, y));

        int width = targetW;
        int height = targetH;
        targetX = x;
        targetY = y;

        if (width >= 3) x -= 1;
        if (height >= 3) y -= 1;

        if (CheckLandmarkAvailability(x, y, width, height))
        {
            targetAvialable = true;
            finishBtn.GetComponent<Button>().interactable = true;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = possibleMaterial;
                }
            }
        } else
        {
            //Wrong Position
            targetAvialable = false;
            finishBtn.GetComponent<Button>().interactable = false;
            print(string.Format("[LocationManager: LocationMarkClicked] Cannot Place in that location. x : {0} y : {1}", x, y));
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (GetLocation(x + i, y + j) != null)
                    {
                        GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = occupiedMaterial;
                    }
                }
            }
        }

        //move location
        if (GetLocation(x, y) != null & GetLocation(x + width - 1, y + height - 1) != null) {
            targetObj.transform.position = Vector3.Lerp(GetLocation(x, y).GetCnerterPos(), GetLocation(x + width - 1, y + height - 1).GetCnerterPos(), 0.5f);
            targetObj.SetActive(true);
            buildBtnSet.transform.position = Camera.main.WorldToScreenPoint(targetObj.transform.position);
        }
        else targetObj.SetActive(false);

        //Setting MovePosBtn
        bool left = true;
        bool right = true;
        bool up = true;
        bool down = true;

        for(int i = 0; i < height; i++)
        {
            if (!CheckLocationAvailability(x + width, y + i)) right = false;
            if (!CheckLocationAvailability(x - 1, y + i)) left = false;
        }

        for (int i = 0; i < width; i++)
        {
            if (!CheckLocationAvailability(x + i, y + height)) up = false;
            if (!CheckLocationAvailability(x + i, y - 1)) down = false;
        }

        //rightBtn
        if (right)
        {
            rightBtn.transform.position = Vector3.Lerp(GetLocation(x + width, y).GetCnerterPos(), GetLocation(x + width, y + height - 1).GetCnerterPos(), 0.5f);
            rightBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }
        //leftBtn
        if (left)
        {
            leftBtn.transform.position = Vector3.Lerp(GetLocation(x - 1, y).GetCnerterPos(), GetLocation(x - 1, y + height -1).GetCnerterPos(), 0.5f);
            leftBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }
        //upBtn
        if (up)
        {
            upBtn.transform.position = Vector3.Lerp(GetLocation(x, y + height).GetCnerterPos(), GetLocation(x + width - 1, y + height).GetCnerterPos(), 0.5f);
            upBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }
        //downBtn
        if (down)
        {
            downBtn.transform.position = Vector3.Lerp(GetLocation(x, y - 1).GetCnerterPos(), GetLocation(x + width -1, y - 1).GetCnerterPos(), 0.5f);
            downBtn.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        rightBtn.SetActive(right);
        leftBtn.SetActive(left);
        upBtn.SetActive(up);
        downBtn.SetActive(down);
    }

    public void LocationMarkMoveClicked(string direction)
    {
        switch(direction)
        {
            case "up":
                LocationMarkClicked(GetLocationIdx(targetX, targetY + 1));
                break;
            case "down":
                LocationMarkClicked(GetLocationIdx(targetX, targetY - 1));
                break;
            case "left":
                LocationMarkClicked(GetLocationIdx(targetX - 1, targetY));
                break;
            case "right":
                LocationMarkClicked(GetLocationIdx(targetX + 1, targetY));
                break;

        }
        print(direction);
    }

    public void FinishBuilBtnPressed()
    {
        if(!targetAvialable)
        {
            print("[FinishBuilBtnPressed] targetAvialable = false");
            return;
        }

        if (!moveMode) myAudio.PlaySFX(2);

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);

        settingMode = false;
        ToggleLocactionActive(false);

        if (targetW >= 3) targetX -= 1;
        if (targetH >= 3) targetY -= 1;

        AllocateLandmark(targetObj);
        Camera.main.GetComponent<PinchZoom>().buildMode = false;
        moveMode = false;
    }

    public void MoveBtnClicked(GameObject target)
    {
        moveMode = true;
        settingMode = true;
        targetObj = target;

        targetX = targetObj.GetComponent<LocationObject>().x;
        targetY = targetObj.GetComponent<LocationObject>().y;
        targetW = targetObj.GetComponent<LocationObject>().width;
        targetH = targetObj.GetComponent<LocationObject>().height;

        if (targetW >= 3) targetX += 1;
        if (targetH >= 3) targetY += 1;

        move_originalPosX = targetX;
        move_originalPosY = targetY;

        //
        allocatedObj.Remove(targetObj);

        //
        ToggleLocactionActive(true);

        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(true);

        touchCountDelta = -1;
        print(string.Format("X : {0} Y : {1} idx : {2}", targetX, targetY, GetLocationIdx(targetX, targetY)));
        LocationMarkClicked(GetLocationIdx(targetX, targetY));
    }

    public void CancelBtnClicked()
    {
        upBtn.SetActive(false);
        rightBtn.SetActive(false);
        leftBtn.SetActive(false);
        downBtn.SetActive(false);
        buildBtnSet.SetActive(false);
        settingMode = false;
        ToggleLocactionActive(false);

        //if (targetW >= 3) targetX -= 1;
        //if (targetH >= 3) targetY -= 1;

        if (moveMode)
        {
            LocationMarkClicked(GetLocationIdx(move_originalPosX, move_originalPosY));
            upBtn.SetActive(false);
            rightBtn.SetActive(false);
            leftBtn.SetActive(false);
            downBtn.SetActive(false);
            allocatedObj.Add(targetObj);
        }
        else
        {
            Destroy(targetObj);
        }

        moveMode = false;
        Camera.main.GetComponent<PinchZoom>().buildMode = false;
    }

    private void AllocateLandmark(GameObject obj)
    {
        int x = targetX;
        int y = targetY;
        int w = obj.GetComponent<LocationObject>().width;
        int h = obj.GetComponent<LocationObject>().height;


        if (w != obj.GetComponent<LocationObject>().width | h!= obj.GetComponent<LocationObject>().height)
        {
            print(string.Format("[LocationManager : AllocateLandmark] Failed to allocate. W/H dismathced. w : {0}, h : {1}", w, h));
            return; 
        }
         
        obj.GetComponent<LocationObject>().x = x;
        obj.GetComponent<LocationObject>().y = y;
        obj.transform.position = Vector3.Lerp(GetLocation(x, y).GetCnerterPos(), GetLocation(x + w - 1, y + h - 1).GetCnerterPos(), 0.5f);

        targetObj.GetComponent<Landmark>().enabled = true;
        targetObj.GetComponent<Landmark>().Start();
        //Camera.main.GetComponent<ParamsData>().landmarks.Add(obj);
        allocatedObj.Add(obj);
        UpdateLocations();

        //SavePositionData
    }

    private void DislocateLandmark(GameObject obj)
    {
        if (!allocatedObj.Contains(obj)) return;
        allocatedObj.Remove(obj);
        UpdateLocations();
    }

    public void UpdateLocations()
    {
        foreach (Location location in locations)
        {
            location.available = true;
            location.locationMark.GetComponent<MeshRenderer>().material = defaultMaterial;
            location.terrain = 1;
        }
        foreach (GameObject obj in allocatedObj)
        {
            int x = obj.GetComponent<LocationObject>().x;
            int y = obj.GetComponent<LocationObject>().y;
            int w = obj.GetComponent<LocationObject>().width;
            int h = obj.GetComponent<LocationObject>().height;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    GetLocation(x + i, y + j).available = false;
                    GetLocation(x + i, y + j).locationMark.GetComponent<MeshRenderer>().material = notPossibleMaterial;

                    if(obj.GetComponent<LocationObject>().modelID_family == "tree") 
                        GetLocation(x + i, y + j).terrain = 3;
                    else if(obj.GetComponent<LocationObject>().modelID_family == "farm") 
                        GetLocation(x + i, y + j).terrain = 2;
                    else if(obj.GetComponent<LocationObject>().modelID_family == "oilfall") 
                        GetLocation(x + i, y + j).terrain = 3;
                    else if(obj.GetComponent<LocationObject>().modelID_family == "storm") 
                        GetLocation(x + i, y + j).terrain = 2;
                }
            }
        }

        UpdateTerrain();
    }

    private void ToggleLocactionActive(bool show)
    {
        foreach (Location location in locations)
        {
            location.locationMark.SetActive(show);
        }
    }

    public GameObject LevelUPLandmark(GameObject landmark)
    {
        GameObject newLandmark = Instantiate(FindAvailableObj(landmark.GetComponent<LocationObject>().nextLevelId));
        
        if (newLandmark == null)
        {
            print("[LocationManager : LevelUPLandmark] Can't find LVUP model of : " + landmark.name);
            return null;
        }

        allocatedObj.Remove(landmark);

        landmark.GetComponent<Landmark>().DestroyLandmark();
        newLandmark.transform.position = landmark.transform.position;
        newLandmark.GetComponent<LocationObject>().ReadCSV();
        newLandmark.GetComponent<LocationObject>().copyN = landmark.GetComponent<LocationObject>().copyN;
        newLandmark.GetComponent<LocationObject>().x = landmark.GetComponent<LocationObject>().x;
        newLandmark.GetComponent<LocationObject>().y = landmark.GetComponent<LocationObject>().y;
        newLandmark.SetActive(true);
        newLandmark.GetComponent<Landmark>().enabled = true;
        newLandmark.GetComponent<Landmark>().Start();

        Destroy(landmark);

        allocatedObj.Add(newLandmark);
        myAudio.PlaySFX(3);

        return newLandmark;
    }

    public GameObject FindAvailableObj(string modelID)
    {
        foreach(GameObject obj in availableObj)
        {
            if(obj.GetComponent<LocationObject>().modelID == modelID)
            {
                return obj;
            }
        }
        print("[LocationManger - FindAvailableObj] Cannot find obj. modelID = " + modelID);
        return null;
    }

    public int CountObj(string id)
    {
        int count = 0;
        foreach (GameObject obj in allocatedObj)
        {
            if (obj.GetComponent<LocationObject>().modelID_family == id) count += 1;
        }
        
        return count;
    }

    public void ResetAllocatedObj()
    {
        for(int i = allocatedObj.Count-1; i>=0; i--)
        {
            Destroy(allocatedObj[i]);
            allocatedObj.RemoveAt(i);
        }
    }

    public void AddAllocatedObj(LocationObjData objData)
    {
        GameObject originalObj = FindAvailableObj(objData.modelId);

        if (originalObj == null)
        {
            print("[LocationManager : BuildNewLandmark] Can't find modelId : " + objData.modelId);
            return;
        }

        GameObject newObj = Instantiate(originalObj, gameObject.transform);
        LocationObject locationObj = newObj.GetComponent<LocationObject>();

        locationObj.landMarkID = objData.modelId;
        locationObj.copyN = objData.copyN;
        locationObj.x = objData.x;
        locationObj.y = objData.y;
        locationObj.ReadCSV(objData.buildTime);

        newObj.transform.position = Vector3.Lerp(GetLocation(objData.x, objData.y).GetCnerterPos(), GetLocation(objData.x + locationObj.width - 1, objData.y + locationObj.height - 1).GetCnerterPos(), 0.5f);

        newObj.SetActive(true);
        newObj.GetComponent<Landmark>().enabled = true;
        newObj.GetComponent<Landmark>().Start();
        allocatedObj.Add(newObj);
    }
}
