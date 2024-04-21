using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Handles the click event on a location mark.
/// </summary>
public class LocationMarkClickEventHandler : MonoBehaviour, IPointerClickHandler
{
    public int idx = -1;
    public string direction;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (idx == -1)
            gameObject.transform.parent.gameObject.GetComponent<LocationManger>()?.LocationMarkMoveClicked(direction);
        else
            gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<LocationManger>()
                ?.LocationMarkClicked(idx);
    }
}