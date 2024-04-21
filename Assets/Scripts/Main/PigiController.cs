using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PigiController : MonoBehaviour //, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("피지 ID")] public string ID;

    [SerializeField] public Material[] obj_2d = new Material[3];
    [SerializeField] private MeshRenderer plane;

    public bool autoParticleOn = true;

    public float timer;
    public float progress;

    public float growTime = 1;
    public bool showCoin = true;
    [FormerlySerializedAs("landmark")] public LandmarkController landmarkController;
    public bool grown;
    private Animator animator;

    private float bonusMultiplier = 1f;
    private GameObject coin2d;

    private int currentStatus;
    private ParticleSystem.EmissionModule emission;
    private bool harvested;
    private ParticleSystem.MainModule main;

    private GameObject mainCamera;

    private MoneyManager money;
    private GameObject myParticle;
    private NewPigiAnimationController newPigiAnimationController;
    private bool pop;
    public Price sellPrice = new(1);
    private ParticleSystem.ShapeModule shape;
    private DateTime startTime;
    private bool timerSet;
    private TimeSpan timeSpan;

    public void Start()
    {
        newPigiAnimationController = NewPigiAnimationController.Instance;
        animator = gameObject.GetComponent<Animator>();

        money = MoneyManager.Instance;
        mainCamera = GameObject.Find("Main Camera"); //Main Camera
        coin2d = GameObject.Find("UI-CANVAS"); //UI-CANVAS

        StartTimer();
    }

    private void Update()
    {
        if (timerSet)
        {
            timeSpan = DateTime.Now - startTime;
            timer = (float)timeSpan.TotalSeconds / growTime;

            progress = timer * 100 / 90f;
            progress = Mathf.Max(Mathf.Min(progress, 1f), 0.01f);

            if ((timer > 0.3f) & (currentStatus == 0))
            {
                currentStatus = 1;
                PlayAnim(currentStatus);
            }
            else if ((timer > 0.5f) & (currentStatus == 1))
            {
                currentStatus = 2;
                PlayAnim(currentStatus);
            }
            else if ((timer >= 0.8f) & (currentStatus == 2))
            {
                currentStatus = 3;
                PlayAnim(currentStatus);
            }
        }

        if (harvested & (myParticle != null) && myParticle.gameObject.activeSelf)
            shape.position = gameObject.transform.position;

        if (harvested & (mainCamera.GetComponent<Camera>().WorldToScreenPoint(gameObject.transform.position).y >
                         Screen.height))
        {
            AddMoney();
            AnimFinished();
            harvested = false;
        }
    }

    public void Init(LandmarkController landmarkController, Vector3 _pos, string _ID, float _bonusMultiplier = 1f)
    {
        ID = _ID;
        this.landmarkController = landmarkController;
        gameObject.transform.parent.gameObject.transform.localPosition = _pos;
        gameObject.transform.parent.gameObject.SetActive(true);
        bonusMultiplier = _bonusMultiplier;
        Set2DOBJ(0);
    }

    private void PlayAnim(int currentStatus)
    {
        //print(currentStatus);
        animator.SetTrigger("anim-" + currentStatus);

        if (currentStatus == 0) Set2DOBJ(0);
    }

    public void StartTimer()
    {
        growTime = landmarkController.GrowTime * Random.Range(0.6f, 1.5f);
        sellPrice = landmarkController.SellPrice;

        currentStatus = 0;
        PlayAnim(currentStatus);
        startTime = DateTime.Now;
        timerSet = true;
        grown = false;
    }

    private void AnimFinished()
    {
        if (harvested) AddMoney();

        if (myParticle.gameObject.activeSelf) emission.rateOverTime = 0;

        ResetAllAnimatorTriggers(animator);
        var newPos = transform.parent.gameObject.transform.position;
        newPos.y = 0;
        transform.parent.gameObject.transform.position = newPos;
        StartTimer();
        pop = false;
    }

    private void AddMoney()
    {
        newPigiAnimationController.GotPigi(ID);
        if (!showCoin) return;
        money.AddCoin2D(3, Camera.main.WorldToScreenPoint(gameObject.transform.position));

        var price = new Price(Mathf.RoundToInt(landmarkController.SellPrice.amount * bonusMultiplier),
            landmarkController.SellPrice.charCode);
        money.AddMoney(price);
        harvested = false;
    }

    public void AutoHarvest()
    {
        currentStatus = 3;
        harvested = true;
        grown = false;
        timerSet = false;
        ResetAllAnimatorTriggers(animator);
        animator.SetTrigger("anim-5");
        animator.speed = Random.Range(0.7f, 1.4f);

        if (!autoParticleOn) return;
        CreateAndInitFlyFX(10);
    }

    private void ResetAllAnimatorTriggers(Animator animator)
    {
        foreach (var trigger in animator.parameters)
            if (trigger.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(trigger.name);
    }

    public void Set2DOBJ(int idx)
    {
        plane.material = obj_2d[idx];

        if (idx == 2)
        {
            grown = true;
            timerSet = false;
            progress = 1f;
            landmarkController.PigiIsReady(gameObject);
        }
    }

    public void Event_Down()
    {
        CreateAndInitFlyFX();
    }

    public void Event_Drag(float targetY)
    {
        targetY *= 2f;

        if ((targetY < 3f) & (targetY > 0)) targetY = EaseInExpo(targetY / 3f) * 3f;

        if (!pop)
            if (targetY > 2.5f)
            {
                pop = true;
                FXManager.Instance.CreateFX(FXType.PigiPopFX, gameObject.transform);
                AudioManager.Instance.PlaySFXbyTag(SFX_tag.Pigipop);
                landmarkController.RemovePigiIsReady(gameObject);
            }

        float EaseInExpo(float x)
        {
            return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
        }

        //curPosition.x = transform.parent.gameObject.transform.position.x;
        //curPosition.z = transform.parent.gameObject.transform.position.z;


        var targetPos = transform.parent.gameObject.transform.position;
        targetPos.y = targetY;

        transform.parent.gameObject.transform.position = targetPos;

        var normal = transform.parent.gameObject.transform.position.y / 6;
        if (normal >= 1) normal = 0.99f;
        else if (normal < 0) normal = 0;

        if (myParticle.gameObject.activeSelf) shape.position = gameObject.transform.position;
        animator.Play("pigi_4", 0, normal);
    }

    public void Event_Up()
    {
        if (transform.parent.gameObject.transform.position.y < 2.5f)
        {
            if (myParticle.gameObject.activeSelf) emission.rateOverTime = 0;
            pop = false;
            var newPos = transform.parent.gameObject.transform.position;
            newPos.y = 0;
            transform.parent.gameObject.transform.position = newPos;
            animator.SetTrigger("anim-3-idle");
            animator.speed = 1f;
        }
        else
        {
            if (myParticle.gameObject.activeSelf)
            {
                main.startSpeed = 30f;
                emission.rateOverTime = 30;
            }

            harvested = true;
            grown = false;
            animator.SetTrigger("anim-5");
            animator.speed = 1f;

            if (landmarkController.GrownPigis.Contains(gameObject))
                landmarkController.GrownPigis.Remove(gameObject);
        }
    }

    private void CreateAndInitFlyFX(int count = 20)
    {
        myParticle = FXManager.Instance.CreateFX(FXType.FlyFX).gameObject;
        shape = myParticle.GetComponent<ParticleSystem>().shape;
        shape.position = gameObject.transform.position;
        main = myParticle.GetComponent<ParticleSystem>().main;
        main.startSpeed = 10f;
        emission = myParticle.GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = count;
    }
}