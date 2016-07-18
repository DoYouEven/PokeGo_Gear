

using System.Text.RegularExpressions;
public class Base32Utils {

    /* number of bits per base 32 character */
    public  static int BITS_PER_BASE32_CHAR = 5;

    private static string BASE32_CHARS = "0123456789bcdefghjkmnpqrstuvwxyz";

    private Base32Utils() {}

    public static char valueToBase32Char(int value) {
        if (value < 0 || value >= BASE32_CHARS.Length) {
           // throw new IllegalArgumentException("Not a valid base32 value: " + value);
        }
        return BASE32_CHARS[value];
    }

    public static int base32CharToValue(char base32Char) {
        int value = BASE32_CHARS.IndexOf(base32Char, 0);
     
   
        if (value == -1) {
            return -1;
            //throw new IllegalArgumentException("Not a valid base32 char: " + base32Char);
        } else {
            return value;
        }
    }

    public static bool isValidBase32String(string text) {

       return  Regex.Match(text, "^[" + BASE32_CHARS + "]*$").Success;
        
    }
}