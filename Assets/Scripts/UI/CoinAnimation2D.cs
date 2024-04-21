using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Represents a coin object and its animation.
/// </summary>
public class Coin
{
    public RectTransform coin;
    private bool onTransition;

    private Vector2 startPos, targetPos, endPos;
    private float startTime, endTime;

    public void SetupAnimation(Vector2 end, RectTransform obj, float velocity, Vector2 starts)
    {
        onTransition = true;
        startPos = starts;
        targetPos = new Vector2(startPos.x + Random.Range(0, Screen.width) - Screen.width / 2,
            Screen.height - Random.Range(0, Screen.height / 2f * velocity));
        endPos = end;
        startTime = Time.time;
        endTime = startTime + Vector2.Distance(startPos, targetPos) / 1250f;
        coin = obj;

        coin.transform.position = startPos;
    }

    public bool IsAnimFinished()
    {
        var normal = (Time.time - startTime) / (endTime - startTime);
        normal = normal == 1 ? 1 : 1 - Mathf.Pow(2, -10 * normal);

        if (normal > 0.995f)
        {
            onTransition = false;
            return true;
        }

        UpdateTransform(normal);
        return false;
    }

    private void UpdateTransform(float normal)
    {
        Vector2 a, b, c;
        a = Vector2.Lerp(startPos, targetPos, normal);
        b = Vector2.Lerp(targetPos, endPos, normal);
        c = Vector2.Lerp(a, b, normal);

        coin.transform.position = c;
    }
}

/// <summary>
///     Represents a class that handles the animation of coin objects.
/// </summary>
public class CoinAnimation2D : MonoBehaviour
{
    [SerializeField] private GameObject coin_ui;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject coin_holder;

    private readonly List<Coin> coins = new();

    private void Update()
    {
        for (var i = coins.Count - 1; i >= 0; i--)
            if (coins[i].IsAnimFinished())
            {
                Destroy(coins[i].coin.gameObject);
                coins.Remove(coins[i]);
            }
    }

    public void AddCoin(int count, Vector2 startPos)
    {
        for (var i = 0; i < count; i++)
        {
            var newCoinObj = Instantiate(coin, coin_holder.transform);
            var newCoin = new Coin();
            newCoinObj.SetActive(true);
            newCoin.SetupAnimation(coin_ui.GetComponent<RectTransform>().transform.position,
                newCoinObj.GetComponent<RectTransform>(), 1.5f, startPos);

            coins.Add(newCoin);
        }
    }
}