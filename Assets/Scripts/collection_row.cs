using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class collection_row : MonoBehaviour
{
    [SerializeField] List<GameObject> items = new List<GameObject>();
    [SerializeField] InfoDataManager dataManager;
    [SerializeField] Collection_Pigi collection_pigi;

    public async Task InitializeRow(int idx, int count) {
        for(int i = 0; i<4; i++) {
            if(i>=count) {
                items[i].SetActive(false);
                continue;
            }
            //Null exception
            if(dataManager.PigiItems[idx+i].Img == null) continue;

            int btnIdx = idx + i;
            items[i].GetComponent<Image>().sprite = dataManager.PigiItems[btnIdx].Img;
            items[i].GetComponent<Button>().onClick.AddListener(()=>collection_pigi.Collection_Pigi_Clicked(btnIdx));

            collection_pigi.Pigi_objects.Add(items[i]);
        }
        ResetItems();
        await Task.Yield();
    }

    public void BtnClicked(int idx) {
        print("btnclicked " + idx);
    }

    public async void ResetItems() {
        foreach(GameObject item in items) {
            item.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        await Task.Yield();
    }
}
