using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//public class CameraTransition
//{
//    public bool onTransition = false;
//    Vector3 startPosition;
//    Vector3 endPosition;
//    float startSize;
//    float endSize;
//    float startTime, endTime;



//    public void Setup(Vector3 start_pos, Vector3 end_pos, float start_size, float end_size, float duration)
//    {
//        onTransition = true;
//        startPosition = start_pos;
//        endPosition = end_pos;
//        startSize = start_size;
//        endSize = end_size;
//        startTime = Time.time;
//        endTime = startTime + duration;
//    }

//    public void Update(Camera cam)
//    {
//        if (!onTransition) return;
//        if(Time.time >= endTime)
//        {
//            onTransition = false;
//            return;
//        }

//        float normal = Mathf.Abs((Time.time - startTime) / (endTime - startTime));
//        normal = EaseOutCirc(normal);

//        cam.transform.position = Vector3.Lerp(startPosition, endPosition, normal);
//        cam.orthographicSize = Mathf.Lerp(startSize, endSize, normal);
//    }

//    float EaseOutCirc(float x){
//        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
//    }
//}

public class PinchZoom : MonoBehaviour
{
    const float orthoZoomSpeed = 0.05f;
    const float camMoveSpeed = 0.001f;

    [SerializeField] GameObject settings, collection, build;

    //public CameraTransition camTrans = new CameraTransition();
    public NewUpgPanel upgradePanel;
    public PigiInfoPanel pigiInfoPanel;
    public bool onMoveLandmark = false;
    public Slider zoomSlider;

    Vector3 targetPosition;
    float targetSize;

    int myTouchCount = 0;
    Vector2 initialPoint;
    Touch touchZero, touchOne;

    public LineRenderer line;
    private GameObject targetPigi;
    float initialY;
    bool distCheck;
    public bool buildMode = false;

    const float touchTolerance = 75f;

    private void Start()
    {
        //Application.targetFrameRate = 60;
        targetPosition = Camera.main.transform.position;
        targetSize = Camera.main.orthographicSize;
        SetSliderValue();
    }

