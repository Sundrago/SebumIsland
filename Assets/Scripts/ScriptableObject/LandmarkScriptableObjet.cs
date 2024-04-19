using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Represents the data needed for spawning a Pigi (a type of object).
/// </summary>
[Serializable]
public class PigiSpawnData
{
    public string ID = "pigi_default";
    public float weightRatio = 1f;
    public float multiplier = 1f;

    [ReadOnly] public float minWeightRatio;

    [ReadOnly] public float maxWeightRatio;
}

/// <summary>
///     Represents the data needed for spawning a Pigi (a type of object).
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects", fileName = "Landmark")]
public class LandmarkScriptableObjet : ScriptableObject
{
    [field: Header("* 건물 ID")]
    [field: Required]
    [field: SerializeField]
    public string ModelID { get; private set; }

    [field: FormerlySerializedAs("modelID_family")]
    [field: Required]
    [field: SerializeField]
    public string ModelIDFamily { get; private set; }

    [field: FormerlySerializedAs("modelID_levelID")]
    [field: Required]
    [field: SerializeField]
    public int ModelIDLevelID { get; private set; }

    [field: Header("* 가로 세로 사이즈")]
    [field: Required]
    [field: SerializeField]
    public int Width { get; private set; }

    [field: Required]
    [field: SerializeField]
    public int Height { get; private set; }

    [field: Header("* 피지가 자라는 건물일 경우 체크")]
    [field: Required]
    [field: SerializeField]
    public bool IsLandmark { get; private set; }

    [field: Header("* 성장 피지 설정")]
    [field: Required]
    [field: TableList]
    [field: SerializeField]
    public PigiSpawnData[] PigiSpawnDatas { get; private set; }
}