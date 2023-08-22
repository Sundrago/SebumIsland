using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HarvestAllCtrl : MonoBehaviour
{
    private List<Landmark> readyLandmarks = new List<Landmark>();
    [SerializeField] Button EasyHarvestBtn;
    [SerializeField] Image[] EasyHarvestImg;
    [SerializeField] InfoDataManager infoDataManager;

    public static HarvestAllCtrl Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateEasyHarvest()
    {
        for(int i = 0; i<EasyHarvestImg.Length; i++)
        {
            if (i >= readyLandmarks.Count) EasyHarvestImg[i].sprite = null;
            else  EasyHarvestImg[i].sprite = infoDataManager.GetLandmarkItemByID(readyLandmarks[i].locationObject.modelID).Img;
        }
    }

    public void EasyHarvestClicked()
    {
        if (readyLandmarks.Count == 0) return;
        if(readyLandmarks[0] != null) readyLandmarks[0].HarvestAll();
        readyLandmarks.RemoveAt(0);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.yeah);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.smallPopPop);

        UpdateEasyHarvest();

        //tutorial 04
        QuestTutorialManager.Instance.AddHarvestAllClickCount();
    }

    public void AddReadyLandmark(GameObject obj) {
        readyLandmarks.Add(obj.GetComponent<Landmark>());
        UpdateEasyHarvest();
        //UpdateBtnInteractable(true);

        if (DOTween.IsTweening(EasyHarvestBtn.gameObject.transform)) DOTween.Kill(EasyHarvestBtn.gameObject.transform);
        EasyHarvestBtn.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        EasyHarvestBtn.gameObject.transform.DOShakeScale(0.15f);

    }

    public void HarvestAllBtnClicked() {
        foreach(Landmark landmark in readyLandmarks) {
            if(landmark != null) landmark.HarvestAll();
        }

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.yeah);
        UpdateBtnInteractable(false);
        readyLandmarks = new List<Landmark>();
    }

    private void UpdateBtnInteractable(bool isActive) {
        if(isActive) {
            gameObject.GetComponent<Button>().interactable = true;
        } else {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void RemoveReadyLandmark(GameObject obj) {
        if(readyLandmarks.Contains(obj.GetComponent<Landmark>())) {
            readyLandmarks.Remove(obj.GetComponent<Landmark>());
            if(readyLandmarks.Count == 0) UpdateBtnInteractable(false);
        }

        UpdateEasyHarvest();
    }

    void Start()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }
}

