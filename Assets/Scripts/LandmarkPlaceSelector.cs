using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkPlaceSelector : MonoBehaviour
{
    public GameObject positionMark;
    public GameObject[] places;
    public GameObject finsihBtn;

    private GameObject[] landmarks;
    private int selectedIdx = -1;
    private bool started = false;


    //private void Start()
    //{
    //    if (started) return;
    //    started = true;
    //    landmarks = new GameObject[places.Length];
    //    finsihBtn.SetActive(false);
    //}

    //public void PlaneSelected(int i)
    //{
    //    if (i == selectedIdx) return;

    //    if(landmarks[i] == null)
    //    {
    //        ChangePosition(selectedIdx, i);
    //    } else
    //    {
    //        ChangeTarget(i);
    //    }
    //}

    //public void OpenPanel(int idx)
    //{
    //    for(int i = 0; i<places.Length; i++)
    //    {
    //        if (landmarks[i] == null) places[i].SetActive(true);
    //        else places[i].SetActive(false);
    //    }
    //    ChangeTarget(idx);
    //    positionMark.SetActive(true);
    //    gameObject.SetActive(true);
    //    Camera.main.GetComponent<PinchZoom>().onMoveLandmark = true;
    //    finsihBtn.SetActive(true);

    //    Camera.main.GetComponent<PinchZoom>().StartCamTransition(Camera.main.transform.position, 50);
    //}

    //private void ChangePosition(int from, int to)
    //{
    //    landmarks[from].transform.position = places[to].transform.position;
    //    landmarks[from].GetComponent<Landmark>().placeID = to;
    //    landmarks[to] = landmarks[from];
    //    landmarks[from] = null;

    //    OpenPanel(to);
    //}

    //private void ChangeTarget(int idx)
    //{
    //    positionMark.transform.position = places[idx].transform.position;
    //    selectedIdx = idx;
    //}

    //public void ClosePanel()
    //{
    //    Camera.main.GetComponent<PinchZoom>().onMoveLandmark = false;
    //    positionMark.SetActive(false);
    //    gameObject.SetActive(false);
    //    finsihBtn.SetActive(false);
    //}

    //public void SetLandmark(GameObject obj, int idx)
    //{
    //    Start();
    //    landmarks[idx] = obj;
    //    obj.transform.position = places[idx].transform.position;
    //}
}
