using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;

    [SerializeField] List<FXData> FXDatas;
    private readonly List<ObjectPool<FX>> FX_Pools = new List<ObjectPool<FX>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (FXData fxdata in FXDatas)
        {
            int defaultCapacity = 10;
            int maxCapacity = 30;

            //switch (fxdata.type)
            //{
            //    case FXType.SwordHitFX2:
            //        defaultCapacity = 5;
            //        maxCapacity = 10;
            //        break;
            //    case FXType.SpiderAttackFX:
            //        defaultCapacity = 10;
            //        maxCapacity = 15;
            //        break;
            //    case FXType.LevelUPFX:
            //        defaultCapacity = 1;
            //        maxCapacity = 1;
            //        break;
            //    case FXType.DespawnFX:
            //        defaultCapacity = 5;
            //        maxCapacity = 10;
            //        break;
            //}

            ObjectPool<FX> new_pool = new ObjectPool<FX>(() =>
            {
                FX fx = Instantiate(fxdata.prefab);
                fx.transform.SetParent(gameObject.transform);
                return fx;
            }, fx =>
            {
                fx.gameObject.SetActive(true);
            }, fx =>
            {
                fx.gameObject.SetActive(false);
            }, fx =>
            {
                Destroy(fx.gameObject);
            }, true, defaultCapacity, maxCapacity);

            FX_Pools.Add(new_pool);
        }
    }

    public FX CreateFX(FXType fXType, Transform target = null)
    {
        ObjectPool<FX> pool = GetObjectPoolByFxType(fXType);

        if (pool == null)
        {
            Debug.Log("FX data not found : " + fXType.ToString());
            return null;
        }

        FX fx = pool.Get();
        fx.InitAndPlayFX(target, fXType);
        return fx;
    }

    public void KillFX(FX fx)
    {
        GetObjectPoolByFxType(fx.GetFXType()).Release(fx);
    }

    private ObjectPool<FX> GetObjectPoolByFxType(FXType type)
    {
        int idx = -1;
        for (int i = 0; i < FXDatas.Count; i++)
        {
            if (FXDatas[i].type == type)
            {
                idx = i;
                break;
            }
        }

        if (idx != -1) return FX_Pools[idx];
        else return null;
    }

    internal object CreateFX(FXType flyFX, Vector3 zero)
    {
        throw new NotImplementedException();
    }
}

public enum FXType
{
    FlyFX, PigiPopFX, UpgradeParticleFX
}

[Serializable]
public class FXData
{
    public FXType type;
    public FX prefab;
}
