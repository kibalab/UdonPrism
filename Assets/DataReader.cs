
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
        data = FlipY(data);
        PrintLine(data);
        var stringdata = Decryption(data);
        Debug.Log(stringdata);
        console.text = stringdata;
    }

    public string Decryption(bool[] data)
    {
        var stringData = "";
        var chars = new int[288];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = 0;
            for (var j = 0; j < 32; j++)
            {
                if(data[i*32+j]) chars[i] |= (int)(1 << j);
            }

            stringData += (char)chars[i];
        }

        return stringData;
    }

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
        for (var i = 0; i < 36; i++)
        {
            for (var j = 0; j < 128; j++)
            {
                var tmp = lines[i * 128 + j];
                lines[i * 128 + j] = lines[(71 - i) * 128 + j];
                lines[(71 - i) * 128 + j] = tmp;
            }
        }

        return lines;
    }

    public bool[] ReadPixels()
    {
        var pixelArray = new bool[9216];

        for (var i = 0; i < 9216; i++)
            pixelArray[i] = DataScreen.GetPixel(5 + (i % 128) * 10, 5 + (i / 128) * 10).grayscale > 0.5f;

        return pixelArray;
    }
}
