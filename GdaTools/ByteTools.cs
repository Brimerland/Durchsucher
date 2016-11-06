using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GdaTools
{
    public class ByteTools
    {
        public static String ToByteString(byte[] data)
        {
            String retValue = null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }
            retValue = sb.ToString();
            return retValue;
        }
    }
}
