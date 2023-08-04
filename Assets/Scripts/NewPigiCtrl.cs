using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class NewPigiCtrl : MonoBehaviour
{
    public AudioCtrl myAudio;

    public Sprite[] imgs = new Sprite[1];
    public List<string> ids = new List<string>();

    public GameObject newPigiAnim;

    private void Start()
    {
        newPigiAnim.SetActive(false);
    }

    public void GotPigi(string id)
    {
        if (!PlayerPrefs.HasKey(id + "_count")) NewPigi(id);

        PlayerPrefs.SetInt(id + "_count", PlayerPrefs.GetInt(id + "_count") + 1);
        print("피지획득 | " + id + " | " + PlayerPrefs.GetInt(id + "_count"));
    }

    public void NewPigi(string id)
    {
        if(!ids.Contains(id))
        {
            print("[NewPigiCtrl - NewPigi] ID not found. id = " + id);
            return;
        }

        GameObject anim = Instantiate(newPigiAnim, gameObject.transform);

        anim.GetComponent<NewPigiAnim>().img.GetComponent<Image>().sprite = imgs[ids.IndexOf(id)];
        anim.GetComponent<NewPigiAnim>().pigi_title.text = GetLocalizedString("pigi", "title_" + id);
        anim.GetComponent<NewPigiAnim>().StartAnim();
        anim.SetActive(true);

        myAudio.PlaySFX(4);
    }

    private static string GetLocalizedString(string table, string name)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, name);
    }
}
