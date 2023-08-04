using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocationMarkTouchEvent : MonoBehaviour, IPointerClickHandler
{
    public int idx = -1;
    public string direction;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(idx == -1) gameObject.transform.parent.gameObject.GetComponent<LocationManger>().LocationMarkMoveClicked(direction);
        else gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<LocationManger>().LocationMarkClicked(idx);
    }
}
