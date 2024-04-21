using UnityEngine;

/// <summary>
///     The GameManager class is responsible for managing the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string PREFS_IS_NEW_USER = "IsNewUser";

    private void Start()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.PlayBGM(SFX_tag.BGMMain);

        if (!PlayerPrefs.HasKey(PREFS_IS_NEW_USER))
            NewToGame();
    }

    private void NewToGame()
    {
        PlayerPrefs.DeleteAll();
        MoneyManager.Instance.AddMoney(new Price(5));
        PlayerPrefs.SetInt(PREFS_IS_NEW_USER, -1);
    }
}