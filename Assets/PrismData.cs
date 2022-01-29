
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PrismData : UdonSharpBehaviour
{
    string m_data;
    bool[] m_rawData;

    public string Data
    {
        get => m_data;
    }

    public bool[] RawData
    {
        get => m_rawData;
    }
}
