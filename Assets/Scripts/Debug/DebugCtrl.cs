using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

public class DebugCtrl : MonoBehaviour
{
    public MoneyUI money;
    public GameObject newPigiAnim;
    [SerializeField] Skybox2D_Manager skybox;
    [SerializeField] Coin2DAnimationManager coin2DAnimationManager;

    public void Debug_ToggleDebugPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Debug_money0()
    {
        money.balcance.Clear();
        money.AddMoney(new Price(0, "a"));
    }

    public void Debug_money1b()
    {
        money.AddMoney(new Price(1, "b"));
    }

    public void Debug_ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Application.Quit();
    }

    public void Debug_newPigiAnim()
    {
        GameObject anim = Instantiate(newPigiAnim, gameObject.transform.parent.transform);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.newPigiFound);
        anim.GetComponent<NewPigiAnim>().StartAnim();
        anim.SetActive(true);
    }

    public void RemoveTimers()
    {
        foreach(GameObject obj in LocationManger.Instance.allocatedObj)
        {
            obj.GetComponent<Landmark>().buildCompleteTime = System.DateTime.Now;
            //obj.GetComponent<LocationObjData>().buildCompleteTime = System.DateTime.Now;
        }
    }

    public void ChangeBG()
    {
        skybox.DebugChangeMood();
    }

    public void Debug_AddOil(int amount)
    {
        coin2DAnimationManager.AddCoin(CoinType.Oil, Vector3.zero, 250f, 5);
        money.AddGemOil(CoinType.Oil, amount);
    }
    
    public void Debug_AddGem(int amount)
    {
        coin2DAnimationManager.AddCoin(CoinType.Gem, Vector3.zero, 250f, 5);
        money.AddGemOil(CoinType.Gem, amount);
    }

    public void Debug_LocaleTest()
    {
        string input = "[abc] hello world!";
        print(Localize.GetLocalizedString(input));
    }
}
