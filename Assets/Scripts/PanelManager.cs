using UnityEngine;

/// <summary>
///     Manages the opening and closing of panels.
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private PanelObject collectionPanel, settingsManager, upgradePanel, pigiInfoPanel, buildPanel;
    public static PanelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CloseOtherPanels(GameObject panel)
    {
        TryToClosePanel(collectionPanel, panel);
        TryToClosePanel(settingsManager, panel);
        TryToClosePanel(upgradePanel, panel);
        TryToClosePanel(pigiInfoPanel, panel);
        TryToClosePanel(buildPanel, panel);
    }

    private void TryToClosePanel(PanelObject panelToClose, GameObject callingPanel)
    {
        if (panelToClose.gameObject != callingPanel)
            panelToClose.ClosePanel();
    }
}