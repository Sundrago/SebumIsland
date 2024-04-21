using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Controls the behavior of a row in a collection.
/// </summary>
public class CollectionRowContorller : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = new();
    [SerializeField] private List<Image> new_icons = new();
    [SerializeField] private InfoDataManager dataManager;

    [FormerlySerializedAs("itemCollection")] [FormerlySerializedAs("collection_pigi")] [SerializeField]
    private ItemCollectionUI itemCollectionUI;

    private int current_idx, current_count;
    private bool isPigi;

    public async Task InitializePigiRow(int idx, int count)
    {
        current_idx = idx;
        current_count = count;
        isPigi = true;

        for (var i = 0; i < 4; i++)
        {
            if (i >= count)
            {
                items[i].SetActive(false);
                continue;
            }

            var btnIdx = idx + i;

            //Null exception
            if (dataManager.PigiItems[btnIdx].Img == null) continue;
            UpdateUI(i, btnIdx);

            var ID = dataManager.PigiItems[btnIdx].ID;
            var known = PlayerPrefs.GetInt(ID + "_count") > 0 ? true : false;
            var isChecked = PlayerPrefs.GetInt(ID + "_checked") > 0 ? true : false;

            UpdateUIColor(known, i, isChecked);
        }

        await Task.Yield();
    }

    private void UpdateUI(int i, int btnIdx)
    {
        if (!itemCollectionUI.PigiObjects.Contains(items[i]))
        {
            itemCollectionUI.PigiObjects.Add(items[i]);
            items[i].GetComponent<Button>().onClick.AddListener(() => itemCollectionUI.OnCollectionPigiClick(btnIdx));
            items[i].GetComponent<Image>().sprite = dataManager.PigiItems[btnIdx].Img;
        }
    }

    private void UpdateUIColor(bool known, int i, bool isChecked)
    {
        if (!known)
        {
            items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            new_icons[i].gameObject.SetActive(false);
        }
        else if (!isChecked)
        {
            items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            new_icons[i].gameObject.SetActive(true);
        }
        else
        {
            items[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            new_icons[i].gameObject.SetActive(false);
        }
    }

    public async Task InitializeLandmarkRow(int idx, int count)
    {
        current_idx = idx;
        current_count = count;
        isPigi = false;

        for (var i = 0; i < 4; i++)
        {
            if (i >= count)
            {
                items[i].SetActive(false);
                continue;
            }

            var btnIdx = idx + i;

            if (UpdateLandmarkUI(btnIdx, i)) continue;

            var ID = dataManager.LandmarkItems[btnIdx].ID;
            var known = PlayerPrefs.GetInt(ID + "_count") > 0 ? true : false;
            var isChecked = PlayerPrefs.GetInt(ID + "_checked") > 0 ? true : false;

            //unknown
            UpdateUIColor(known, i, isChecked);
        }

        await Task.Yield();
    }

    private bool UpdateLandmarkUI(int btnIdx, int i)
    {
        if (dataManager.PigiItems[btnIdx].Img == null) return true;

        if (!itemCollectionUI.LandmarkObjects.Contains(items[i]))
        {
            itemCollectionUI.LandmarkObjects.Add(items[i]);
            items[i].GetComponent<Button>().onClick
                .AddListener(() => itemCollectionUI.OnCollectionLandmarkClick(btnIdx));
            items[i].GetComponent<Image>().sprite = dataManager.LandmarkItems[btnIdx].Img;
        }

        return false;
    }

    public async void ResetItems()
    {
        if (isPigi)
            await InitializePigiRow(current_idx, current_count);
        else
            await InitializeLandmarkRow(current_idx, current_count);
        await Task.Yield();
    }
}