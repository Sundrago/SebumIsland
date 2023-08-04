using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PigiCtrl : MonoBehaviour//, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("피지 ID")]
    public string ID;

    bool debug2d = true;
    public Material[] obj_2d = new Material[3];
    public MeshRenderer plane;

    public bool autoParticleOn = true;

    public GameObject pigi, Pigi_ParticleFX, Pigi_particleFX_pop;
    public float timer;


    public PigiInfoPanel pigiInfoPanel;
    public float growTime = 1;
    public Price sellPrice = new Price(1, "a");
    public bool showCoin = true;
    public Landmark landmark;

    private int currentStatus = 0;

    private const float dragAmount = 2f;

    private bool timerSet = false;
    public bool grown = false;
    private bool harvested = false;
    private bool pop = false;

    private System.DateTime startTime;
    private System.TimeSpan timeSpan;

    private Vector2 initialPoint;
    private Vector3 offset;

    private GameObject mainCamera;
    private GameObject coin2d;
    private GameObject myParticle;

    public float progress;
    public AudioCtrl myAudio;

    public NewPigiCtrl newPigiCtrl;

    public void Start()
    {
        Debug_toggle2D(true);
        
        mainCamera = GameObject.Find("Main Camera"); //Main Camera
        coin2d = GameObject.Find("UI-CANVAS"); //UI-CANVAS

        landmark = gameObject.GetComponentInParent<Landmark>();
        StartTimer();
    }

    void Update()
    {
        if (timerSet)
        {
            timeSpan = System.DateTime.Now - startTime;
            timer = (float)timeSpan.TotalSeconds / growTime;

            progress = timer * 100 / 90f;
            progress = Mathf.Max(Mathf.Min(progress, 1f), 0.01f);

            if (timer > 0.3f & currentStatus == 0)
            {
                currentStatus = 1;
                PlayAnim(currentStatus);
            }
            else if (timer > 0.5f & currentStatus == 1)
            {
                currentStatus = 2;
                PlayAnim(currentStatus);
            }
            else if (timer >= 0.8f & currentStatus == 2)
            {
                currentStatus = 3;
                PlayAnim(currentStatus);
            }
        }

        if (harvested & myParticle != null)
        {
            var shape = myParticle.GetComponent<ParticleSystem>().shape;
            shape.position = gameObject.transform.position;
        }

        if (harvested & mainCamera.GetComponent<Camera>().WorldToScreenPoint(gameObject.transform.position).y > Screen.height)
        {
            AddMoney();
            AnimFinished();
            harvested = false;
        }
    }

    void PlayAnim(int currentStatus)
    {
        //print(currentStatus);
        pigi.GetComponent<Animator>().SetTrigger("anim-" + currentStatus);

        if(currentStatus == 0) Set2DOBJ(0);
        //if (!debug2d) return;
        //switch(currentStatus)
        //{
        //    case 0:
        //        plane.material = obj_2d[0];
        //        break;
        //    case 1:
        //        plane.material = obj_2d[1];
        //        break;
        //    case 2:
        //        plane.material = obj_2d[1];
        //        break;
        //    default:
        //        plane.material = obj_2d[2];
        //        break;
        //}
    }

    public void StartTimer()
    {
        growTime = landmark.growTime * Random.Range(0.6f, 1.5f);
        sellPrice = landmark.sellPrice;

        currentStatus = 0;
        PlayAnim(currentStatus);
        startTime = System.DateTime.Now;
        timerSet = true;
        grown = false;
    }

    //public void OnPointerDown(PointerEventData pointerEventData)
    //{
    //    if (!grown) return;
    //    initialPoint = Camera.main.ScreenToWorldPoint(pointerEventData.position);
    //    //offset = transform.parent.gameObject.transform.position - Camera.main.ScreenToWorldPoint(pointerEventData.position);

    //    pigi.GetComponent<Animator>().speed = 0f;

    //    myParticle = Instantiate(Pigi_ParticleFX);
    //    myParticle.SetActive(true);
    //    var shape = myParticle.GetComponent<ParticleSystem>().shape;
    //    shape.position = gameObject.transform.position;
    //    var main = myParticle.GetComponent<ParticleSystem>().main;
    //    main.startSpeed = 10f;
    //}

    //public void OnPointerUp(PointerEventData pointerEventData)
    //{
    //    if (!grown) return;

    //    if (transform.parent.gameObject.transform.position.y < 2f)
    //    {
    //        if (myParticle != null) Destroy(myParticle);
    //        pop = false;
    //        Vector3 newPos = transform.parent.gameObject.transform.position;
    //        newPos.y = 0;
    //        transform.parent.gameObject.transform.position = newPos;
    //        pigi.GetComponent<Animator>().SetTrigger("anim-3-idle");
    //        pigi.GetComponent<Animator>().speed = 1f;
    //    }
    //    else
    //    {
    //        if(myParticle != null)
    //        { 
    //        var main = myParticle.GetComponent<ParticleSystem>().main;
    //        main.startSpeed = 30f;
    //        var emission = myParticle.GetComponent<ParticleSystem>().emission;
    //        emission.rateOverTime = 30;
    //        }

    //        harvested = true;
    //        grown = false;
    //        pigi.GetComponent<Animator>().SetTrigger("anim-5");
    //        pigi.GetComponent<Animator>().speed = 1f;

    //        if (gameObject.GetComponentInParent<Landmark>().grownPigis.Contains(gameObject))
    //            gameObject.GetComponentInParent<Landmark>().grownPigis.Remove(gameObject);
    //    }
    //}

    //public void OnDrag(PointerEventData pointerEventData)
    //{
    //    if (!grown) return;

    //    //Vector3 curScreenPoint = new Vector3(pointerEventData.position.x, pointerEventData.position.y, 0);
    //    //Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

    //    Vector3 curPosition = transform.parent.gameObject.transform.position;
    //    Vector2 currentPoint = Camera.main.ScreenToWorldPoint(pointerEventData.position);
    //    if(initialPoint.y < currentPoint.y)
    //    {
    //        float dist = Vector2.Distance(initialPoint, currentPoint);
    //        if(dist < 2f)
    //        {
    //            curPosition.y = EaseInExpo(dist / 2f) * 2f;
    //        } else
    //        {
    //            curPosition.y = dist * 2f;
    //        }
    //    }

    //    if (!pop)
    //    {
    //        if(gameObject.transform.position.y > 2f)
    //        {
    //            pop = true;
    //            GameObject Popticle = Instantiate(Pigi_particleFX_pop);
    //            Popticle.transform.position = new Vector3(gameObject.transform.position.x, Popticle.transform.position.y, gameObject.transform.position.z);
    //            Popticle.SetActive(true);
    //        }
    //    }

    //    float EaseInExpo(float x){
    //        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    //    }

    //    //curPosition.x = transform.parent.gameObject.transform.position.x;
    //    //curPosition.z = transform.parent.gameObject.transform.position.z;

    //    transform.parent.gameObject.transform.position = curPosition;

    //    float normal = (transform.parent.gameObject.transform.position.y) / 6;
    //    if (normal >= 1) normal = 0.99f;
    //    else if (normal < 0) normal = 0;

    //    if (myParticle != null)
    //    {
    //        var shape = myParticle.GetComponent<ParticleSystem>().shape;
    //        shape.position = gameObject.transform.position;
    //    }
    //    pigi.GetComponent<Animator>().Play("pigi_4", 0, normal);
    //}

    private void AnimFinished()
    {
        if (harvested) AddMoney();
        ResetAllAnimatorTriggers(pigi.GetComponent<Animator>());
        Vector3 newPos = transform.parent.gameObject.transform.position;
        newPos.y = 0;
        transform.parent.gameObject.transform.position = newPos;
        StartTimer();

        if(myParticle!= null)
        {
            var main = myParticle.GetComponent<ParticleSystem>().main;
            main.loop = false;
            var emission = myParticle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 0;
        }

        pop = false;
    }

    private void AddMoney()
    {
        newPigiCtrl.GotPigi(ID);
        if (!showCoin) return;
        coin2d.GetComponent<CoinAnimation2D>().Addcoin(3, Camera.main.WorldToScreenPoint(gameObject.transform.position));
        Camera.main.GetComponent<MoneyUI>().AddMoney(landmark.sellPrice);
        harvested = false;
    }

    public void AutoHarvest()
    {
        currentStatus = 3;
        harvested = true;
        grown = false;
        timerSet = false;
        ResetAllAnimatorTriggers(pigi.GetComponent<Animator>());
        pigi.GetComponent<Animator>().SetTrigger("anim-5");
        pigi.GetComponent<Animator>().speed = Random.Range(0.7f,1.4f);

        if (!autoParticleOn) return;
        if (myParticle != null) Destroy(myParticle);
        myParticle = Instantiate(Pigi_ParticleFX);
        myParticle.SetActive(true);
        var shape = myParticle.GetComponent<ParticleSystem>().shape;
        shape.position = gameObject.transform.position;
        var main = myParticle.GetComponent<ParticleSystem>().main;
        main.startSpeed = 10f;
    }

    private void ResetAllAnimatorTriggers(Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    public void Debug_toggle2D(bool toggle)
    {
        debug2d = toggle;

        if(debug2d)
        {
            plane.gameObject.SetActive(true);
            GetComponent<MeshRenderer>().enabled = false;
        } else
        {
            plane.gameObject.SetActive(false);
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void Set2DOBJ(int idx)
    {
        if (!debug2d) return;
        plane.material = obj_2d[idx];

        if(idx == 2)
        {
            grown = true;
            timerSet = false;
            progress = 1f;
            gameObject.GetComponentInParent<Landmark>().PigiIsReady(gameObject);
        }
    }

    public void Event_Down()
    {
        myParticle = Instantiate(Pigi_ParticleFX);
        myParticle.SetActive(true);
        var shape = myParticle.GetComponent<ParticleSystem>().shape;
        shape.position = gameObject.transform.position;
        var main = myParticle.GetComponent<ParticleSystem>().main;
        main.startSpeed = 10f;
    }

    public void Event_Drag(float targetY)
    {
        targetY *= 2f;

        if (targetY < 3f & targetY > 0)
        {
            targetY = EaseInExpo(targetY / 3f) * 3f;
        }

        if (!pop)
        {
            if (targetY > 2.5f)
            {
                pop = true;
                GameObject Popticle = Instantiate(Pigi_particleFX_pop);
                Popticle.transform.position = new Vector3(gameObject.transform.position.x, Popticle.transform.position.y, gameObject.transform.position.z);
                Popticle.SetActive(true);
                myAudio.PlaySFX(0);
            }
        }

        float EaseInExpo(float x)
        {
            return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
        }

        //curPosition.x = transform.parent.gameObject.transform.position.x;
        //curPosition.z = transform.parent.gameObject.transform.position.z;


        Vector3 targetPos = transform.parent.gameObject.transform.position;
        targetPos.y = targetY;

        transform.parent.gameObject.transform.position = targetPos;

        float normal = (transform.parent.gameObject.transform.position.y) / 6;
        if (normal >= 1) normal = 0.99f;
        else if (normal < 0) normal = 0;

        if (myParticle != null)
        {
            var shape = myParticle.GetComponent<ParticleSystem>().shape;
            shape.position = gameObject.transform.position;
        }
        pigi.GetComponent<Animator>().Play("pigi_4", 0, normal);
    }

    public void Event_Up()
    {
        if (transform.parent.gameObject.transform.position.y < 2.5f)
        {
            if (myParticle != null) Destroy(myParticle);
            pop = false;
            Vector3 newPos = transform.parent.gameObject.transform.position;
            newPos.y = 0;
            transform.parent.gameObject.transform.position = newPos;
            pigi.GetComponent<Animator>().SetTrigger("anim-3-idle");
            pigi.GetComponent<Animator>().speed = 1f;
        }
        else
        {
            if (myParticle != null)
            {
                var main = myParticle.GetComponent<ParticleSystem>().main;
                main.startSpeed = 30f;
                var emission = myParticle.GetComponent<ParticleSystem>().emission;
                emission.rateOverTime = 30;
            }

            harvested = true;
            grown = false;
            pigi.GetComponent<Animator>().SetTrigger("anim-5");
            pigi.GetComponent<Animator>().speed = 1f;

            if (gameObject.GetComponentInParent<Landmark>().grownPigis.Contains(gameObject))
                gameObject.GetComponentInParent<Landmark>().grownPigis.Remove(gameObject);
        }
    }
}