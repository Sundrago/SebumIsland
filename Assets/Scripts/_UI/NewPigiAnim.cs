using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class NewPigiAnim : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI pigi_title;
    [SerializeField]
    private Transform collectionIcon;

    
    public void StartAnim()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.eulerAngles = Vector3.zero;
        
        gameObject.transform.DOMove(new Vector3(0, -1500, 0), 0.5f)
            .From()
            .SetEase(Ease.OutExpo);
        gameObject.transform.DOLocalRotate(new Vector3(0, 0, 20), 0.5f)
            .From()
            .SetEase(Ease.OutBack);
        gameObject.SetActive(true);
        

        // imageBox.transform.DOLocalMoveX(50, 3f)
        //     .SetRelative();
        //
        // gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f)
        //     .SetEase(Ease.OutExpo)
        //     .From();
        // img.transform.DOLocalMoveX(-300, 0.5f)
        //     .SetEase(Ease.OutExpo)
        //     .From();
        // title.transform.DOLocalMoveY(-300, 0.5f)
        //     .SetEase(Ease.OutExpo)
        //     .From();
        //
        // title.transform.DOLocalMoveY(10, 1.3f)
        //     .SetRelative()
        //     .SetEase(Ease.InOutSine)
        //     .SetDelay(0.5f);
        //
        // img.transform.DOLocalMoveX(Screen.width, 0.5f)
        //     .SetEase(Ease.InExpo)
        //     .SetDelay(1.8f);
        // title.transform.DOLocalMoveY(50, 0.5f)
        //     .SetRelative()
        //     .SetEase(Ease.InExpo)
        //     .SetDelay(1.8f)
        //     .OnComplete(DestroySelf);
        // gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f)
        //     .SetEase(Ease.InExpo)
        //     .SetDelay(1.8f);
    }
    public void Hide()
    {
        if(DOTween.IsTweening((gameObject.transform))) return;

        gameObject.transform.DOMove(collectionIcon.transform.position, 0.75f)
            .SetEase(Ease.InOutQuart);
        gameObject.transform.DOScale(Vector3.zero, 0.75f)
            .SetEase(Ease.InOutQuart);
        gameObject.transform.DORotate(new Vector3(0, 0, -60), 0.5f)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() => {
                collectionIcon.localScale = Vector3.one;
                collectionIcon.DOPunchScale(Vector3.one * 0.15f, 0.3f)
                    .OnComplete(() => Destroy(gameObject));
            });
    }
    
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    
}
