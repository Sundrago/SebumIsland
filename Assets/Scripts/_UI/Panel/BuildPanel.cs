using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Represents a panel for building objects with upgrade options.
/// </summary>
public class BuildPanel : PanelObject
{
    [FormerlySerializedAs("buildBtnSet_prefab")] [SerializeField]
    private BuildButtonManager buildButtonManagerPrefab;

    [SerializeField] private float row_height = 140f;
    [SerializeField] private GameObject btnsHolder;
    [SerializeField] private Image buildBtn;
    private List<BuildButtonManager> buildBtnSets;

    private bool initiated;
    private MoneyManager money;

    private void OnEnable()
    {
        if (!initiated) Initiate();

        GetAvailableUpgrades();
    }

    private void Initiate()
    {
        money = MoneyManager.Instance;

        buildBtnSets = new List<BuildButtonManager>();
        initiated = true;

        foreach (var landmarkItem in InfoDataManager.Instance.LandmarkItems)
        {
            if (CSVReader.Instance.GetDataList(landmarkItem.ID) == null) continue;
            var buildBtn = Instantiate(buildButtonManagerPrefab, btnsHolder.transform);
            var buildBtnPos = buildButtonManagerPrefab.GetComponent<RectTransform>().anchoredPosition;
            buildBtnPos.y -= row_height * buildBtnSets.Count;
            buildBtn.GetComponent<RectTransform>().anchoredPosition = buildBtnPos;
            buildBtn.Init(landmarkItem.ID);
            buildBtn.gameObject.SetActive(true);
            buildBtnSets.Add(buildBtn);
        }

        btnsHolder.GetComponent<RectTransform>().sizeDelta =
            new Vector2(btnsHolder.GetComponent<RectTransform>().sizeDelta.x, row_height * buildBtnSets.Count + 100f);
    }

    public void GetAvailableUpgrades()
    {
        if (!initiated) Initiate();
        var flag = false;
        foreach (var buildBtn in buildBtnSets)
        {
            buildBtn.UpdateBuildBtn();
            if (money.HasEnoughMoney(buildBtn.price)) flag = true;
        }

        buildBtn.color = flag ? Color.white : new Color(1, 1, 1, 0.5f);
    }
}