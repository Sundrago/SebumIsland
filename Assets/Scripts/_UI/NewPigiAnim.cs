using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NewPigiAnim : MonoBehaviour
{
    public GameObject bg, imageBox;
    public Image img;
    public TextMeshProUGUI title, pigi_title;

    // Start is called before the first frame update

    public void StartAnim()
    {
        imageBox.transform.DOLocalMoveX(50, 3f)
            .SetRelative();

        gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f)
            .SetEase(Ease.OutExpo)
            .From();
        img.transform.DOLocalMoveX(-300, 0.5f)
            .SetEase(Ease.OutExpo)
            .From();
        title.transform.DOLocalMoveY(-300, 0.5f)
            .SetEase(Ease.OutExpo)
            .From();

        title.transform.DOLocalMoveY(10, 1.3f)
            .SetRelative()
            .SetEase(Ease.InOutSine)
            .SetDelay(0.5f);

        img.transform.DOLocalMoveX(Screen.width, 0.5f)
            .SetEase(Ease.InExpo)
            .SetDelay(1.8f);
        title.transform.DOLocalMoveY(50, 0.5f)
            .SetRelative()
            .SetEase(Ease.InExpo)
            .SetDelay(1.8f)
            .OnComplete(DestroySelf);
        gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 0), 0.5f)
            .SetEase(Ease.InExpo)
            .SetDelay(1.8f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
