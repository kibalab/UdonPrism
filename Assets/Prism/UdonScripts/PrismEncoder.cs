
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class PrismEncoder : UdonSharpBehaviour
    {
        public DataMap DataMap;
        public string StringToBase64(string data)
        {
            string result = "";
            
            char[] dataCharArray = data.ToCharArray();

            for (int i = 0; i < dataCharArray.Length / 3; i++)
            {
                result += (char) (dataCharArray[i * 3] >> 2) & 0x3F;
                result += (char) (((dataCharArray[i * 3] & 3) << 4) | (dataCharArray[i * 3 + 1] >> 4)) & 0x3F;
                result += (char) (((dataCharArray[i * 3 + 1] & 15) << 2) | (dataCharArray[i * 3 + 2] >> 6)) & 0x3F;
                result += (char) (dataCharArray[i * 3 + 2] & 0x3F);
            }

            if (dataCharArray.Length % 3 == 1)
            {
                result += (char) (dataCharArray[dataCharArray.Length - 1] >> 2) & 0x3F;
                result += (char) ((dataCharArray[dataCharArray.Length - 1] & 3) << 4) & 0x3F;
            }
            else if (dataCharArray.Length % 3 == 2)
            {
                result += (char) (dataCharArray[dataCharArray.Length - 2] >> 2) & 0x3F;
                result += (char) (((dataCharArray[dataCharArray.Length - 2] & 3) << 4) | (dataCharArray[dataCharArray.Length - 1] >> 4)) & 0x3F;
                result += (char) ((dataCharArray[dataCharArray.Length - 1] & 15) << 2) & 0x3F;
            }

            return result;
        }
    }
}
