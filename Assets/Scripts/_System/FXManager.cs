using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
///     Manages the creation, initialization, and destruction of special effects (FX).
/// </summary>
public class FXManager : MonoBehaviour
{
    [SerializeField] private List<FXData> FXDatas;
    private readonly List<ObjectPool<FX>> FX_Pools = new();
    public static FXManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var fxdata in FXDatas)
        {
            var defaultCapacity = 10;
            var maxCapacity = 30;

            var new_pool = new ObjectPool<FX>(() =>
                {
                    var fx = Instantiate(fxdata.prefab);
                    fx.transform.SetParent(gameObject.transform);
                    return fx;
                }, fx => { fx.gameObject.SetActive(true); }, fx => { fx.gameObject.SetActive(false); },
                fx => { Destroy(fx.gameObject); }, true, defaultCapacity, maxCapacity);

            FX_Pools.Add(new_pool);
        }
    }

    public FX CreateFX(FXType fXType, Transform target = null, float sizeMultiplier = -1f)
    {
        var pool = GetObjectPoolByFxType(fXType);

        if (pool == null)
        {
            Debug.Log("FX data not found : " + fXType);
            return null;
        }

        var fx = pool.Get();
        if (sizeMultiplier != -1f) fx.transform.localScale = Vector3.one * sizeMultiplier;
        fx.InitAndPlayFX(target, fXType);
        return fx;
    }

    public void KillFX(FX fx)
    {
        GetObjectPoolByFxType(fx.GetFXType()).Release(fx);
    }

    private ObjectPool<FX> GetObjectPoolByFxType(FXType type)
    {
        var idx = -1;
        for (var i = 0; i < FXDatas.Count; i++)
            if (FXDatas[i].type == type)
            {
                idx = i;
                break;
            }

        if (idx != -1) return FX_Pools[idx];
        return null;
    }

    internal object CreateFX(FXType flyFX, Vector3 zero)
    {
        throw new NotImplementedException();
    }
}

public enum FXType
{
    FlyFX,
    PigiPopFX,
    UpgradeParticleFX
}

[Serializable]
public class FXData
{
    public FXType type;
    public FX prefab;
}