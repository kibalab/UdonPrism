
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DataMap : UdonSharpBehaviour
{
    public VRCUrl[] endPointBaseMap;
    [SerializeField] public VRCUrl[] endPointCommandMap;
}
