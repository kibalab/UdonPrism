
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class DataMap : UdonSharpBehaviour
    {
        public VRCUrl[] endPointBaseMap;
        [SerializeField] public VRCUrl[] endPointCommandMap;
    }
}
