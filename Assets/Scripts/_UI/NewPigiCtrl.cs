using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class NewPigiCtrl : MonoBehaviour
{
    public NewPigiAnim newPigiAnim;
    public static NewPigiCtrl Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GotPigi(string id)
    {
        if (!PlayerPrefs.HasKey(id + "_count")) NewPigi(id);

        PlayerPrefs.SetInt(id + "_count", PlayerPrefs.GetInt(id + "_count") + 1);
        //print("피지획득 | " + id + " | " + PlayerPrefs.GetInt(id + "_count"));

        if(id == "pigi_farm0")
        {
            QuestTutorialManager.Instance.AddBasicPigiCount();
        }
    }

    public void NewPigi(string id)
    {
        PigiItem pigiItem = InfoDataManager.Instance.GetPigiItemByID(id);

        if(pigiItem == null)
        {
            print("[NewPigiCtrl - NewPigi] ID not found. id = " + id);
            return;
        }

        NewPigiAnim anim = Instantiate(newPigiAnim, gameObject.transform);
        anim.img.sprite = pigiItem.Img;
        anim.pigi_title.text = GetLocalizedString("Pigi","title_" + id);
        anim.StartAnim();
        gameObject.SetActive(true);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.newPigiFound);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }
}
