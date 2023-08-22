using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField]
    CollectionPanelCtrl collectionPanelCtrl;
    [SerializeField]
    Settings settings;
    [SerializeField]
    NewUpgPanel newUpgPanel;
    [SerializeField]
    PigiInfoPanel pigiInfoPanel;
    [SerializeField]
    BuildPanelCtrl buildPanelCtrl;

    public static PanelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CloseOtherPanels(GameObject panel)
    {
        if(panel.GetComponent<CollectionPanelCtrl>() == null)
        {
            collectionPanelCtrl.Hide();
        }
        if (panel.GetComponent<Settings>() == null)
        {
            settings.Hide();
        }
        if (panel.GetComponent<NewUpgPanel>() == null)
        {
            newUpgPanel.ClosePanel();
        }
        if (panel.GetComponent<PigiInfoPanel>() == null)
        {
            pigiInfoPanel.ClosePanel();
        }
        if (panel.GetComponent<BuildPanelCtrl>() == null)
        {
            buildPanelCtrl.ClosePanel();
        }
    }
}