    void Update()
    {
        if (DOTween.IsTweening(Camera.main) | DOTween.IsTweening(Camera.main.transform)) return;
        if (settings.activeSelf || collection.activeSelf || build.activeSelf) return;

        if (Vector3.Distance(targetPosition, Camera.main.transform.position) > 0.1f)
        {
            //print(Camera.main.transform.position);
            Vector3 tempPos = Camera.main.transform.position;
            tempPos += (targetPosition - tempPos) * 0.25f;
            Camera.main.transform.position = tempPos;
        }
        else Camera.main.transform.position = targetPosition;

        if (Mathf.Abs(Camera.main.orthographicSize - targetSize) > 0.01f)
        {
            float tempSize = Camera.main.orthographicSize;
            tempSize += (targetSize - tempSize) * 0.25f;
            Camera.main.orthographicSize = tempSize;
        }
        else Camera.main.orthographicSize = targetSize;

        if (buildMode) return;

        if (Input.touchCount == 0)
        {
            if(myTouchCount == 0)   return;

            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);

            if (myTouchCount == -1)
            {
                if (targetPigi != null)
                {
                    targetPigi.GetComponent<PigiCtrl>().Event_Up();
                    targetPigi = null;
                }
            }

            if((myTouchCount == 1 || myTouchCount == -1) && Vector2.Distance(initialPoint, touchZero.position) < 50f && !onMoveLandmark)
            {

                Ray ray = Camera.main.ScreenPointToRay(touchZero.position);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                //return if UI element is clicked
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "UI") return;
                }

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "pigi")
                    {
                        //Open Pigi info Panel
                        pigiInfoPanel.OpenPanel(hit.collider.gameObject);
                        StartCamTransition(hit.collider.gameObject.transform.position, 6f);
                        myTouchCount = 0;
                        return;
                    }
                }
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "landmark")
                    {
                        if (!hit.collider.gameObject.GetComponent<Landmark>().enabled) return;
                        upgradePanel.OpenPanel(hit.collider.gameObject);
                        StartCamTransition(hit.collider.gameObject.transform.position, 24f);
                        myTouchCount = 0;
                        return;
                    }
                }
            }
            myTouchCount = 0;
        }

        if (Input.touchCount == 1)
        {
            if (myTouchCount == 2)
            {
                return;
            }
            

            touchZero = Input.GetTouch(0);

            //// Touch Event Handler
            //var objects = GameObject.FindGameObjectsWithTag("pigi");
            //float[] dists = new float[objects.Length];
            //float minDist = float.MaxValue;
            //int minIdx = -1;

            //for(int i = 0; i<objects.Length; i++)
            //{
            //    GameObject obj = objects[i];
            //    Debug.DrawLine(Camera.main.WorldToScreenPoint(obj.transform.position), touchZero.position, Color.red, 0.1f);
            //    dists[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(obj.transform.position), touchZero.position);
            //    if(dists[i] < minDist)
            //    {
            //        minDist = dists[i];
            //        minIdx = i;
            //    }
            //}

            line.SetPosition(0, Camera.main.ScreenToWorldPoint(touchZero.position));
            //objects[minIdx].transform.position
            if (targetPigi != null)
            {
                //line.SetPosition(0, Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, targetPigi.transform.position.z)));
                //line.SetPosition(1, targetPigi.transform.position);
                float newY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, targetPigi.transform.position.z)).y;

                Vector3 objPos = targetPigi.transform.position;
                objPos.y = 0.5f;

                line.SetPosition(0, Camera.main.ScreenToWorldPoint(touchZero.position));
                line.SetPosition(1, objPos);

                targetPigi.GetComponent<PigiCtrl>().Event_Drag(newY - initialY);

                if(!distCheck & Vector2.Distance(initialPoint, touchZero.position) > 100f)
                {
                    distCheck = true;
                    if (touchZero.position.y - initialPoint.y < 0 | Mathf.Abs(touchZero.position.y - initialPoint.y) * 2 < Mathf.Abs(touchZero.position.x - initialPoint.x))
                    {
                        myTouchCount = 1;
                        targetPigi.GetComponent<PigiCtrl>().Event_Up();
                        targetPigi = null;
                        initialPoint = touchZero.position;
                    }
                }
            }

            Debug.DrawLine(touchZero.position - new Vector2(touchTolerance, touchTolerance), touchZero.position + new Vector2(touchTolerance, touchTolerance), Color.yellow, 0.1f);
            Debug.DrawLine(touchZero.position - new Vector2(touchTolerance, - touchTolerance), touchZero.position + new Vector2(touchTolerance, - touchTolerance), Color.yellow, 0.1f);
            //

            if (myTouchCount == 0)
            {
                initialPoint = touchZero.position;
                targetPigi = null;
                myTouchCount = 1;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.tag == "pigi")
                    {
                        myTouchCount = -1;
                        if (hit.collider.gameObject.GetComponent<PigiCtrl>().grown)
                        {
                            distCheck = false;
                            targetPigi = hit.collider.gameObject;
                            initialY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, targetPigi.transform.position.z)).y;
                        }
                    }
                }

                // Touch Event Handler
                var objects = GameObject.FindGameObjectsWithTag("pigi");
                float dist, minDist;
                int minIdx = -1;

                minDist = Mathf.Lerp(150f, 75f, ((Camera.main.orthographicSize-4.9f) / 55f));

                for (int i = 0; i < objects.Length; i++)
                {
                    GameObject obj = objects[i];
                    Debug.DrawLine(Camera.main.WorldToScreenPoint(obj.transform.position), touchZero.position, Color.red, 0.1f);

                    if (!objects[i].GetComponent<PigiCtrl>().grown) continue;
                    Vector3 objPos = obj.transform.position;
                    objPos.y = 0.5f;
                    dist = Vector2.Distance(Camera.main.WorldToScreenPoint(objPos), touchZero.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minIdx = i;
                    }
                }

                if (minIdx != -1)
                {
                    distCheck = false;
                    targetPigi = objects[minIdx];
                    myTouchCount = -1;
                    targetPigi.GetComponent<PigiCtrl>().Event_Down();
                    initialY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, targetPigi.transform.position.z)).y;
                }
            }

            //Dont move cam if Pigi is first clicked
            if (myTouchCount == -1) return;

            line.SetPosition(0, Camera.main.ScreenToWorldPoint(touchZero.position));
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(initialPoint));

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;

            float diffX = (touchZeroPrevPos.x - touchZero.position.x);
            float diffY = (touchZeroPrevPos.y - touchZero.position.y);

            Vector3 newPos = targetPosition;
            newPos.x += diffX * camMoveSpeed * Camera.main.orthographicSize;
            newPos.x -= diffY * camMoveSpeed * Camera.main.orthographicSize;

            newPos.z += diffX * camMoveSpeed * Camera.main.orthographicSize;
            newPos.z += diffY * camMoveSpeed * Camera.main.orthographicSize;

            newPos.x = Mathf.Min(Mathf.Max(newPos.x, -20f), 20f);
            newPos.z = Mathf.Min(Mathf.Max(newPos.z, -70f), 50f);

            targetPosition = newPos;


        }

        if (Input.touchCount == 2)
        {
            if(myTouchCount != 2) myTouchCount = 2;

            targetPigi = null;
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            targetSize += deltaMagnitudeDiff * orthoZoomSpeed;
            targetSize = Mathf.Min(Mathf.Max(targetSize, 5f), 60f);
            SetSliderValue();
        }
    }

    public void StartCamTransition(Vector3 target, float zoomSize)
    {
        targetPosition = target;
        targetSize = zoomSize;

        if (DOTween.IsTweening(Camera.main.transform)) DOTween.Kill(Camera.main.transform);
        if (DOTween.IsTweening(Camera.main)) DOTween.Kill(Camera.main);

        Camera.main.transform.DOMove(target, 0.75f);
        Camera.main.DOOrthoSize(zoomSize, 0.75f);

        SetSliderValue();
    }

    public void SldierValueChanged()
    {
        if (zoomSlider.value * 60f + 5f == targetSize) return;

        targetSize = zoomSlider.value * 60f + 5f;
    }

    public void SetSliderValue()
    {
        zoomSlider.value = (targetSize - 5f) / 60f;
    }
}