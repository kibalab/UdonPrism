
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism {
    public class PrismInputField : UdonSharpBehaviour
    {
        public PrismVideoController controller;
        public GameObject encoder;
        PrismEncoder m_encoder;
        public InputField input;

        private void Start()
        {
            m_encoder = encoder.GetComponent<PrismEncoder>();

            if (!m_encoder)
            {
                Debug.Log("[PrismInputField] Can not load PrismEncoder");
            }
        }

        public void RequestServer()
        {
            var urls = m_encoder.StringToBase64Urls(input.text);
            controller.SendData(urls);
        }
    }
}
