using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Skybox2D_Manager : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] Sprite[] bg_sprites;
    private GameObject child;
    private RectTransform rect;

    int debugInt = 0;
    /*
     * 0 ~ 5 : midnight
     * 5 ~ 9 : evening
     * 9 ~ 18 : midday
     * 18 ~ 19  : daybreak
     * 19 ~ 22 : sunset
     * 22 ~ 24 : midnight
     */

    void Start()
    {
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.height, Screen.height);
        GetComponent<Image>().sprite = bg_sprites[GetBgIdx()];

        //create child
        child = Instantiate(gameObject, gameObject.transform);
        Destroy(child.GetComponent<Skybox2D_Manager>());
        child.transform.position = gameObject.transform.position;

        //set child pos
        Vector2 childPos = child.GetComponent<RectTransform>().anchoredPosition;
        childPos.x -= rect.sizeDelta.x;
        child.GetComponent<RectTransform>().anchoredPosition = childPos;
    }

    void Update()
    {
        rect.anchoredPosition = new Vector2(
            rect.anchoredPosition.x + scrollSpeed * Time.deltaTime,
            rect.anchoredPosition.y
        );

        if (rect.anchoredPosition.x > rect.sizeDelta.x)
        {
            rect.anchoredPosition = new Vector2(
                rect.anchoredPosition.x - rect.sizeDelta.x,
                rect.anchoredPosition.y
            );
        }
    }

    private int GetBgIdx()
    {
        System.DateTime time = System.DateTime.Now;
        if (time.Hour < 5) return 0;
        if (time.Hour < 9) return 1;
        if (time.Hour < 18) return 2;
        if (time.Hour < 19) return 3;
        if (time.Hour < 22) return 4;
        return 0;
    }

    public void DebugChangeMood()
    {
        gameObject.GetComponent<Image>().sprite = bg_sprites[debugInt];
        child.GetComponent<Image>().sprite = bg_sprites[debugInt];

        debugInt += 1;
        if (debugInt >= bg_sprites.Length) debugInt = 0;
    }
}
