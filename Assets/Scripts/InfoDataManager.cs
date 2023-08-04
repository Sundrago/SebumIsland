using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Localization.Settings;

public class InfoDataManager : MonoBehaviour
{
    [TableList(ShowIndexLabels = true, ShowPaging = true)]
    public List<PigiItem> PigiItems = new List<PigiItem>();


    [Serializable]
    public class PigiItem 
    {
        [TableColumnWidth(57, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite Img;

        [InlineButton("LoadData", "load")]
        [TextArea]
        public string ID;

        [ReadOnly]
        [VerticalGroup("Localized Info"), LabelWidth(30)]
        public string Name, Descr;

#if UNITY_EDITOR // Editor-related code must be excluded from builds
        [OnInspectorInit]
        private void LoadData() {
            Name = LocalizationSettings.StringDatabase.GetLocalizedString("pigi", "title_"+ID);
            Descr = LocalizationSettings.StringDatabase.GetLocalizedString("pigi", "descr_"+ID);
        }
#endif
    }
}