using System;
using UnityEngine;

/// <summary>
///     DebugManager class represents a MonoBehaviour that provides debug functionalities for the game.
/// </summary>
public class DebugManager : MonoBehaviour
{
    [SerializeField] private MoneyManager money;
    [SerializeField] private GameObject newPigiAnim;
    [SerializeField] private Skybox2DManager skybox;
    [SerializeField] private Coin2DAnimationManager coin2DAnimationManager;

    public void Debug_ToggleDebugPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Debug_money0()
    {
        money.balcance.Clear();
        money.AddMoney(new Price());
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
        var anim = Instantiate(newPigiAnim, gameObject.transform.parent.transform);
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.NewPigiFound);
        anim.GetComponent<NewPigiAnimator>().StartAnim();
        anim.SetActive(true);
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

    public void RemoveTimers()
    {
        foreach (var obj in LocationManger.Instance.AllocatedObj) obj.buildCompleteTime = DateTime.Now;
    }

    public void ChangeBG()
    {
        skybox.DebugChangeMood();
    }
}