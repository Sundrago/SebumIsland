using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

/// <summary>
///     Animation controller for new pigi objects.
/// </summary>
public class NewPigiAnimationController : MonoBehaviour
{
    public static NewPigiAnimationController Instance;
    [FormerlySerializedAs("newPigiAnim")] public NewPigiAnimator newPigiAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void GotPigi(string id)
    {
        if (!PlayerPrefs.HasKey(id + "_count")) NewPigi(id);

        PlayerPrefs.SetInt(id + "_count", PlayerPrefs.GetInt(id + "_count") + 1);
        //print("피지획득 | " + id + " | " + PlayerPrefs.GetInt(id + "_count"));

        if (id == "pigi_farm0") QuestTutorialManager.Instance.AddBasicPigiCount();
    }

    public void NewPigi(string id)
    {
        var pigiItem = InfoDataManager.Instance.GetPigiItemByID(id);

        if (pigiItem == null)
        {
            print("[NewPigiAnimationController - NewPigi] ID not found. id = " + id);
            return;
        }

        var animator = Instantiate(newPigiAnimator, gameObject.transform);
        animator.SetupUI(pigiItem.Img, GetLocalizedString("Pigi", "title_" + id));
        animator.StartAnim();
        gameObject.SetActive(true);
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.NewPigiFound);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }
}