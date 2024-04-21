using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Responsible for displaying a balloon message UI element with fade-in and fade-out animations.
/// </summary>
public class BalloonMsg : MonoBehaviour
{
    [FormerlySerializedAs("balloon_ui")] [SerializeField]
    private GameObject balloonUI;

    [FormerlySerializedAs("text_ui")] [SerializeField]
    private TextMeshProUGUI textUI;

    [FormerlySerializedAs("bg_ui")] [SerializeField]
    private Image bgUI;

    private bool shown;

    public void Show(string msg)
    {
        textUI.text = msg;

        balloonUI.transform.DOMoveY(-500f, 0.4f)
            .SetEase(Ease.OutQuint)
            .From()
            .OnComplete(Wait);
        bgUI.DOFade(0, 0.3f)
            .SetEase(Ease.OutQuint)
            .From();
        textUI.DOFade(0, 0.3f)
            .SetEase(Ease.OutQuint)
            .From();
        gameObject.SetActive(true);
    }

    public void Wait()
    {
        bgUI.DOFade(0.5f, 1.5f)
            .OnComplete(Hide);
        textUI.DOFade(0.8f, 1.5f);
        shown = true;
    }

    public void Hide()
    {
        if (!shown) return;

        balloonUI.transform.DOMoveY(Screen.height, 0.5f)
            .SetEase(Ease.OutQuint)
            .OnComplete(DestroySelf);
        bgUI.DOFade(0, 0.5f)
            .SetEase(Ease.OutQuint);
        textUI.DOFade(0, 0.5f)
            .SetEase(Ease.OutQuint);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}