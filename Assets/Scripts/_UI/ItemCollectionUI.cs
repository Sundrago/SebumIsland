using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Class for managing the UI of item collections.
/// </summary>
public class ItemCollectionUI : MonoBehaviour
{
    private const int ITEMS_PER_ROW = 4;

    [SerializeField] private InfoDataManager dataManager;

    [FormerlySerializedAs("collection_row")] [SerializeField]
    private CollectionRowContorller collectionRowContorller;

    [SerializeField] private GameObject contents_holder;
    [SerializeField] private TextMeshProUGUI info_title, info_descr, info_count;
    [SerializeField] private Image info_img;
    [SerializeField] private int height = 10;

    public List<GameObject> PigiObjects { get; private set; }
    public List<CollectionRowContorller> PigiRowObjects { get; private set; }
    public List<GameObject> LandmarkObjects { get; private set; }
    public List<CollectionRowContorller> LandmarkRowObjects { get; private set; }

    public bool PigiInitialized { get; private set; }
    public bool LandmarkInitialized { get; private set; }


    public async void InitializePigiCollection()
    {
        SetDefaultState();

        var itemCount = dataManager.PigiItems.Count;
        var rowCount = GetRowCount(itemCount);

        await InstantiateRows(rowCount, itemCount);
        contents_holder.GetComponent<RectTransform>().sizeDelta =
            new Vector2(contents_holder.GetComponent<RectTransform>().sizeDelta.x, rowCount * height + 50f);
        OnCollectionPigiClick(0);
    }

    private static int GetRowCount(int itemCount)
    {
        var rowCount = (itemCount + (ITEMS_PER_ROW - itemCount % ITEMS_PER_ROW)) / ITEMS_PER_ROW;
        if (itemCount % ITEMS_PER_ROW == 0) --rowCount;
        if (itemCount == 0) rowCount = 0;
        return rowCount;
    }

    private async Task InstantiateRows(int rowCount, int itemCount)
    {
        for (var i = 0; i < rowCount; i++)
        {
            var row = Instantiate(collectionRowContorller, contents_holder.transform);
            Vector2 pos = row.transform.localPosition;
            pos.y -= height * i;
            row.transform.localPosition = pos;
            row.gameObject.SetActive(true);

            var idx = i * ITEMS_PER_ROW;
            var count = itemCount - idx >= ITEMS_PER_ROW ? 4 : itemCount % ITEMS_PER_ROW;
            PigiRowObjects.Add(row);
            await row.InitializePigiRow(idx, count);
            await Task.Delay(5);
        }
    }

    private void SetDefaultState()
    {
        for (var i = PigiRowObjects.Count - 1; i >= 0; i--)
        {
            Destroy(PigiRowObjects[i]);
            PigiRowObjects.RemoveAt(i);
        }

        PigiInitialized = true;
        PigiObjects = new List<GameObject>();
        PigiRowObjects = new List<CollectionRowContorller>();
    }

    public async void InitializeLandmarkCollection()
    {
        DestroyObjectIfExists();
        LandmarkInitialized = true;

        LandmarkObjects = new List<GameObject>();
        LandmarkRowObjects = new List<CollectionRowContorller>();

        var itemCount = dataManager.LandmarkItems.Count;
        var rowCount = GetRowCount(itemCount);

        await InstantiateCollectionRow(rowCount, itemCount);
        contents_holder.GetComponent<RectTransform>().sizeDelta =
            new Vector2(contents_holder.GetComponent<RectTransform>().sizeDelta.x, rowCount * height + 50f);
        OnCollectionLandmarkClick(0);
    }

    private async Task InstantiateCollectionRow(int rowCount, int itemCount)
    {
        for (var i = 0; i < rowCount; i++)
        {
            var row = Instantiate(collectionRowContorller, contents_holder.transform);
            Vector2 pos = row.transform.localPosition;
            pos.y -= height * i;
            row.transform.localPosition = pos;
            row.gameObject.SetActive(true);

            var idx = i * ITEMS_PER_ROW;
            var count = itemCount - idx >= ITEMS_PER_ROW ? 4 : itemCount % ITEMS_PER_ROW;
            LandmarkRowObjects.Add(row);
            await row.InitializeLandmarkRow(idx, count);
            await Task.Delay(5);
        }
    }

    private void DestroyObjectIfExists()
    {
        for (var i = LandmarkRowObjects.Count - 1; i >= 0; i--)
        {
            Destroy(LandmarkRowObjects[i]);
            LandmarkRowObjects.RemoveAt(i);
        }
    }

    public void OnCollectionPigiClick(int idx)
    {
        var ID = dataManager.PigiItems[idx].ID;
        PlayerPrefs.SetInt(ID + "_checked", 1);

        foreach (var obj in PigiRowObjects) obj.ResetItems();

        UpdatePigiUI(idx, ID);
    }

    private void UpdatePigiUI(int idx, string ID)
    {
        info_img.sprite = dataManager.PigiItems[idx].Img;
        info_img.color = PlayerPrefs.GetInt(ID + "_count") > 0
            ? new Color(1f, 1f, 1f, 1f)
            : new Color(0f, 0f, 0f, 0.7f);

        info_title.text = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "title_" + ID);
        info_descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "descr_" + ID);
        info_count.text = "획득한 피지 숫자 : " + PlayerPrefs.GetInt(ID + "_count") + "개";
    }

    public void OnCollectionLandmarkClick(int idx)
    {
        var ID = dataManager.LandmarkItems[idx].ID;
        if (PlayerPrefs.GetInt(ID + "_count") > 0) PlayerPrefs.SetInt(ID + "_checked", 1);

        foreach (var obj in LandmarkRowObjects) obj.ResetItems();

        UpdateCollectionUI(idx, ID);
    }

    private void UpdateCollectionUI(int idx, string ID)
    {
        info_img.sprite = dataManager.LandmarkItems[idx].Img;
        info_img.color = PlayerPrefs.GetInt(ID + "_count") > 0
            ? new Color(1f, 1f, 1f, 1f)
            : new Color(0f, 0f, 0f, 0.7f);
        info_title.text = LocalizationSettings.StringDatabase.GetLocalizedString("LandmarkController", ID + "_title");
        info_descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("LandmarkController", ID + "_descr");
        info_count.text = "";
    }
}