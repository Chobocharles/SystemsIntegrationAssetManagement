using System;
using System.Text;

namespace Asset_Management.Models.Strings
{
    public static class Base64Model
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="outString"></param>
        /// <returns></returns>
        public static bool ToBase64(string text, Encoding encoding, out string outString)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            try
            {
                var textAsBytes = encoding.GetBytes(text);
                outString = Convert.ToBase64String(textAsBytes);
                return true;
            }
            catch
            {
                outString = "";
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="outString"></param>
        /// <returns></returns>
        public static bool TryParseBase64(string text, Encoding encoding, out string outString)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            try
            {
                var textAsBytes = Convert.FromBase64String(text);
                outString = encoding.GetString(textAsBytes);
                return true;
            }
            catch (FormatException)
            {
                outString = "";
                return false;
            }
        }
    }
}
