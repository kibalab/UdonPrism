
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

    public int FrameCalculationDivision = 1;
    int divisionCalculationCount = 0;
    int OneSectorSize = 0;

    bool Reading = false;

    bool[] pixelArray = new bool[36864];

    public void Read()
    {
        if(!Reading) ReadScreen();
        else Debug.Log($"[PrismDataReader] Now Reading - Please try again later");
    }

    private string Decryption(bool[] data)
    {
        Debug.Log($"[PrismDataReader] Start Decryption");

        var stringData = "";
        var chars = new int[2304];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = 0;
            for (var j = 0; j < 16; j++)
            {
                if(data[i*16+j]) chars[i] |= (int)(1 << j);
            }

            if ((char)chars[i] == null) return stringData; // Null일 경우 이후 데이터는 비어있음으로 끝냄

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

    private void ReadScreen()
    {
        Reading = true;
        pixelArray = new bool[36864];
        OneSectorSize = (36864 / FrameCalculationDivision);
        Debug.Log($"[PrismDataReader] Start Read | OneSectorSize {OneSectorSize}");
    }

    private void OnReadEnd()
    {
        Reading = false;
        Debug.Log($"[PrismDataReader] End Read");
        var stringdata = Decryption(pixelArray);
        console.text = stringdata;
        Debug.Log(stringdata);
    }

    private void Update()
    {
        #region ReadPixels
        {
            if (Reading) return;

            var firstPixel = divisionCalculationCount * OneSectorSize;

            Debug.Log($"[PrismDataReader] Read Sector {firstPixel} ~ {firstPixel + OneSectorSize}");

            for (var i = firstPixel; i < firstPixel + OneSectorSize; i++)
            {

                var XIndex = 2 + (i % 256) * 5;
                var YIndex = 717 - (i / 256) * 5; /*2 + (i / 256) * 5*/

                var pixel = DataScreen.GetPixel(XIndex, YIndex);
                pixelArray[i] = pixel.r > 0.5f;

                if (i == pixelArray.Length - 1)
                {
                    OnReadEnd();
                    break;
                }
            }

            divisionCalculationCount++;
        }
        #endregion ReadPixels
    }
}
