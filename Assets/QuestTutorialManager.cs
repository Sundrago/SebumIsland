using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class QuestTutorialManager : MonoBehaviour
{
    [SerializeField] Sprite tear_filled, tear_empth;
    [SerializeField] Transform tearLeft, tearRight;
    [SerializeField] GameObject tear_prefab;
    [SerializeField] float tearWidth;
    [SerializeField] GameObject complete_img;
    [SerializeField] TextMeshProUGUI mission_text;

    [SerializeField] List<string> tutorial = new List<string>();
    [SerializeField] List<Image> tears;

    [SerializeField] GameObject hand, particleFX;

    [TableList(ShowIndexLabels = true)]
    public List<QuestData> questDatas;
    
    int state;
    int tearsCount, tearsMaxCount = 0;
    bool completed;

    public static QuestTutorialManager Instance;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        Instance = this;
    }

    [Button]
    void Start()
    {
        MoneyUI.Instance.AddMoney(new Price(5));

        ChangeState(1);

        //for(int i = 0; i<8; i++)
        //{
        //    GameObject tear = Instantiate(tear_prefab, gameObject.transform);
        //    tear.transform.position = Vector3.Lerp(tearLeft.position, tearRight.position, 0.5f);
        //    tears.Add(tear.GetComponent<Image>());
        //}
    } 

   
    void Update()
    {
        if (Time.frameCount % 10 != 0) return;
        if (completed) return;

        switch (state)
        {
            case 1:
                CheckTutorial_1();
                break;
            case 2:
                CheckTutorial_2();
                break;
            case 3:
                CheckTutorial_3();
                break;
            case 4:
                CheckTutorial_4();
                break;
            case 5:
                CheckTutorial_5();
                break;
            case 6:
                CheckTutorial_6();
                break;

        }
    }

    void ChangeState(int _state)
    {
        state = _state;

        switch(state)
        {
            case 1:
                InitTutorial_1();
                break;
            case 2:
                InitTutorial_2();
                break;
            case 3:
                InitTutorial_3();
                break;
            case 4:
                InitTutorial_4();
                break;
            case 5:
                InitTutorial_5();
                break;
            case 6:
                InitTutorial_6();
                break;

        }
    }

    void ShowQuest()
    {
        gameObject.transform.DOLocalMoveX(-0, 1f)
            .SetEase(Ease.OutExpo);

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.quest_arrive);
    }

    void HideQuest()
    {
        gameObject.transform.DOLocalMoveX(-350, 1f)
            .SetEase(Ease.OutExpo)
            .OnComplete(()=> { ChangeState(state + 1); });
    }

    void CompleteQuest()
    {

    }

    [Button]
    void InitiateTear(int count)
    {
        Vector3 targetpos = Vector3.zero;

        for (int i = 0; i < 8; i++)
        {
            //Initiate Fades
            if (i < count) tears[i].color = Color.white;
            tears[i].gameObject.SetActive(i < count);
            tears[i].sprite = tear_empth;

            //Initiate Pos
            if (i == 0)
            {
                tears[i].transform.position = Vector3.Lerp(tearLeft.position, tearRight.position, 0.5f);
                targetpos = tears[i].GetComponent<RectTransform>().localPosition;
                targetpos.x -= (count - 1) * tearWidth * 1 / 2f;
                tears[i].GetComponent<RectTransform>().localPosition = targetpos;
            } else
            {
                targetpos.x += tearWidth;
                tears[i].GetComponent<RectTransform>().localPosition = targetpos;
            }
        }

        tearsCount = 0;
        tearsMaxCount = count;
        completed = false;
        complete_img.gameObject.SetActive(false);
    }

    [Button]
    void UpdateTears(int count)
    {
        print("UpdateTears "+count);
        if (completed) return;

        tearsCount = count;

        for (int i = 0; i < 8; i++)
        {
            tears[i].sprite = i<count ? tear_filled : tear_empth;
        }

        if(count >= tearsMaxCount)
        {
            //clear
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.quest_clear);
            for (int i = 0; i<count; i++)
            {
                tears[i].DOFade(0f, 0.5f);
            }
            complete_img.gameObject.SetActive(true);
            complete_img.gameObject.transform.DOPunchScale(Vector3.one, 1f, 7, 0.7f)
                .OnComplete(()=> { HideQuest();
                    hand.SetActive(false);
                    particleFX.SetActive(false);
                });
            completed = true;
            hand.SetActive(false);
            particleFX.SetActive(false);
        }
    }



    /* ------------------------------------------------------------------------ */

    //허름한 피지 농장을 건설하세요!
    [TitleGroup("Tutorial 1")]
    [SerializeField] GameObject buildBtn, buildPanel, build_farm_btn;

    void InitTutorial_1()
    {
        
        mission_text.text = tutorial[0];
        InitiateTear(1);
        ShowQuest();
    }

    void CheckTutorial_1()
    {
        particleFX.transform.position = Camera.main.ScreenToWorldPoint(hand.transform.position);

        if (LocationManger.Instance.allocatedObj.Count != 0) UpdateTears(1);

        if (LocationManger.Instance.settingMode)
        {
            hand.gameObject.SetActive(false);
            particleFX.gameObject.SetActive(false);
        }
        else
        {
            hand.gameObject.SetActive(true);
            particleFX.gameObject.SetActive(true);

            if (buildPanel.activeSelf)
            {
                hand.transform.position = build_farm_btn.transform.position;
            }
            else
            {
                hand.transform.position = buildBtn.transform.position;
            }
        }
    }

    /* ------------------------------------------------------------------------ */

    //베이직 피지를 네 개 수확하세요!
    [TitleGroup("Tutorial 2")]
    int basicPigiCount;

    void InitTutorial_2()
    {
        basicPigiCount = 0;
        mission_text.text = tutorial[1];
        InitiateTear(4);
        ShowQuest();
    }

    void CheckTutorial_2()
    {

    }

    public void AddBasicPigiCount()
    {
        if (state != 2) return;
        basicPigiCount += 1;
        UpdateTears(basicPigiCount);
    }

    /* ------------------------------------------------------------------------ */

    //허름한 농장을 업그레이드 하세요!
    [TitleGroup("Tutorial 3")]
    int farm0UpdateCount;

    void InitTutorial_3()
    {
        farm0UpdateCount = 0;
        mission_text.text = tutorial[2];
        InitiateTear(1);
        ShowQuest();
    }

    void CheckTutorial_3()
    {

    }

    public void Addfarm0UpdateCount()
    {
        if (state != 3) return;
        farm0UpdateCount += 1;
        UpdateTears(farm0UpdateCount);
    }

    /* ------------------------------------------------------------------------ */

    //한번에 수확 버튼을 눌러 수확하세요!
    [TitleGroup("Tutorial 4")]
    int harvestAllClickCount;


    void InitTutorial_4()
    {
        harvestAllClickCount = 0;
        mission_text.text = tutorial[3];
        InitiateTear(1);
        ShowQuest();
    }

    void CheckTutorial_4()
    {

    }

    public void AddHarvestAllClickCount()
    {
        if (state != 4) return;
        harvestAllClickCount += 1;
        UpdateTears(harvestAllClickCount);
    }

    /* ------------------------------------------------------------------------ */

    //빠른 업그레이드 버튼을 눌러 업그레이드 하세요!
    [TitleGroup("Tutorial 5")]
    int fastUpgradeClickCount;

    void InitTutorial_5()
    {
        fastUpgradeClickCount = 0;
        mission_text.text = tutorial[4];
        InitiateTear(1);
        ShowQuest();
    }

    void CheckTutorial_5()
    {

    }

    public void AddFastUpgradeClickCount()
    {
        if (state != 5) return;
        fastUpgradeClickCount += 1;
        UpdateTears(fastUpgradeClickCount);
    }

    /* ------------------------------------------------------------------------ */


    //피지샘을 건설하세요!
    [TitleGroup("Tutorial 6")]


    void InitTutorial_6()
    {
        mission_text.text = tutorial[5];
        InitiateTear(1);
        ShowQuest();
    }

    void CheckTutorial_6()
    {

    }

    public void OilFarm0Build()
    {
        if (state != 6) return;
        UpdateTears(1);
    }

    /* ------------------------------------------------------------------------ */

    //Quest Checker - landmark, pigiCount, 
}

