namespace Utility {
    
    static class Converter {
        public static int StringToInt(string value) {
            int number;
            if (int.TryParse(value, out number))
                return number;
            else
                return 0;
        }
    }
 
}
