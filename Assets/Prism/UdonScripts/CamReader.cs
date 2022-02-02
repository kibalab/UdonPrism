
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CamReader : UdonSharpBehaviour
{
    private Camera Reader;
    private RenderTexture CamTex;
    public Texture2D Render;

    public DataReader DataReader;

    public float RenderFreq = 1.0f;

    void Start()
    {
        Reader = gameObject.GetComponent<Camera>();
        CamTex = Reader.targetTexture;
    }

    private void OnPostRender()
    {
        
        if (RenderFreq >= 0)
        {
            RenderFreq -= Time.deltaTime;
            return;
        }
        else
        {
            RenderFreq = 1.0f;
        }

        Render.ReadPixels(new Rect(0, 0, CamTex.width, CamTex.height), 0, 0);
        Render.Apply();
        DataReader.Read();
    }
}
