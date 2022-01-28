
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

public class VideoObserver : UdonSharpBehaviour
{
    public Material screen;

    public Camera cam;

    public MeshRenderer SubScreen;

    public Texture2D pool;

    public string texName = "_MainTex";

    public Text test;

    public int i = 0;
    public int j = 0;

    public void Update()
    {
        /*
        var tex = screen.mainTexture;
        

        if (tex == null)
        {
            test.text = $"Stand by";
            return; // Screen is Empty (NULL)
        }

        if(tex.GetType() == typeof(RenderTexture))
        {
            return;
        }

        Color pixelBuffer = ((Texture2D)tex).GetPixel(i, j);
        pool.SetPixel(i, j, pixelBuffer);
        pool.Apply();

        var pixel = pool.GetPixel(i, j);


        SubScreen.material.SetTexture("_MainTex", ((Texture2D)tex));

        test.text = $"X : {i}, Y : {j}, Color : {pixel}, W : {((Texture2D)tex).width}, H : {((Texture2D)tex).height}";

        i++;
        if (i > tex.width)
        {
            i = 0;
            j++;
            if(j > tex.height)
            {
                j = 0;
            }
        }
        */
    }

    private void OnPostRender()
    {
        var renT = cam.targetTexture;

        pool.ReadPixels(new Rect(0, 0, renT.width, renT.height), 0, 0);
        pool.Apply();

        Debug.Log("OnPostRender()");
    }
}
