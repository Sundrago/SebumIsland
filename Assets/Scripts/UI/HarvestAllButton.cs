using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Represents a button that initiates the harvest of all ready landmarks.
/// </summary>
public class HarvestAllButton : MonoBehaviour
{
    [SerializeField] private Button EasyHarvestBtn;
    [SerializeField] private Image[] EasyHarvestImg;
    [SerializeField] private InfoDataManager infoDataManager;
    private List<LandmarkController> readyLandmarks = new();
    public static HarvestAllButton Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }

    private void UpdateEasyHarvest()
    {
        for (var i = 0; i < EasyHarvestImg.Length; i++)
            if (i >= readyLandmarks.Count) EasyHarvestImg[i].sprite = null;
            else
                EasyHarvestImg[i].sprite =
                    infoDataManager.GetLandmarkItemByID(readyLandmarks[i].ScriptableObjet.ModelID).Img;
    }

    public void EasyHarvestClicked()
    {
        if (readyLandmarks.Count == 0) return;
        if (readyLandmarks[0] != null) readyLandmarks[0].HarvestAll();
        readyLandmarks.RemoveAt(0);
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.Yeah);
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.SmallPopPop);

        UpdateEasyHarvest();

        //tutorial 04
        QuestTutorialManager.Instance.AddHarvestAllClickCount();
    }

    public void AddReadyLandmark(GameObject obj)
    {
        readyLandmarks.Add(obj.GetComponent<LandmarkController>());
        UpdateEasyHarvest();

        if (DOTween.IsTweening(EasyHarvestBtn.gameObject.transform)) DOTween.Kill(EasyHarvestBtn.gameObject.transform);
        EasyHarvestBtn.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        EasyHarvestBtn.gameObject.transform.DOShakeScale(0.15f);
    }

    public void HarvestAllBtnClicked()
    {
        foreach (var landmark in readyLandmarks)
            if (landmark != null)
                landmark.HarvestAll();

        AudioManager.Instance.PlaySFXbyTag(SFX_tag.Yeah);
        UpdateBtnInteractable(false);
        readyLandmarks = new List<LandmarkController>();
    }

    private void UpdateBtnInteractable(bool isActive)
    {
        if (isActive)
            gameObject.GetComponent<Button>().interactable = true;
        else
            gameObject.GetComponent<Button>().interactable = false;
    }

    public void RemoveReadyLandmark(GameObject obj)
    {
        if (readyLandmarks.Contains(obj.GetComponent<LandmarkController>()))
        {
            readyLandmarks.Remove(obj.GetComponent<LandmarkController>());
            if (readyLandmarks.Count == 0) UpdateBtnInteractable(false);
        }

        UpdateEasyHarvest();
    }
}