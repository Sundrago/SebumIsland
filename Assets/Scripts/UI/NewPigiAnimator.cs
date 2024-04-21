using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewPigiAnimator : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI pigi_title;
    [SerializeField] private Transform collectionIcon;

    public void SetupUI(Sprite sprite, string title)
    {
        img.sprite = sprite;
        pigi_title.text = title;
    }

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
    }

    public void Hide()
    {
        if (DOTween.IsTweening(gameObject.transform)) return;

        gameObject.transform.DOMove(collectionIcon.transform.position, 0.75f)
            .SetEase(Ease.InOutQuart);
        gameObject.transform.DOScale(Vector3.zero, 0.75f)
            .SetEase(Ease.InOutQuart);
        gameObject.transform.DORotate(new Vector3(0, 0, -60), 0.5f)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
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