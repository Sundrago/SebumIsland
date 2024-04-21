using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Manages touch events and camera transitions in the game.
/// </summary>
public class TouchEventManager : MonoBehaviour
{
    private const float orthoZoomSpeed = 0.05f;
    private const float camMoveSpeed = 0.001f;

    [SerializeField] private GameObject settings, collection, build;

    public UpgradePanel upgradePanel;
    public bool onMoveLandmark;
    public Slider zoomSlider;
    private int deltaTouchCount;
    private bool distCheck;
    private Vector2 initialPoint;
    private PigiController targetPigi;

    private Vector3 targetPosition;
    private float targetSize, initialY;
    private Touch touchZero, touchOne;

    public bool IsInBuildMode { get; set; }

    private void Start()
    {
        targetPosition = Camera.main.transform.position;
        targetSize = Camera.main.orthographicSize;
        SetSliderValue();
    }

    private void Update()
    {
        if (DOTween.IsTweening(Camera.main) | DOTween.IsTweening(Camera.main.transform)) return;
        if (settings.activeSelf || collection.activeSelf || build.activeSelf) return;

        UpdateCameraTransform();

        if (IsInBuildMode) return;

        if (Input.touchCount == 0)
        {
            if (deltaTouchCount == 0) return;

            if (deltaTouchCount == -1)
                if (targetPigi != null)
                {
                    targetPigi.Event_Up();
                    targetPigi = null;
                }

            if ((deltaTouchCount == 1 || deltaTouchCount == -1) &&
                Vector2.Distance(initialPoint, touchZero.position) < 50f && !onMoveLandmark)
                if (HandleClick())
                    return;

            deltaTouchCount = 0;
        }

        if (Input.touchCount == 1)
        {
            if (deltaTouchCount == 2)
                return;

            HandleDrag();

            if (deltaTouchCount == 0)
                HandleMouseDown();
            if (deltaTouchCount == -1)
                return;

            targetPosition = GetTargetPositioin();
        }

        if (Input.touchCount == 2)
        {
            if (deltaTouchCount != 2) deltaTouchCount = 2;
            HandlePinchZoom();
        }
    }

    private bool HandleClick()
    {
        var ray = Camera.main.ScreenPointToRay(touchZero.position);
        var hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
            if (hit.collider.gameObject.tag == "UI")
                return true;

        foreach (var hit in hits)
            if (hit.collider.gameObject.tag == "landmarkController")
            {
                if (!hit.collider.gameObject.GetComponent<LandmarkController>().enabled) return true;
                upgradePanel.OpenPanel(hit.collider.gameObject);
                StartCamTransition(hit.collider.gameObject.transform.position, 24f);
                deltaTouchCount = 0;
                return true;
            }

        return false;
    }

    private void HandleMouseDown()
    {
        initialPoint = touchZero.position;
        targetPigi = null;
        deltaTouchCount = 1;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        foreach (var hit in hits)
            if (hit.collider.gameObject.tag == "pigi")
            {
                deltaTouchCount = -1;
                if (hit.collider.gameObject.GetComponent<PigiController>().grown)
                {
                    distCheck = false;
                    targetPigi = hit.collider.gameObject.GetComponent<PigiController>();
                    initialY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y,
                        targetPigi.transform.position.z)).y;
                }
            }

        var objects = GameObject.FindGameObjectsWithTag("pigi");
        float dist, minDist;
        var minIdx = -1;

        minDist = Mathf.Lerp(150f, 75f, (Camera.main.orthographicSize - 4.9f) / 55f);

        for (var i = 0; i < objects.Length; i++)
        {
            var obj = objects[i];
            Debug.DrawLine(Camera.main.WorldToScreenPoint(obj.transform.position), touchZero.position, Color.red, 0.1f);

            if (!objects[i].GetComponent<PigiController>().grown) continue;
            var objPos = obj.transform.position;
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
            targetPigi = objects[minIdx].GetComponent<PigiController>();
            deltaTouchCount = -1;
            targetPigi.Event_Down();
            initialY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y,
                targetPigi.transform.position.z)).y;
        }
    }

    private void HandleDrag()
    {
        touchZero = Input.GetTouch(0);
        if (targetPigi == null)
            return;

        var newY = Camera.main.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y,
            targetPigi.transform.position.z)).y;
        var objPos = targetPigi.transform.position;
        objPos.y = 0.5f;

        targetPigi.Event_Drag(newY - initialY);

        if (!distCheck & (Vector2.Distance(initialPoint, touchZero.position) > 100f))
        {
            distCheck = true;
            if ((touchZero.position.y - initialPoint.y < 0) |
                (Mathf.Abs(touchZero.position.y - initialPoint.y) * 2 <
                 Mathf.Abs(touchZero.position.x - initialPoint.x)))
            {
                deltaTouchCount = 1;
                targetPigi.Event_Up();
                targetPigi = null;
                initialPoint = touchZero.position;
            }
        }
    }

    private void HandlePinchZoom()
    {
        targetPigi = null;
        touchZero = Input.GetTouch(0);
        touchOne = Input.GetTouch(1);

        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        targetSize += deltaMagnitudeDiff * orthoZoomSpeed;
        targetSize = Mathf.Min(Mathf.Max(targetSize, 5f), 60f);
        SetSliderValue();
    }

    private Vector3 GetTargetPositioin()
    {
        var newPos = targetPosition;
        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;

        var diffX = touchZeroPrevPos.x - touchZero.position.x;
        var diffY = touchZeroPrevPos.y - touchZero.position.y;
        newPos.x += diffX * camMoveSpeed * Camera.main.orthographicSize;
        newPos.x -= diffY * camMoveSpeed * Camera.main.orthographicSize;

        newPos.z += diffX * camMoveSpeed * Camera.main.orthographicSize;
        newPos.z += diffY * camMoveSpeed * Camera.main.orthographicSize;

        newPos.x = Mathf.Min(Mathf.Max(newPos.x, -20f), 20f);
        newPos.z = Mathf.Min(Mathf.Max(newPos.z, -70f), 50f);
        return newPos;
    }

    private void UpdateCameraTransform()
    {
        if (Vector3.Distance(targetPosition, Camera.main.transform.position) > 0.1f)
        {
            var tempPos = Camera.main.transform.position;
            tempPos += (targetPosition - tempPos) * 0.25f;
            Camera.main.transform.position = tempPos;
        }
        else
        {
            Camera.main.transform.position = targetPosition;
        }

        if (Mathf.Abs(Camera.main.orthographicSize - targetSize) > 0.01f)
        {
            var tempSize = Camera.main.orthographicSize;
            tempSize += (targetSize - tempSize) * 0.25f;
            Camera.main.orthographicSize = tempSize;
        }
        else
        {
            Camera.main.orthographicSize = targetSize;
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