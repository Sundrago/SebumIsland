using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectionPanelCtrl : MonoBehaviour
{
    [SerializeField] Collection_Pigi collection_pigi;
    public void Show() {
        if(!collection_pigi.initialized) collection_pigi.InitializePigiCollection();
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
}
