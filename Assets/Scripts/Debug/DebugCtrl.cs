using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCtrl : MonoBehaviour
{
    public MoneyUI money;
    public GameObject newPigiAnim;
    [SerializeField] Skybox2D_Manager skybox;

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
        anim.GetComponent<NewPigiAnim>().StartAnim();
        anim.SetActive(true);
    }

    public void RemoveTimers()
    {
        foreach(GameObject obj in LocationManger.Instance.allocatedObj)
        {
            obj.GetComponent<Landmark>().buildCompleteTime = System.DateTime.Now;
        }
    }

    public void ChangeBG()
    {
        skybox.DebugChangeMood();
    }
}
