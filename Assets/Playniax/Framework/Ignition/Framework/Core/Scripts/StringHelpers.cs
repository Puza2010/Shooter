namespace Playniax.Ignition
{
    // Collection of string functions.
    public class StringHelpers
    {
        // Obsolete! replace with str.PadLeft
        public static string Format(string str, int chars)
        {
            int length = chars - str.Length;

            for (int i = 0; i < length; i++)
                str = "0" + str;

            return str;
        }
    }
}