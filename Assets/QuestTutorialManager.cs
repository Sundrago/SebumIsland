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

    [SerializeField] GameObject buildBtn, buildPanel, build_farm_btn;

    int state;
    int tearsCount, tearsMaxCount = 0;
    bool completed;

    [Button]
    void Start()
    {
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

        particleFX.transform.position = Camera.main.ScreenToWorldPoint(hand.transform.position);

        switch (state)
        {
            case 1:
                //허름한 피지 농장을 건설하세요!
                if (LocationManger.Instance.allocatedObj.Count != 0) UpdateTears(1);

                if(LocationManger.Instance.settingMode)
                {
                    hand.gameObject.SetActive(false);
                    particleFX.gameObject.SetActive(false);
                } else
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
                break;  
                
            case 2:
                //베이직 피지를 네 개 수확하세요!

                break;
            case 3:
                //허름한 피지를 업그레이드 하세요!

                break;
            case 4:
                //한번에 수확 버튼을 눌러 수확하세요!

                break;
            case 5:
                //빠른 업그레이드 버튼을 눌러 업그레이드 하세요!

                break;
            case 6:
                //피지샘을 건설하세요!
                break;

        }
    }

    void ChangeState(int _state)
    {
        print(_state);
        state = _state;
        ShowQuest();

        switch(state)
        {
            case 1:
                //허름한 피지 농장을 건설하세요!
                mission_text.text = tutorial[0];
                InitiateTear(1);
                ShowQuest();
                break;
            case 2:
                //베이직 피지를 네 개 수확하세요!
                mission_text.text = tutorial[1];
                InitiateTear(4);
                ShowQuest();

                hand.gameObject.SetActive(false);
                particleFX.gameObject.SetActive(false);

                break;
            case 3:
                //허름한 피지를 업그레이드 하세요!
                mission_text.text = tutorial[2];
                InitiateTear(2);
                ShowQuest();
                break;
            case 4:
                //한번에 수확 버튼을 눌러 수확하세요!
                mission_text.text = tutorial[3];
                InitiateTear(1);
                ShowQuest();
                break;
            case 5:
                //빠른 업그레이드 버튼을 눌러 업그레이드 하세요!
                mission_text.text = tutorial[4];
                InitiateTear(4);
                ShowQuest();
                break;
            case 6:
                //피지샘을 건설하세요!
                break;

        }
    }

    void ShowQuest()
    {
        gameObject.transform.DOLocalMoveX(-0, 1f)
            .SetEase(Ease.OutExpo);
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
        print("UpdateTears");
        if (completed) return;

        tearsCount = count;

        for (int i = 0; i < 8; i++)
        {
            tears[i].sprite = i<count ? tear_filled : tear_empth;
        }

        if(count >= tearsMaxCount)
        {
            for(int i = 0; i<count; i++)
            {
                tears[i].DOFade(0f, 0.5f);
            }
            complete_img.gameObject.SetActive(true);
            complete_img.gameObject.transform.DOPunchScale(Vector3.one, 1f, 7, 0.7f)
                .OnComplete(()=> { HideQuest(); });
            completed = true;
            hand.gameObject.SetActive(false);
            particleFX.gameObject.SetActive(false);
        }

    }
}
