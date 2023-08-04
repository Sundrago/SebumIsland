using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public void ToggleSettingsPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
