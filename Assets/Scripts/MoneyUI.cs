using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Price
{
    public int amount;
    public string charCode;
    public int idx;
    
    public Price(int amt = 0, string idxCode = "a")
    {
        if(amount < 10000)
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
        return amount.ToString() + charCode;
    }

    public int ConvertCodeToInt(string code)
    {
        int index = 0;
        // for(int i = code.Length-1; i>=0; i--)
        // {
        //     char charIdx = code[i];
        //     if (i == code.Length - 1) index += (charIdx - 'a');
        //     else
        //     {
        //         int digit = code.Length - 1 - i;
        //         index += (charIdx - 'a' + 1) * Mathf.RoundToInt(Mathf.Pow(26, digit));
        //     }
        // }
        index += (code[0] - 'a');
        return index;
    }

    public string ConvertIntToCode(int idx)
    {
        string code = "";

        if (idx >= 26)
        {
            int idxCode = (int)System.Math.Truncate(idx / 26f) + 96;
            char myChar = System.Convert.ToChar(idxCode);
            code += myChar;

            idx = idx % 26;
        }

        if (idx >= 0)
        {
            char myChar;
            myChar = System.Convert.ToChar(idx + 97);

            code += myChar;
        }

        return code;
    }
}

public class MoneyUI : MonoBehaviour
{
    public List<Price> balcance = new List<Price>();

    public TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        LoadData();   
    }

    // public void Update()
    // {
    //     if(Time.frameCount % 30 == 0)
    //     {
            
    //     }
    // }

    void UpdateUI(){
        for (int i = balcance.Count - 1; i >= 0; i--)
            {
                if (balcance[i].amount > 0)
                {
                    moneyText.text = i > 0 & balcance[i].amount < 10 ? balcance[i].amount + "." + Mathf.Floor(balcance[i - 1].amount / 1000f) + balcance[i].charCode : balcance[i].amount + balcance[i].charCode;
                    break;
                } else
                {
                    if (i == 0) moneyText.text = GetMyBalance().GetString();
                }
            }
    }

    string ConvertIntToCode(int idx)
    {
        string code = "";

        if(idx >= 26)
        {
            int idxCode = (int)System.Math.Truncate(idx / 26f) + 96;
            char myChar = System.Convert.ToChar(idxCode);
            code += myChar;

            idx = idx % 26;
        }

        if(idx >= 0)
        {
            char myChar;
            myChar = System.Convert.ToChar(idx + 97);

            code += myChar;
        }

        return code;
    }

    public void AddMoney(Price myPrice)
    {
        int money_idx = myPrice.idx;

        if(balcance.Count <= money_idx)
        {
            int addAmount = money_idx - balcance.Count;
            for (int i = 0; i<= addAmount; i++)
                balcance.Add(new Price(0, ConvertIntToCode(balcance.Count)));
        }

        balcance[money_idx].amount += myPrice.amount;
        CheckIfExceed(money_idx);
        UpdateUI();
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
        return true;
    }

    private void CheckIfShort(int idx)
    {
        if (balcance[idx].amount < 0)
        {
            balcance[idx].amount += 10000;
            balcance[idx+1].amount -= 1;
            CheckIfShort(idx + 1);
        }
    }

    private Price GetMyBalance()
    {
        for (int i = balcance.Count - 1; i >= 0; i--)
        {
            if (balcance[i].amount > 0)
            {
                return new Price(balcance[i].amount, balcance[i].charCode);
            }
        }
        return new Price(0, "a");
    }

    public bool HasEnoughMoney(Price price)
    {
        Price balance = GetMyBalance();
        if (balance.idx > price.idx) return true;
        else if(balance.idx == price.idx)
        {
            if (balance.amount >= price.amount) return true;
            else return false;
        }
        return false;
    }

    private void LoadData()
    {
        // if(PlayerPrefs.HasKey("myBalanceAmount"))
        // {
        //     Price myBalance = new Price(PlayerPrefs.GetInt("myBalanceAmount"), PlayerPrefs.GetString("myBalanceChar"));
        //     AddMoney(myBalance);
        //     print("data loaded - balance : " + myBalance.GetString());
        // } else 
        AddMoney(new Price(5000, "a"));
    }

    private void SaveData()
    {
        Price myBalance = GetMyBalance();
        PlayerPrefs.SetInt("myBalanceAmount", myBalance.amount);
        PlayerPrefs.SetString("myBalanceChar", myBalance.charCode);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        //if (pause) SaveData();
    }

    private void OnApplicationQuit()
    {
        //SaveData();
    }
}