
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DataReader : UdonSharpBehaviour
{

    struct Line
    {
        Color[] line;
    }

    public Texture2D DataScreen;

    public void Update()
    {

        ReadPixels(DataScreen.GetPixels(0,0, DataScreen.width, DataScreen.height));
    }

    public bool[] FlipY(bool[] lines)
    {
        for(var i = 1; i<= 36; i++)
        {
            for (var j = 0; j< 128; j++)
            {
                //var tmp = lines[i * j + ];
            }
        }

        return lines;
    }

    public bool[] ReadPixels(Color[] ScreenPixels)
    {
        var pixelArray = new bool[9216];
        /*
        var ArrayCount = 0;

        var jumpLength = 6400;
        for (var i = 0; i< 9216; i++)
        {
            pixelArray[i] = ScreenPixels[i * 5 + jumpLength].r > 0.5f  ? true : false;

            if(i % 128 == 0) jumpLength+= 12800;
        }

        return test;
        */

        for (var i = 0; i < 9216; i++)
        {
            pixelArray[i] = ScreenPixels[6405 + ((i / 128) * 12800) + ((i % 128) * 10)].r > 0.4f;
        }


        string test = "";
        foreach (var pixel in pixelArray)
        {
            test += pixel ? "1" : "0";
        }
        Debug.Log(test);


        return pixelArray;
    }
}
