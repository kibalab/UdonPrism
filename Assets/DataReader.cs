
using System;
using System.Collections;
using System.Text;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class DataReader : UdonSharpBehaviour
{

    public Texture2D DataScreen;

    public Text console;

    public void Read()
    {
        var data = ReadPixels();
        //data = FlipY(data);
        //PrintLine(data);
        var stringdata = Decryption(data);
        //Debug.Log(stringdata);
        console.text = stringdata;
        Debug.Log(stringdata);
    }

    public string Decryption(bool[] data)
    {
        var stringData = "";
        var chars = new int[2304];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = 0;
            for (var j = 0; j < 16; j++)
            {
                if(data[i*16+j]) chars[i] |= (int)(1 << j);
            }

            stringData += (char)chars[i];
        }

        return stringData;
    }

    /*
    public void PrintLine(bool[] pixelArray)
    {
        string test = "";
        foreach (var pixel in pixelArray)
        {
            test += pixel ? "1" : "0";
        }
        Debug.Log(test);
    }

    public bool[] FlipY(bool[] lines)
    {
        for (var i = 0; i < 72; i++)
        {
            for (var j = 0; j < 256; j++)
            {
                var tmp = lines[i * 256 + j];
                lines[i * 256 + j] = lines[(143 - i) * 256 + j];
                lines[(143 - i) * 256 + j] = tmp;
            }
        }

        return lines;
    }*/

    public bool[] ReadPixels()
    {
        var pixelArray = new bool[36864];

        for (var i = 0; i < 36864; i++)
        {

            var XIndex = 2 + (i % 256) * 5;
            var YIndex = 717 - (i / 256) * 5; /*2 + (i / 256) * 5*/

            var pixel = DataScreen.GetPixel(XIndex, YIndex);
            pixelArray[i] = pixel.r > 0.5f;
        }

        return pixelArray;
    }
}
