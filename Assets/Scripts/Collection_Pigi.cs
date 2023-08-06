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
    private List<GameObject> Row_objects;

    public int height = 180;

    const int ItemsPerRow = 4;
    public bool initialized = false;

    [Button]
    public async void InitializePigiCollection() {
        initialized = true;

        Pigi_objects = new List<GameObject>();
        Row_objects = new List<GameObject>();

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
            Row_objects.Add(row);

            await row.GetComponent<collection_row>().InitializeRow(idx, count);
            await Task.Delay(5);
        }
        contents_holder.GetComponent<RectTransform>().sizeDelta = new Vector2(contents_holder.GetComponent<RectTransform>().sizeDelta.x, rowCount * height);
        Collection_Pigi_Clicked(0);
    }

    void Start()
    {
        if(!initialized) InitializePigiCollection();    
    }

    public void Collection_Pigi_Clicked(int idx) {
        print(idx);
        
        foreach(GameObject obj in Row_objects) {
            obj.GetComponent<collection_row>().ResetItems();
        }
        Pigi_objects[idx].GetComponent<Image>().color = Color.white;

        info_img.sprite = dataManager.PigiItems[idx].Img;
        string ID = dataManager.PigiItems[idx].ID;
        info_title.text = LocalizationSettings.StringDatabase.GetLocalizedString("pigi", "title_"+ID);
        info_descr.text = LocalizationSettings.StringDatabase.GetLocalizedString("pigi", "descr_"+ID);
        info_count.text = "획득한 피지 숫자 : " + PlayerPrefs.GetInt(ID + "_count") + "개";

    }
}
