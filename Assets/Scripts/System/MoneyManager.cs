using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// This class represents a price, consisting of an amount (integer) and a character code (string).
public class Price
{
    public int amount;
    public string charCode;
    public int idx;

    public Price(int amt = 0, string idxCode = "a")
    {
        if (amount < 10000)
        {
            amount = amt;
            charCode = idxCode;
            idx = ConvertCodeToInt(idxCode);
        }

        if (amount >= 10000) UpdateUnit();
    }

    private void UpdateUnit()
    {
        amount = Mathf.RoundToInt(amount / 10000);
        idx = idx += 1;
        charCode = ConvertIntToCode(idx);

        if (amount >= 10000) UpdateUnit();
    }

    public string GetString()
    {
        return amount + charCode;
    }

    public int ConvertCodeToInt(string code)
    {
        var index = 0;
        index += code[0] - 'a';
        return index;
    }

    public string ConvertIntToCode(int idx)
    {
        var code = "";

        if (idx >= 26)
        {
            var idxCode = (int)Math.Truncate(idx / 26f) + 96;
            var myChar = Convert.ToChar(idxCode);
            code += myChar;

            idx = idx % 26;
        }

        if (idx >= 0)
        {
            char myChar;
            myChar = Convert.ToChar(idx + 97);

            code += myChar;
        }

        return code;
    }
}

/// <summary>
///     Manages money-related operations such as adding, subtracting, and checking money balance.
/// </summary>
public class MoneyManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI moneyText, gemText, oilText;

    [FormerlySerializedAs("remoteUpgrade")] [SerializeField]
    private RemoteUpgradeManager remoteUpgradeManager;

    [FormerlySerializedAs("buildPanelCtrl")] [SerializeField]
    private BuildPanel buildPanel;

    [SerializeField] private Coin2DAnimationManager coin2D;

    public List<Price> balcance = new();
    private int gemAmount;
    private int oilAmount;
    public static MoneyManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (var i = balcance.Count - 1; i >= 0; i--)
            if (balcance[i].amount > 0)
            {
                moneyText.text = (i > 0) & (balcance[i].amount < 10)
                    ? balcance[i].amount + "." + Mathf.Floor(balcance[i - 1].amount / 1000f) + balcance[i].charCode
                    : balcance[i].amount + balcance[i].charCode;
                break;
            }
            else
            {
                if (i == 0) moneyText.text = GetMyBalance().GetString();
            }

        if (balcance.Count == 0) moneyText.text = "0a";

        oilText.text = oilAmount.ToString();
        gemText.text = gemAmount.ToString();
    }

    private string ConvertIntToCode(int idx)
    {
        var code = "";

        if (idx >= 26)
        {
            var idxCode = (int)Math.Truncate(idx / 26f) + 96;
            var myChar = Convert.ToChar(idxCode);
            code += myChar;

            idx = idx % 26;
        }

        if (idx >= 0)
        {
            char myChar;
            myChar = Convert.ToChar(idx + 97);

            code += myChar;
        }

        return code;
    }

    public void AddMoney(Price myPrice)
    {
        var money_idx = myPrice.idx;

        if (balcance.Count <= money_idx)
        {
            var addAmount = money_idx - balcance.Count;
            for (var i = 0; i <= addAmount; i++)
                balcance.Add(new Price(0, ConvertIntToCode(balcance.Count)));
        }

        balcance[money_idx].amount += myPrice.amount;
        CheckIfExceed(money_idx);
        UpdateUI();
        SetBtnAvailabilty();
    }

    public void AddGemOil(CoinType coinType, int amount)
    {
        if (coinType == CoinType.Gem)
            gemAmount += amount;
        else if (coinType == CoinType.Oil)
            oilAmount += amount;
        UpdateUI();
    }

    public bool HasEnoughGemOil(CoinType coinType, int amount)
    {
        if (coinType == CoinType.Gem)
        {
            if (gemAmount >= amount) return true;
            return false;
        }

        if (coinType == CoinType.Oil)
        {
            if (oilAmount >= amount) return true;
            return false;
        }

        return false;
    }

    public bool SubtractGemOil(CoinType coinType, int amount)
    {
        if (!HasEnoughGemOil(coinType, amount)) return false;

        if (coinType == CoinType.Gem)
        {
            gemAmount -= amount;
            return true;
        }

        if (coinType == CoinType.Oil)
        {
            oilAmount -= amount;
            return true;
        }

        return false;
    }

    public int GetMyGemOil(CoinType coinType)
    {
        if (coinType == CoinType.Gem) return gemAmount;
        if (coinType == CoinType.Oil) return oilAmount;

        return -1;
    }

    public void AddCoin2D(int count, Vector3 startPos)
    {
        coin2D.AddCoin(CoinType.Coin, startPos, 250f, count, 0.5f, 1.5f);
    }

    private void CheckIfExceed(int idx)
    {
        if (balcance[idx].amount >= 10000)
        {
            balcance[idx].amount -= 10000;

            if (balcance.Count <= idx + 1) balcance.Add(new Price(0, ConvertIntToCode(idx + 1)));
            balcance[idx + 1].amount += 1;
            CheckIfExceed(idx + 1);
        }
    }

    public bool SubtractMoney(Price price)
    {
        if (!HasEnoughMoney(price)) return false;

        balcance[price.idx].amount -= price.amount;
        CheckIfShort(price.idx);
        UpdateUI();
        SetBtnAvailabilty();
        return true;
    }

    private void CheckIfShort(int idx)
    {
        if (balcance[idx].amount < 0)
        {
            balcance[idx].amount += 10000;
            balcance[idx + 1].amount -= 1;
            CheckIfShort(idx + 1);
        }
    }

    public Price GetMyBalance()
    {
        for (var i = balcance.Count - 1; i >= 0; i--)
            if (balcance[i].amount > 0)
                return new Price(balcance[i].amount, balcance[i].charCode);
        return new Price();
    }

    public bool HasEnoughMoney(Price price)
    {
        var balance = GetMyBalance();
        if (balance.idx > price.idx) return true;

        if (balance.idx == price.idx)
        {
            if (balance.amount >= price.amount) return true;
            return false;
        }

        return false;
    }

    public void ResetMoney()
    {
        balcance = new List<Price>();
        UpdateUI();
    }

    private void SetBtnAvailabilty()
    {
        remoteUpgradeManager.GetAvailableUpgrades();
        buildPanel.GetAvailableUpgrades();
    }
}