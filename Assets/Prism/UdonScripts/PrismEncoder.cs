
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class PrismEncoder : UdonSharpBehaviour
    {
        public DataMap DataMap;
        public bool[] StringToBinary(string data)
        {
            var binary = new bool[data.Length * 8];

            var k = 0;

            for(var i = 0; i< data.Length; i++)
            {

                k++;
            }
        }
    }
}
