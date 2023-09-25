using UnityEditorInternal;
using UnityEngine;

namespace MyUtility {
    using UnityEngine.Localization.Settings;
    static class Converter {
        public static int StringToInt(string value) {
            int number;
            if (int.TryParse(value, out number))
                return number;
            else
                return 0;
        }
    }
    
    static class Localize {
        public static string GetLocalizedString(string input)
        {
            if (input.Contains('[') && input.Contains(']'))
            {
                string[] sliced = input.Split('[', ']');
                string key = sliced[input.IndexOf('[') + 1];
                return LocalizationSettings.StringDatabase.GetLocalizedString("UI", key);
            }
            
            Debug.Log("LocaleCodeNotFound for string : " + input);
            return input;
        }
    }
 
}
