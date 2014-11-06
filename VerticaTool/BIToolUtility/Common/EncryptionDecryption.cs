using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Common
{
    public class EncryptionDecryption
    {
        public static string Encode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        public static string Decode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
    }
}
