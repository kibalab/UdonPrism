
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

    public Text Console;

    public int SectorDivision = 1;
    int CurrentSectorIndex = 0;
    int OneSectorSize = 0;

    bool Reading = false;

    string data = "";

    public void Read()
    {
        if(!Reading) ReadScreen();
        else Debug.Log($"[PrismDataReader] Now Reading - Please try again later");
    }

    private string Decryption(bool[] data)
    {
        //Debug.Log($"[PrismDataReader] Start Decryption");

        var stringData = "";
        var chars = new int[data.Length / 16];
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

    public void PrintLine(bool[] pixelArray)
    {
        string test = "";
        foreach (var pixel in pixelArray)
        {
            test += pixel ? "1" : "0";
        }
        Debug.Log(test);
    }
    /*
    

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

        ReaderInit();

        OneSectorSize = (36864 / SectorDivision);
        Debug.Log($"[PrismDataReader] Start Read | OneSectorSize {OneSectorSize}");
    }

    public void ReaderInit()
    {
        CurrentSectorIndex = 0;
        data = "";
        //Console.text = "";
    }

    private void OnReadEnd()
    {
        Reading = false;
        Debug.Log($"[PrismDataReader] End Read");
        Debug.Log(data);
    }

    private void Update()
    {
        #region ReadInit
        var SectorPixelArray = new bool[OneSectorSize];
        #endregion ReadInit

        #region ReadPixels
        {
            if (!Reading) return;

            var firstPixel = CurrentSectorIndex * OneSectorSize;

            //Debug.Log($"[PrismDataReader] Read Sector {firstPixel} ~ {firstPixel + OneSectorSize}");


            for (var i = firstPixel; i < firstPixel + OneSectorSize; i++)
            {
                if (CurrentSectorIndex >= SectorDivision)
                {
                    Reading = false;
                    break;
                }

                var XIndex = 2 + (i % 256) * 5;
                var YIndex = 717 - (i / 256) * 5; /*2 + (i / 256) * 5*/

                var pixel = DataScreen.GetPixel(XIndex, YIndex);
                SectorPixelArray[i - firstPixel] = pixel.r > 0.5f;
            }


            CurrentSectorIndex++;
        }
        #endregion ReadPixels

        #region Sector Decryption
        {
            //PrintLine(SectorPixelArray);
            var stringdata = Decryption(SectorPixelArray);
            data += stringdata;
            //Console.text += stringdata;

            if (!Reading) OnReadEnd();
        }
        #endregion
    }
}
