using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class collection_row : MonoBehaviour
{
    [SerializeField] List<GameObject> items = new List<GameObject>();
    [SerializeField] List<Image> new_icons = new List<Image>();
    [SerializeField] InfoDataManager dataManager;
    [SerializeField] Collection_Pigi collection_pigi;

    private int current_idx, current_count;
    private bool isPigi;

    public async Task InitializePigiRow(int idx, int count) {

        current_idx = idx;
        current_count = count;
        isPigi = true;

        for(int i = 0; i<4; i++) {
            if(i>=count) {
                items[i].SetActive(false);
                continue;
            }

            int btnIdx = idx + i;

            //Null exception
            if(dataManager.PigiItems[btnIdx].Img == null) continue;

            if (!collection_pigi.Pigi_objects.Contains(items[i]))
            {
                collection_pigi.Pigi_objects.Add(items[i]);
                items[i].GetComponent<Button>().onClick.AddListener(() => collection_pigi.Collection_Pigi_Clicked(btnIdx));
                items[i].GetComponent<Image>().sprite = dataManager.PigiItems[btnIdx].Img;
            }

            string ID = dataManager.PigiItems[btnIdx].ID;
            bool known = PlayerPrefs.GetInt(ID + "_count") > 0 ? true : false;
            bool isChecked = PlayerPrefs.GetInt(ID + "_checked") > 0 ? true : false;

            //unknown
            if (!known)
            {
                items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
                new_icons[i].gameObject.SetActive(false);
            }
            else if (!isChecked)
            {
                //known not checked
                items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
                new_icons[i].gameObject.SetActive(true);
            }
            else
            {
                //known and checked
                items[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                new_icons[i].gameObject.SetActive(false);
            }
        }
        
        await Task.Yield();
    }

    public async Task InitializeLandmarkRow(int idx, int count)
    {
        current_idx = idx;
        current_count = count;
        isPigi = false;

        for (int i = 0; i < 4; i++)
        {
            if (i >= count)
            {
                items[i].SetActive(false);
                continue;
            }
            int btnIdx = idx + i;
            //Null exception
            if (dataManager.PigiItems[btnIdx].Img == null) continue;

            if (!collection_pigi.Landmark_objects.Contains(items[i]))
            {
                collection_pigi.Landmark_objects.Add(items[i]);
                items[i].GetComponent<Button>().onClick.AddListener(() => collection_pigi.Collection_Landmark_Clicked(btnIdx));
                items[i].GetComponent<Image>().sprite = dataManager.LandmarkItems[btnIdx].Img;
            }

            string ID = dataManager.LandmarkItems[btnIdx].ID;
            bool known = PlayerPrefs.GetInt(ID + "_count") > 0 ? true : false;
            bool isChecked = PlayerPrefs.GetInt(ID + "_checked") > 0 ? true : false;

            //unknown
            if (!known)
            {
                items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
                new_icons[i].gameObject.SetActive(false);
            }
            else if (!isChecked)
            {
                //known not checked
                items[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
                new_icons[i].gameObject.SetActive(true);
            }
            else
            {
                //known and checked
                items[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                new_icons[i].gameObject.SetActive(false);
            }
        }
        await Task.Yield();
    }

    public async void ResetItems()
    {
        if(isPigi)
        {
            await InitializePigiRow(current_idx, current_count);
        } else
        {
            await InitializeLandmarkRow(current_idx, current_count);
        }
        await Task.Yield();
    }
}
