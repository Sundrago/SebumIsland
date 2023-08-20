using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class Collection_Pigi : MonoBehaviour
{
    [SerializeField] InfoDataManager dataManager;
    [SerializeField] GameObject collection_row;
    [SerializeField] GameObject contents_holder;
    [SerializeField] TextMeshProUGUI info_title, info_descr, info_count;
    [SerializeField] Image info_img;
    
    [ReadOnly]
    public List<GameObject> Pigi_objects;
    public List<GameObject> Pigi_Row_objects;
    public List<GameObject> Landmark_objects;
    public List<GameObject> Landmark_Row_objects;

    public int height = 10;

    const int ItemsPerRow = 4;
    public bool PigiInitialized = false;
    public bool LandmarkInitialized = false;

    [Button]
    public async void InitializePigiCollection() {
        for (int i = Pigi_Row_objects.Count - 1; i >= 0; i--)
        {
            Destroy(Pigi_Row_objects[i]);
            Pigi_Row_objects.RemoveAt(i);
        }

        PigiInitialized = true;

        Pigi_objects = new List<GameObject>();
        Pigi_Row_objects = new List<GameObject>();

        int itemCount = dataManager.PigiItems.Count;
        int rowCount = (itemCount + (ItemsPerRow - itemCount%ItemsPerRow)) / ItemsPerRow;
        if(itemCount % ItemsPerRow == 0) --rowCount;
        if(itemCount == 0) rowCount = 0;

        Debug.Log("itemCount = " + itemCount + ", rowCount = " + rowCount);
        for(int i = 0; i<rowCount; i++) {
            GameObject row = Instantiate(collection_row, contents_holder.transform);
            Vector2 pos = row.transform.localPosition;
            pos.y -= height*i;
            row.transform.localPosition = pos;
            row.SetActive(true);
            
            int idx = i*ItemsPerRow;
            int count = (itemCount - idx >= ItemsPerRow) ? 4 : itemCount % ItemsPerRow;
            Pigi_Row_objects.Add(row);

            await row.GetComponent<collection_row>().InitializePigiRow(idx, count);
            await Task.Delay(5);
        }
        contents_holder.GetComponent<RectTransform>().sizeDelta = new Vector2(contents_holder.GetComponent<RectTransform>().sizeDelta.x, rowCount * height + 50f);
        Collection_Pigi_Clicked(0);
    }

    [Button]
    public async void InitializeLandmarkCollection()
    {
        for(int i = Landmark_Row_objects.Count-1; i>=0; i--)
        {
            Destroy(Landmark_Row_objects[i]);
            Landmark_Row_objects.RemoveAt(i);
        }

        LandmarkInitialized = true;

        Landmark_objects = new List<GameObject>();
        Landmark_Row_objects = new List<GameObject>();

        int itemCount = dataManager.LandmarkItems.Count;
        int rowCount = (itemCount + (ItemsPerRow - itemCount % ItemsPerRow)) / ItemsPerRow;
        if (itemCount % ItemsPerRow == 0) --rowCount;
        if (itemCount == 0) rowCount = 0;

        for (int i = 0; i < rowCount; i++)
        {
            GameObject row = Instantiate(collection_row, contents_holder.transform);
            Vector2 pos = row.transform.localPosition;
            pos.y -= height * i;
            row.transform.localPosition = pos;
            row.SetActive(true);

            int idx = i * ItemsPerRow;
            int count = (itemCount - idx >= ItemsPerRow) ? 4 : itemCount % ItemsPerRow;
            Landmark_Row_objects.Add(row);

            await row.GetComponent<collection_row>().InitializeLandmarkRow(idx, count);
            await Task.Delay(5);
        }
        contents_holder.GetComponent<RectTransform>().sizeDelta = new Vector2(contents_holder.GetComponent<RectTransform>().sizeDelta.x, rowCount * height + 50f);
        Collection_Landmark_Clicked(0);
    }

    public void Collection_Pigi_Clicked(int idx) {
        string ID = dataManager.PigiItems[idx].ID;
        PlayerPrefs.SetInt(ID + "_checked", 1);

        foreach(GameObject obj in Pigi_Row_objects) {
            obj.GetComponent<collection_row>().ResetItems();
        }

        info_img.sprite = dataManager.PigiItems[idx].Img;
        info_img.color = PlayerPrefs.GetInt(ID + "_count") > 0 ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 0.7f);

        info_title.text = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "title_" + ID);
        info_descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("Pigi", "descr_" + ID);
        info_count.text = "획득한 피지 숫자 : " + PlayerPrefs.GetInt(ID + "_count") + "개";
    }

    public void Collection_Landmark_Clicked(int idx)
    {
        string ID = dataManager.LandmarkItems[idx].ID;
        if(PlayerPrefs.GetInt(ID + "_count") > 0) PlayerPrefs.SetInt(ID + "_checked", 1);

        foreach (GameObject obj in Landmark_Row_objects)
        {
            obj.GetComponent<collection_row>().ResetItems();
        }

        info_img.sprite = dataManager.LandmarkItems[idx].Img;
        info_img.color = PlayerPrefs.GetInt(ID + "_count") > 0 ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 0.7f);

        info_title.text = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_title");
        info_descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("Landmark", ID + "_descr");
        info_count.text = "";
    }
}
