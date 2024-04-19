using UnityEngine;
using UnityEngine.Localization.Settings;

namespace MyUtility
{
    /// <summary>
    ///     Provides methods for converting values.
    /// </summary>
    internal static class Converter
    {
        public static int StringToInt(string value)
        {
            int number;
            if (int.TryParse(value, out number))
                return number;
            return 0;
        }
    }

    /// <summary>
    ///     Provides methods for localizing strings.
    /// </summary>
    internal static class Localize
    {
        public static string GetLocalizedString(string input)
        {
            if (input.Contains('[') && input.Contains(']'))
            {
                var sliced = input.Split('[', ']');
                var key = sliced[input.IndexOf('[') + 1];
                return LocalizationSettings.StringDatabase.GetLocalizedString("UI", key);
            }

            Debug.Log("LocaleCodeNotFound for string : " + input);
            return input;
        }
    }
}