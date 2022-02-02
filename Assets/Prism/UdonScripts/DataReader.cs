
using System;
using System.Collections;
using System.Text;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Prism
{
    public class DataReader : UdonSharpBehaviour
    {
        #region Field

        #region Public Field

        public Texture2D DataScreen;

        public Text Console;

        public EventManager EventBehaviour;

        #endregion Public Field

        #region Member Field

        string m_Data = "";
        string m_Error = "";

        // [ Indexes ]
        int CurrentFrameIndex = 1;
        int FrameCount = 1;
        int CurrentFrameBytes = 0;

        public int SectorDivision = 1;
        int CurrentSectorIndex = 0;
        int OneSectorSize = 0;


        // [ State ]
        bool IsSectorReading = false;
        bool IsEndFrameRead = false;
        bool IsReadable = false;

        #endregion Member Field

        #region Properties
        public string Data
        {
            get => m_Data;
        }
        public string Error
        {
            get => m_Error;
        }
        #endregion Properties

        #endregion Field


        public void Start()
        {
            OnPrismReady();
        }

        public void Read()
        {
            IsReadable = true;
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


        /* [ Header ]
         * Header[0] : 0 (Always)
         * Header[1] : Current Frame
         * Header[2] : 0 (Always)
         * Header[3] : All Frame Count
         * Header[4] : 0 (Always)
         * Header[5] : Current Frame Byte Size
         * Header[6] : 0 (Always)
         * Header[7] : 0 (Always)
         */

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

        private void OnPrismReady()
        {
            OneSectorSize = (36864 / SectorDivision);

            InvokeEvent(nameof(OnPrismReady));
        }

        private void OnPrismStart()
        {
            Debug.Log($"[PrismDataReader] Start Read | OneSectorSize {OneSectorSize}");

            CurrentFrameIndex = 0;
            FrameCount = 0;
            CurrentFrameBytes = 0;

            InvokeEvent(nameof(OnPrismStart));
        }

        private void OnPrismFrameChange(int[] header)
        {
            CurrentFrameIndex = header[1];
            FrameCount = header[3];
            CurrentFrameBytes = header[5];

            CurrentSectorIndex = 1;

            IsSectorReading = true;
            IsEndFrameRead = false; // Start Read Current Frame

            Debug.Log($"[PrismDataReader] OnFrameChanged | Current Frame {header[1]}");

            InvokeEvent(nameof(OnPrismFrameChange));
        }

        private void OnPrismFrameReadEnd()
        {
            IsSectorReading = false;
            IsEndFrameRead = true; // End Current Frame Read

            Debug.Log($"[PrismDataReader] Frame Read End");

            InvokeEvent(nameof(OnPrismFrameReadEnd));
        }

        private void OnPrismReadEnd()
        {
            Debug.Log($"[PrismDataReader] Read End");
            Debug.Log($"[PrismDataReader] Data : {m_Data}");

            IsReadable = false;

            InvokeEvent(nameof(OnPrismReadEnd));
        }

        private void OnReadEndException()
        {
            Debug.Log($"[PrismDataReader] Frame changed while reading");

            m_Data = "";

            IsSectorReading = false;
            IsReadable = false;

            OnPrismException("Frame changed while reading");
        }

        private void OnPrismException(string e)
        {
            m_Error = e;

            InvokeEvent(nameof(OnPrismException));
        }

        // IsEndFrameRead : Current Frame Read State [Bool]
        private void Update()
        {
            if (!IsReadable) return;

            #region Watcher

            var header = DecryptInt32(ReadSector(0));

            if (header[1] == 0) return;

            if (IsSectorReading && header[1] != CurrentFrameIndex)
            {
                OnReadEndException();
                return;
            }

            if (IsEndFrameRead && header[1] == CurrentFrameIndex) return;

            if (!IsSectorReading && header[1] == 1) OnPrismStart();

            if (header[1] > CurrentFrameIndex) OnPrismFrameChange(header);

            #endregion Watcher

            #region Read

            if (!IsSectorReading) return;

            var SectorPixelArray = ReadSector(CurrentSectorIndex++);
            var decryptedData = DecryptUnicode(SectorPixelArray);
            m_Data += decryptedData;

            #endregion Read

            #region End

            if (CurrentSectorIndex == SectorDivision || CurrentFrameBytes * 8 <= CurrentSectorIndex * OneSectorSize)
            {
                OnPrismFrameReadEnd();
            }

            if (CurrentFrameIndex == FrameCount && CurrentFrameBytes * 8 <= CurrentSectorIndex * OneSectorSize)
            {
                OnPrismReadEnd();
            }

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
                    IsSectorReading = false;
                    break;
                }

                var XIndex = 2 + (i % 256) * 5;
                var YIndex = 717 - (i / 256) * 5; /*2 + (i / 256) * 5*/

                var pixel = DataScreen.GetPixel(XIndex, YIndex);
                SectorPixelArray[i - firstPixel] = pixel.grayscale > 0.5f;
            }

            return SectorPixelArray;
        }


        private void InvokeEvent(string eventName)
        {
            EventBehaviour.Invoke(eventName);
        }
    }
}