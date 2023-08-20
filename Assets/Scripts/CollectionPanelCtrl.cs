using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CollectionPanelCtrl : MonoBehaviour
{
    [SerializeField] Collection_Pigi collection_pigi, collection_Landmark;
    [SerializeField] Button pigi_tab, landmark_tab;

    private void Start()
    {
        ShowPigi();
    }

    public void Show() {
        if(!collection_pigi.PigiInitialized) collection_pigi.InitializePigiCollection();
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;

        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(-2500,-500,0), 0.5f)
            .SetEase(Ease.OutExpo)
            .From();
        gameObject.transform.DOLocalRotate(new Vector3(0,0,10), 0.5f)
            .SetEase(Ease.OutBack)
            .From();

        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;

        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(2500,500,0), 0.5f)
            .SetEase(Ease.InQuint);
        gameObject.transform.DOLocalRotate(new Vector3(0,0,10), 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(()=>gameObject.SetActive(false));
    }

    public void ShowPigi()
    {
        if (!collection_pigi.PigiInitialized) collection_pigi.InitializePigiCollection();
        collection_pigi.gameObject.SetActive(true);
        collection_Landmark.gameObject.SetActive(false);

        pigi_tab.interactable = false;
        landmark_tab.interactable = true;
    }

    public void ShowLandmark()
    {
        if (!collection_pigi.LandmarkInitialized) collection_Landmark.InitializeLandmarkCollection();
        collection_pigi.gameObject.SetActive(false);
        collection_Landmark.gameObject.SetActive(true);

        pigi_tab.interactable = true;
        landmark_tab.interactable = false;
    }
}
