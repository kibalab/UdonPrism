
using Prism;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PrismVideoController : UdonSharpBehaviour
{
    public VRCUnityVideoPlayer player;

    public VRCUrlInputField stateField;

    public GameObject dataMap;

    DataMap map;

    uint RequestIndex = 0;

    VRCUrl[] query;
    VRCUrl currentUrl;

    private void Start()
    {
        map = dataMap.GetComponent<DataMap>();
    }

    public void SendData(VRCUrl[] data)
    {
        query = data;
        RequestIndex = 0;

        Debug.Log("[PrismVideoController] SendData");
        PlayVideo(query[RequestIndex++]);
    }

    public void Update()
    {
        if (query == null)
            return;
        
        if (currentUrl != null && RequestIndex != 0)
            return;

        if (RequestIndex == query.Length)
        {
            query = null;
            RequestIndex = 0;
            return;
        }

        PlayVideo(query[RequestIndex++]);
    }

    void PlayVideo(VRCUrl url)
    {
        StopVideo();
        currentUrl = url;
        player.LoadURL(url);
        Debug.Log("[PrismVideoController] PlayVideo " + url.Get());
    }

    public override void OnVideoReady()
    {
        player.Play();
    }

    public override void OnVideoStart()
    {
        player.SetTime(0f);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AllowRender");
    }

    public override void OnVideoEnd()
    {
        Debug.Log("[PrismVideoController] OnVideoEnd");
        StopVideo();
    }

    void StopVideo()
    {
        player.Stop();
        currentUrl = null;
    }
}
