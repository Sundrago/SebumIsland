using DG.Tweening;
using UnityEngine;

/// <summary>
///     A base class for panels in the game.
/// </summary>
public abstract class PanelObject : MonoBehaviour
{
    private Vector3 closePanelPosition = new(2500, 500, 0);
    private Vector3 openPanelPosition = new(-2500, -500, 0);
    private Vector3 panelRotation = new(0, 0, 10);
    private Transform panelTransform;

    /// <summary>
    ///     Opens the panel by animating its position, rotation, and visibility.
    /// </summary>
    public virtual void OpenPanel()
    {
        if (gameObject.activeSelf) return;

        AnimatePanelOpen();
        OnPanelOpen();
    }

    private void AnimatePanelOpen()
    {
        PanelManager.Instance.CloseOtherPanels(gameObject);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(-2500, -500, 0), 0.5f)
            .SetEase(Ease.OutExpo)
            .From();
        gameObject.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.5f)
            .SetEase(Ease.OutBack)
            .From();
        gameObject.SetActive(true);
    }

    protected virtual void OnPanelOpen()
    {
        PanelManager.Instance.CloseOtherPanels(gameObject);
    }

    /// <summary>
    ///     Closes the panel by animating its position, rotation, and visibility.
    /// </summary>
    public virtual void ClosePanel()
    {
        if (!gameObject.activeSelf) return;

        AnimatePanelClose();
    }

    private void AnimatePanelClose()
    {
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.transform.DOLocalMove(new Vector3(2500, 500, 0), 0.5f)
            .SetEase(Ease.InQuint);
        gameObject.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}