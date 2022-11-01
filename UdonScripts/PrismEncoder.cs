
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class PrismEncoder : UdonSharpBehaviour
    {
        public DataMap DataMap;

        public VRCUrl[] StringToBase64Urls(string data)
        {
            int base64length = (data.Length + 2) / 3 * 8;

            VRCUrl[] result = new VRCUrl[base64length];
            char[] dataCharArray = data.ToCharArray();

            for (int i = 0; i < ((base64length / 8) - 1); i++)
            {
                result[i * 8] = DataMap.endPointBaseMap[dataCharArray[i * 3] >> 10 & 0x3F];
                result[i * 8 + 1] = DataMap.endPointBaseMap[dataCharArray[i * 3] >> 4 & 0x3F];
                result[i * 8 + 2] = DataMap.endPointBaseMap[(dataCharArray[i * 3] << 2 & 0x3C) | (dataCharArray[i * 3 + 1] >> 14 & 0x3)];
                result[i * 8 + 3] = DataMap.endPointBaseMap[dataCharArray[i * 3 + 1] >> 8 & 0x3F];
                result[i * 8 + 4] = DataMap.endPointBaseMap[dataCharArray[i * 3 + 1] >> 2 & 0x3F];
                result[i * 8 + 5] = DataMap.endPointBaseMap[(dataCharArray[i * 3 + 1] << 4 & 0x30) | (dataCharArray[i * 3 + 2] >> 12 & 0xF)];
                result[i * 8 + 6] = DataMap.endPointBaseMap[dataCharArray[i * 3 + 2] >> 6 & 0x3F];
                result[i * 8 + 7] = DataMap.endPointBaseMap[dataCharArray[i * 3 + 2] & 0x3F];
            }

            if (data.Length % 3 == 1)
            {
                result[base64length - 8] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] >> 10 & 0x3F];
                result[base64length - 7] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] >> 4 & 0x30];
                result[base64length - 6] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] << 2 & 0x3C];
                result[base64length - 5] = DataMap.endPointBaseMap[64];
                result[base64length - 4] = DataMap.endPointBaseMap[64];
                result[base64length - 3] = DataMap.endPointBaseMap[64];
                result[base64length - 2] = DataMap.endPointBaseMap[64];
                result[base64length - 1] = DataMap.endPointBaseMap[64];
            }
            else if (data.Length % 3 == 2)
            {
                result[base64length - 8] = DataMap.endPointBaseMap[dataCharArray[data.Length - 2] >> 10 & 0x3F];
                result[base64length - 7] = DataMap.endPointBaseMap[dataCharArray[data.Length - 2] >> 4 & 0x3F];
                result[base64length - 6] = DataMap.endPointBaseMap[(dataCharArray[data.Length - 2] << 2 & 0x3C) | (dataCharArray[data.Length - 1] >> 14 & 0x3)];
                result[base64length - 5] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] >> 8 & 0x3F];
                result[base64length - 4] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] >> 2 & 0x30];
                result[base64length - 3] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] << 4 & 0x30];
                result[base64length - 2] = DataMap.endPointBaseMap[64];
                result[base64length - 1] = DataMap.endPointBaseMap[64];
            }
            else
            {
                result[base64length - 8] = DataMap.endPointBaseMap[dataCharArray[data.Length - 3] >> 10 & 0x3F];
                result[base64length - 7] = DataMap.endPointBaseMap[dataCharArray[data.Length - 3] >> 4 & 0x3F];
                result[base64length - 6] = DataMap.endPointBaseMap[(dataCharArray[data.Length - 3] << 2 & 0x3C) | (dataCharArray[data.Length - 2] >> 14 & 0x3)];
                result[base64length - 5] = DataMap.endPointBaseMap[dataCharArray[data.Length - 2] >> 8 & 0x3F];
                result[base64length - 4] = DataMap.endPointBaseMap[dataCharArray[data.Length - 2] >> 2 & 0x3F];
                result[base64length - 3] = DataMap.endPointBaseMap[(dataCharArray[data.Length - 2] << 4 & 0x30) | (dataCharArray[data.Length - 1] >> 12 & 0xF)];
                result[base64length - 2] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] >> 6 & 0x3F];
                result[base64length - 1] = DataMap.endPointBaseMap[dataCharArray[data.Length - 1] & 0x3F];
            }

            return result;
        }
    }
}
