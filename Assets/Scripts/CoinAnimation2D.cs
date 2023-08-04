using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin
{
    Vector2 startPos, targetPos, endPos;
    bool onTransition;
    float startTime, endTime;
    public RectTransform coin;

    public void Setup(Vector2 end, RectTransform obj, float velocity, Vector2 starts)
    {
        onTransition = true;
        startPos = starts;
        targetPos = new Vector2(startPos.x + Random.Range(0, Screen.width) - Screen.width/2, Screen.height - Random.Range(0, Screen.height * velocity));
        endPos = end;
        startTime = Time.time;
        endTime = startTime + Vector2.Distance(startPos, targetPos) / 1250f;
        coin = obj;

        coin.transform.position = startPos;
    }

    public bool Update()
    {
        float normal = (Time.time - startTime) / (endTime - startTime);
        normal = normal == 1 ? 1 : 1 - Mathf.Pow(2, -10 * normal);

        if (normal > 0.995f)
        {
            onTransition = false;
            return true;
        }

        Vector2 a, b, c;
        a = Vector2.Lerp(startPos, targetPos, normal);
        b = Vector2.Lerp(targetPos, endPos, normal);
        c = Vector2.Lerp(a, b, normal);

        coin.transform.position = c;
        return false;
    }
}

public class CoinAnimation2D : MonoBehaviour
{
    public GameObject coin_ui;
    public GameObject coin;
    public GameObject coin_holder;

    List<Coin> coins = new List<Coin>();

    // Update is called once per frame
    void Update()
    {
        for(int i = coins.Count - 1; i>=0; i--)
        {
            if(coins[i].Update())
            {
                Destroy(coins[i].coin.gameObject);
                coins.Remove(coins[i]);
            }
        }
    }

    public void Addcoin(int count, Vector2 startPos)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newCoinObj = Instantiate(coin, coin_holder.transform);
            Coin newCoin = new Coin();
            newCoinObj.SetActive(true);
            newCoin.Setup(coin_ui.GetComponent<RectTransform>().transform.position, newCoinObj.GetComponent<RectTransform>(), 1.5f, startPos);

            coins.Add(newCoin);
        }
    }
}
