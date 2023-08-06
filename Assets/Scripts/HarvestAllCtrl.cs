using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HarvestAllCtrl : MonoBehaviour
{
    [SerializeField] AudioCtrl audioCtrl;

    private List<Landmark> readyLandmarks = new List<Landmark>();

    public void AddReadyLandmark(GameObject obj) {
        readyLandmarks.Add(obj.GetComponent<Landmark>());
        UpdateBtnInteractable(true);

        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.transform.DOShakeScale(0.3f);
    }

    public void HarvestAllBtnClicked() {
        foreach(Landmark landmark in readyLandmarks) {
            if(landmark != null) landmark.HarvestAll();
        }
        
        audioCtrl.PlaySFX(6);
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
    }

    void Start()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }
}

