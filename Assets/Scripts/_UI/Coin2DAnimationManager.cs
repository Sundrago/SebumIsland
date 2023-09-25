using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public enum CoinType { Coin, Gem, Oil };
public class Coin2DAnimationManager : MonoBehaviour
{
    [SerializeField] private GameObject Coin_prefab, Gem_prefab, Oil_prefab;
    [SerializeField] private Transform coin_target, gem_target, oil_target;

    private List<ObjectPool<GameObject>> obj_pools = new List<ObjectPool<GameObject>>();

    

    private void Start()
    {
        foreach (CoinType type in Enum.GetValues(typeof(CoinType)))
        {
            int defaultCapacity = 5;
            int maxCapacity = 30;

            ObjectPool<GameObject> new_pool = new ObjectPool<GameObject>(() => {
                GameObject obj;
                switch (type)
                {
                    case CoinType.Coin:
                        obj = Instantiate(Coin_prefab, gameObject.transform);
                        break;
                    case CoinType.Gem:
                        obj = Instantiate(Gem_prefab, gameObject.transform);
                        break;
                    default: //CoinType.Oil:
                        obj = Instantiate(Oil_prefab, gameObject.transform);
                        break;
                }
                return obj;
            }, obj =>
            {
                obj.gameObject.SetActive(true);
            }, obj =>
            {
                obj.gameObject.SetActive(false);
            }, obj =>
            {
                Destroy(obj.gameObject);
            }, true, defaultCapacity, maxCapacity);

            obj_pools.Add(new_pool);
        }
    }
    
    public void AddCoin(CoinType type, Vector3 startPos, float _velocity, int count, float startAngle = 0f, float endAngle = 2f)
    {
        for (int i = 0; i < count; i++)
        {
            float durationFactor = Random.Range(0.75f, 1.5f);

            Vector3[] path = new Vector3[3];
            path[0] = startPos;

            GameObject obj = obj_pools[(int)type].Get();
            float velocity = _velocity * Random.Range(0.8f, 1.2f);

            switch (type)
            {
                case CoinType.Coin:
                    path[2] = coin_target.position;
                    break;
                case CoinType.Gem:
                    path[2] = gem_target.position;
                    break;
                default: //CoinType.Oil:
                    path[2] = oil_target.position;
                    break;
            }

            obj.transform.position = startPos;

            float angle = Random.Range(startAngle, endAngle) * Mathf.PI;
            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);

            Vector3 diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - (diff * Random.Range(0.3f, 0.8f));

            obj.transform.DOMove(path[0], 0.3f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() => {
                    obj.transform.DOPath(path, 0.7f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase((Ease.InOutCubic))
                        .OnComplete(() => {
                            obj_pools[(int)type].Release(obj);
                        });
                });
        }
    }

}
