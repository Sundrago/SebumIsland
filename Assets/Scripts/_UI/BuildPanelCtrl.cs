using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BuildPanelCtrl : MonoBehaviour
{
    [SerializeField] BuildBtnSet buildBtnSet_prefab;
    [SerializeField] float row_height = 140f;
    [SerializeField] GameObject btnsHolder;
    [SerializeField] Image buildBtn;

    private bool initiated = false;
    private List<BuildBtnSet> buildBtnSets;
    MoneyUI money;

    private async void Initiate()
    {
        money = MoneyUI.Instance;

        buildBtnSets = new List<BuildBtnSet>();
        initiated = true;

        foreach (LandmarkItem landmarkItem in InfoDataManager.Instance.LandmarkItems)
        {
            if (CSVReader.Instance.GetDataList(landmarkItem.ID) == null) return;
            BuildBtnSet buildBtn = Instantiate(buildBtnSet_prefab, btnsHolder.transform);
            Vector2 buildBtnPos = buildBtnSet_prefab.GetComponent<RectTransform>().anchoredPosition;
            buildBtnPos.y -= row_height * buildBtnSets.Count;
            buildBtn.GetComponent<RectTransform>().anchoredPosition = buildBtnPos;
            await buildBtn.Init(landmarkItem.ID);
            buildBtn.gameObject.SetActive(true);
            buildBtnSets.Add(buildBtn);
        }

        btnsHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(btnsHolder.GetComponent<RectTransform>().sizeDelta.x, row_height * buildBtnSets.Count + 100f);
    }

    private void OnEnable()
    {
        if (!initiated) Initiate();

        GetAvailableUpgrades();
    }

    public void GetAvailableUpgrades()
    {
        if (!initiated) Initiate();
        bool flag = false;
        foreach (BuildBtnSet buildBtn in buildBtnSets)
        {
            buildBtn.UpdateBuildBtn();
            if (money.HasEnoughMoney(buildBtn.price)) flag = true;
        }

        buildBtn.color = flag ? Color.white : new Color(1, 1, 1, 0.5f);
    }

    //private void Update()
    //{
    //    if (Time.frameCount % 60 == 0)
    //    {
    //        GetAvailableUpgrades();
    //    }
    //}

    public void OpenPanel()
    {
        if (gameObject.activeSelf) return;
        PanelManager.Instance.CloseOtherPanels(gameObject);

        //SHOW ANIM
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

    public void ClosePanel()
    {
        if (!gameObject.activeSelf) return;

        //HIDE ANIM
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