[Serializable]
public class QuestData
{
    [VerticalGroup("Strings")]
    [LabelWidth(50)]
    public string Descr;
    
    [VerticalGroup("Quest")]
    [LabelText("type"), LabelWidth(30)]
    public QuestType questType;
    
    [VerticalGroup("Quest")]
    [LabelText("amt"), LabelWidth(30)]
    [DisableIf("questType", QuestType.Custom)]
    public int questamt;
    
    [VerticalGroup("Quest")]
    [LabelText("ID"), LabelWidth(30)]
    public String quesetID;

    [VerticalGroup("Reward")]
    [LabelText("type"), LabelWidth(30)]
    public RewardType rewardType;
    
    [VerticalGroup("Reward")]
    [LabelText("amt"), LabelWidth(30)]
    public int rewardAmt;
    
    [VerticalGroup("Reward")]
    [DisableIf("@this.rewardType == RewardType.jewel || this.rewardType == RewardType.oil")]
    [LabelText("ID"), LabelWidth(30)]
    public string reardID;

    [VerticalGroup("Strings")]
    [TableList]
    public List<BalloonMsgData> EndString;
}

public enum QuestType { CollectPigi, BuildLandmark, CollectMoney, Upgrade, Custom }
public enum RewardType { money, jewel, oil }

[Serializable]
public class BalloonMsgData
{
    [HorizontalGroup(Width = 0.25f), HideLabel]
    public String ID = "default";
    [HorizontalGroup, HideLabel]
    public String Descr;
}