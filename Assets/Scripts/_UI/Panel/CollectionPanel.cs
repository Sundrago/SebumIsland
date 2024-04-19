using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
///     Represents a panel that displays a collection of items.
/// </summary>
public class CollectionPanel : PanelObject
{
    [FormerlySerializedAs("itemCollection")] [FormerlySerializedAs("collection_pigi")] [SerializeField]
    private ItemCollectionUI itemCollectionUI;

    [FormerlySerializedAs("itemCollectionLandmark")] [FormerlySerializedAs("collection_Landmark")] [SerializeField]
    private ItemCollectionUI itemCollectionUILandmark;

    [SerializeField] private Button pigi_tab, landmark_tab;

    private void Start()
    {
        ShowPigiButtonClick();
    }

    protected override void OnPanelOpen()
    {
        if (!itemCollectionUI.PigiInitialized) itemCollectionUI.InitializePigiCollection();
        base.OnPanelOpen();
    }

    public void ShowPigiButtonClick()
    {
        if (!itemCollectionUI.PigiInitialized) itemCollectionUI.InitializePigiCollection();
        itemCollectionUI.gameObject.SetActive(true);
        itemCollectionUILandmark.gameObject.SetActive(false);

        pigi_tab.interactable = false;
        landmark_tab.interactable = true;
    }

    public void ShowLandmarkButtonClick()
    {
        if (!itemCollectionUI.LandmarkInitialized) itemCollectionUILandmark.InitializeLandmarkCollection();
        itemCollectionUI.gameObject.SetActive(false);
        itemCollectionUILandmark.gameObject.SetActive(true);

        pigi_tab.interactable = true;
        landmark_tab.interactable = false;
    }
}