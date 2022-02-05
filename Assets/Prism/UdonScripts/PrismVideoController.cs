
using Prism;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PrismVideoController : UdonSharpBehaviour
{
    public bool isSending = false;

    public float req_freq = 1.0f;

    public VRCUnityVideoPlayer player;

    public VRCUrlInputField stateField;

    public GameObject dataMap;

    DataMap map;

    uint RequestIndex = 0;

    VRCUrl[] urls;

    float m_req_freq;

    private void Start()
    {
        map = dataMap.GetComponent<DataMap>();
    }


    /*
    public override void OnVideoEnd()
    {
        if (urls.Length == RequestIndex)
        {
            isSending = false;

            stateField.SetUrl(map.endPointCommandMap[0]); // End Command
            player.PlayURL(map.endPointCommandMap[0]);
            Debug.Log($"[Prism Player Controller] End Request");
        }
        else
        {
            stateField.SetUrl(urls[++RequestIndex]);
            player.PlayURL(urls[RequestIndex]);
            Debug.Log($"[Prism Player Controller] Request URL : {urls[RequestIndex].Get()}");
        }
    }
    */

    public void SendData(VRCUrl[] data)
    {
        urls = data;

        RequestIndex = 0;

        isSending = true;

        m_req_freq = req_freq;


        /*
        stateField.SetUrl(urls[RequestIndex]);
        player.PlayURL(urls[RequestIndex]);
        Debug.Log($"[Prism Player Controller] Request URL : {urls[RequestIndex].Get()}");
        */
    }

    public void Update()
    {
        if (!isSending) return;

        m_req_freq -= Time.deltaTime;

        if (m_req_freq > 0) return;

        VRCUrl url;

        if (urls.Length == RequestIndex)
        {
            isSending = false;

            url = map.endPointCommandMap[0]; // End Command
        }
        else
        {
            url = urls[RequestIndex];
        }

        if(stateField != null)
        {
            stateField.SetUrl(url);
        }

        Debug.Log($"[Prism Player Controller] Request URL : {url.Get()}");

        player.LoadURL(url);

        if (url.Get().EndsWith("end"))
        {
            isSending = false;
            Debug.Log($"[Prism Player Controller] End Request");
        }

        RequestIndex++;

        m_req_freq = req_freq;
    }
}
