using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BalloonMsg : MonoBehaviour
{
    public GameObject balloon_ui;
    public TextMeshProUGUI text_ui;
    public Image bg_ui;
    public bool shown = false;

    public void Show(string msg)
    {
        text_ui.text = msg;

        balloon_ui.transform.DOMoveY(-500f, 0.4f)
            .SetEase(Ease.OutQuint)
            .From()
            .OnComplete(Wait);
        bg_ui.DOFade(0, 0.3f)
            .SetEase(Ease.OutQuint)
            .From();
        text_ui.DOFade(0, 0.3f)
            .SetEase(Ease.OutQuint)
            .From();
        gameObject.SetActive(true);
    }

    public void Wait()
    {
        bg_ui.DOFade(0.5f, 1.5f)
            .OnComplete(Hide);
        text_ui.DOFade(0.8f, 1.5f);
        shown = true;
    }

    public void Hide()
    {
        if (!shown) return;

        balloon_ui.transform.DOMoveY(Screen.height, 0.5f)
            .SetEase(Ease.OutQuint)
            .OnComplete(DestroySelf);
        bg_ui.DOFade(0, 0.5f)
            .SetEase(Ease.OutQuint);
        text_ui.DOFade(0, 0.5f)
            .SetEase(Ease.OutQuint);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
