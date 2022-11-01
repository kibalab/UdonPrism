using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using VRC.SDK3.Components;
using VRC.Udon;

namespace Prism.Setup
{
    public class PrismSetup : MonoBehaviour
    {
        public float Size = 1;
        public GameObject InputField;
        public VRCUrlInputField c_InputField;
        public Camera CaptureCam;
        public float OriginSize = 2.813095f;
        public string StartUrl = "http://localhost:8080/";
        public bool CraditOpen;
        public GameObject EventPool;
        public UdonSharpBehaviour map;
        public string[] endPointCommand;
        [SerializeField] public UdonBehaviour[] events;
    }   
}
