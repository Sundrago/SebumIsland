using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum CoinType
{
    Coin,
    Gem,
    Oil
}

/// <summary>
///     Handles the animation and movement of coins, gems, and oil in the game.
/// </summary>
public class Coin2DAnimationManager : MonoBehaviour
{
    [FormerlySerializedAs("Coin_prefab")] [SerializeField]
    private GameObject coinPrefab;

    [FormerlySerializedAs("Gem_prefab")] [SerializeField]
    private GameObject gemPrefab;

    [FormerlySerializedAs("Oil_prefab")] [SerializeField]
    private GameObject oilPrefab;

    [FormerlySerializedAs("coin_target")] [SerializeField]
    private Transform coinTarget;

    [FormerlySerializedAs("gem_target")] [SerializeField]
    private Transform gemTarget;

    [FormerlySerializedAs("oil_target")] [SerializeField]
    private Transform oilTarget;

    private readonly List<ObjectPool<GameObject>> obj_pools = new();

    private void Start()
    {
        foreach (CoinType type in Enum.GetValues(typeof(CoinType)))
        {
            var defaultCapacity = 5;
            var maxCapacity = 30;

            var new_pool = new ObjectPool<GameObject>(() =>
                {
                    GameObject obj;
                    switch (type)
                    {
                        case CoinType.Coin:
                            obj = Instantiate(coinPrefab, gameObject.transform);
                            break;
                        case CoinType.Gem:
                            obj = Instantiate(gemPrefab, gameObject.transform);
                            break;
                        default: //CoinType.Oil:
                            obj = Instantiate(oilPrefab, gameObject.transform);
                            break;
                    }

                    return obj;
                }, obj => { obj.gameObject.SetActive(true); }, obj => { obj.gameObject.SetActive(false); },
                obj => { Destroy(obj.gameObject); }, true, defaultCapacity, maxCapacity);

            obj_pools.Add(new_pool);
        }
    }

    public void AddCoin(CoinType type, Vector3 startPos, float _velocity, int count, float startAngle = 0f,
        float endAngle = 2f)
    {
        for (var i = 0; i < count; i++)
        {
            var durationFactor = Random.Range(0.75f, 1.5f);

            var path = new Vector3[3];
            path[0] = startPos;

            var obj = obj_pools[(int)type].Get();
            var velocity = _velocity * Random.Range(0.8f, 1.2f);

            switch (type)
            {
                case CoinType.Coin:
                    path[2] = coinTarget.position;
                    break;
                case CoinType.Gem:
                    path[2] = gemTarget.position;
                    break;
                default:
                    path[2] = oilTarget.position;
                    break;
            }

            AnimateCoinObject(type, startPos, startAngle, endAngle, obj, path, velocity, durationFactor);
        }
    }

    private void AnimateCoinObject(CoinType type, Vector3 startPos, float startAngle, float endAngle, GameObject obj,
        Vector3[] path, float velocity, float durationFactor)
    {
        obj.transform.position = startPos;

        var angle = Random.Range(startAngle, endAngle) * Mathf.PI;
        path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);

        var diff = startPos - path[0];
        path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - diff * Random.Range(0.3f, 0.8f);

        obj.transform.DOMove(path[0], 0.3f * durationFactor)
            .SetEase(Ease.OutCirc)
            .OnComplete(() =>
            {
                obj.transform.DOPath(path, 0.7f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(() => { obj_pools[(int)type].Release(obj); });
            });
    }
}