using UnityEngine;

/// <summary>
///     Class responsible for managing the terrain in the game.
/// </summary>
public class TerrainManager : MonoBehaviour
{
    [SerializeField] private LocationManger locationManger;

    public Material[] mat_sand;
    public Material[] mat_ground;
    public Material[] mat_grass;

    public void UpdateTerrain()
    {
        foreach (var location in locationManger.Locations)
        {
            var shape = GetTerrainShape(location);

            if (location.TerrainType == TerrainType.Sand)
                location.bgtile.GetComponent<MeshRenderer>().material = mat_sand[shape[0]];
            else if (location.TerrainType == TerrainType.Ground)
                location.bgtile.GetComponent<MeshRenderer>().material = mat_ground[shape[0]];
            else if (location.TerrainType == TerrainType.Grass)
                location.bgtile.GetComponent<MeshRenderer>().material = mat_grass[shape[0]];

            location.bgtile.transform.localEulerAngles = new Vector3(0, shape[1], 0);
        }
    }

    private int[] GetTerrainShape(LocationData location)
    {
        var shape = 0;
        var degree = 0;

        var nearTerrain = new bool[4]
        {
            IsTerrainNear(location.x, location.y + 1, location.TerrainType) // 0 => down
            ,
            IsTerrainNear(location.x - 1, location.y, location.TerrainType) // 1 => left
            ,
            IsTerrainNear(location.x, location.y - 1, location.TerrainType) // 2 => up
            ,
            IsTerrainNear(location.x + 1, location.y, location.TerrainType) // 3 => right
        };

        var count = 0;
        for (var i = 0; i < 4; i++)
            if (nearTerrain[i])
                count += 1;

        if (count == 3)
        {
            shape = 1;
            if (!nearTerrain[0]) degree = 0;
            if (!nearTerrain[1]) degree = 270;
            if (!nearTerrain[2]) degree = 180;
            if (!nearTerrain[3]) degree = 90;
        }
        else if (count == 2)
        {
            shape = 2;
            if (!nearTerrain[0] & !nearTerrain[1]) degree = 270;
            if (!nearTerrain[1] & !nearTerrain[2]) degree = 180;
            if (!nearTerrain[2] & !nearTerrain[3]) degree = 90;
            if (!nearTerrain[3] & !nearTerrain[0]) degree = 0;
        }

        var returnList = new int[2] { shape, degree };
        return returnList;
    }

    private bool IsTerrainNear(int x, int y, TerrainType targetTerrain)
    {
        var idx = GetLocationIdx(x, y);

        if (idx < 0) return false;
        if (idx >= locationManger.Locations.Count) return false;

        if (locationManger.Locations[idx].TerrainType == targetTerrain) return true;
        return false;
    }

    public int GetLocationIdx(int x, int y)
    {
        for (var i = 0; i < locationManger.Locations.Count; i++)
            if ((locationManger.Locations[i].x == x) & (locationManger.Locations[i].y == y))
                return i;
        Debug.LogError($"[LocationManager : GetLocationIdx] Can't find locationsIDX on x : {x}, y : {y}");
        return -1;
    }
}