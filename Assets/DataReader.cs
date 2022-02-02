
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

    bool isReading = false;
    bool currentFrameRead = false;
    bool canRead = false;

    string data = "";

    int PreviousFrameIndex = 0;
    int FrameCount = 0;
    int CurrentFrameBytes = 0;

    public void Start()
    {
        OneSectorSize = (36864 / SectorDivision);
    }

    public void Read()
    {
        canRead = true;
    }

    private string DecryptUnicode(bool[] bitData)
    {
        //Debug.Log($"[PrismDataReader] Start Decryption");

        var stringData = "";
        for (var i = 0; i < bitData.Length / 16; i++)
        {
            int unicodeCharacter = 0;

            for (var j = 0; j < 16; j++)
                if (bitData[i * 16 + j]) unicodeCharacter |= (int)(1 << j);

            // this not working
            // if ((char)chars[i] == null) return stringData; // Null일 경우 이후 데이터는 비어있음으로 끝냄

            stringData += (char)unicodeCharacter;
        }

        return stringData;
    }

    private int[] DecryptInt32(bool[] bitData)
    {
        var intData = new int[bitData.Length / 32];
        for (var i = 0; i < intData.Length; i++)
        {
            intData[i] = 0;
            for (var j = 0; j < 32; j++)
            {
                if (bitData[i * 32 + j]) intData[i] |= (int)(1 << j);
            }
        }

        return intData;
    }

    public void PrintBoolArray(bool[] pixelArray)
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

    private void OnPrismStart()
    {     
        Debug.Log($"[PrismDataReader] Start Read | OneSectorSize {OneSectorSize}");
    }

    private void OnFrameChange(int[] header)
    {
        PreviousFrameIndex = header[1];
        FrameCount = header[3];
        CurrentFrameBytes = header[5];

        CurrentSectorIndex = 1;

        isReading = true;
        currentFrameRead = false;

        Debug.Log($"[PrismDataReader] Frame Read Start | Current Frame {header[1]}");
    }

    private void OnFrameReadEnd()
    {
        isReading = false;
        currentFrameRead = true;

        Debug.Log($"[PrismDataReader] Frame Read End");
    }

    private void OnReadEnd()
    {
        Debug.Log($"[PrismDataReader] Read End");
        Debug.Log($"[PrismDataReader] Data : {data}");

        PreviousFrameIndex = 0;
        FrameCount = 0;
        CurrentFrameBytes = 0;

        canRead = false;
    }

    private void OnReadEndException()
    {
        Debug.Log($"[PrismDataReader] Frame changed while reading");

        PreviousFrameIndex = 0;
        FrameCount = 0;
        CurrentFrameBytes = 0;

        data = "";

        isReading = false;
        canRead = false;
    }

    private void Update()
    {
        if (!canRead) return;

        #region Watcher
        var header = DecryptInt32(ReadSector(0));
        if (header[1] == 0) return;
        if (isReading && header[1] != PreviousFrameIndex)
        {
            OnReadEndException();
            return;
        }

        if (currentFrameRead && header[1] == PreviousFrameIndex) return;
        if (!isReading && header[1] == 1) OnPrismStart();
        if (header[1] > PreviousFrameIndex) OnFrameChange(header);
        #endregion Watcher

        #region Read
        if (!isReading) return;
        var SectorPixelArray = ReadSector(CurrentSectorIndex++);
        var decryptedData = DecryptUnicode(SectorPixelArray);
        data += decryptedData;
        #endregion Read

        #region End
        if (CurrentSectorIndex == SectorDivision || CurrentFrameBytes * 8 <= CurrentSectorIndex * OneSectorSize)
            OnFrameReadEnd();
        if (PreviousFrameIndex == FrameCount && CurrentFrameBytes * 8 <= CurrentSectorIndex * OneSectorSize)
            OnReadEnd();
        #endregion End
    }

    private bool[] ReadSector(int sectorIndex)
    {
        var firstPixel = sectorIndex * OneSectorSize;
        var SectorPixelArray = new bool[OneSectorSize];

        for (var i = firstPixel; i < firstPixel + OneSectorSize; i++)
        {
            if (sectorIndex >= SectorDivision)
            {
                isReading = false;
                break;
            }

            var XIndex = 2 + (i % 256) * 5;
            var YIndex = 717 - (i / 256) * 5; /*2 + (i / 256) * 5*/

            var pixel = DataScreen.GetPixel(XIndex, YIndex);
            SectorPixelArray[i - firstPixel] = pixel.grayscale > 0.5f;
        }

        return SectorPixelArray;
    }
}
