using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Manages and animate 2D skybox.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Skybox2D_Manager : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Sprite[] bg_sprites;
    private GameObject child;

    private int debugInt;
    private RectTransform rect;

    private void Start()
    {
        InitiateRectTrandform();
        CreateChild();
        SetChildPosition();
    }

    private void Update()
    {
        rect.anchoredPosition = new Vector2(
            rect.anchoredPosition.x + scrollSpeed * Time.deltaTime,
            rect.anchoredPosition.y
        );

        if (rect.anchoredPosition.x > rect.sizeDelta.x)
            rect.anchoredPosition = new Vector2(
                rect.anchoredPosition.x - rect.sizeDelta.x,
                rect.anchoredPosition.y
            );
    }

    private void InitiateRectTrandform()
    {
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.height, Screen.height);
        GetComponent<Image>().sprite = bg_sprites[GetBgIdx()];
    }

    private void SetChildPosition()
    {
        var childPos = child.GetComponent<RectTransform>().anchoredPosition;
        childPos.x -= rect.sizeDelta.x;
        child.GetComponent<RectTransform>().anchoredPosition = childPos;
    }

    private void CreateChild()
    {
        child = Instantiate(gameObject, gameObject.transform);
        Destroy(child.GetComponent<Skybox2D_Manager>());
        child.transform.position = gameObject.transform.position;
    }

    private int GetBgIdx()
    {
        var time = DateTime.Now;
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