using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ACR128
{
    class ACR120U
    {
        //=============================== Error Code ===============================

        public const int ERR_ACR120_INTERNAL_UNEXPECTED = -1000;
        public const int ERR_ACR120_PORT_INVALID = -2000;
        public const int ERR_ACR120_PORT_OCCUPIED = -2010;
        public const int ERR_ACR120_HANDLE_INVALID = -2020;
        public const int ERR_ACR120_INCORRECT_PARAM = -2030;
        public const int ERR_ACR120_READER_NO_TAG = -3000;
        public const int ERR_ACR120_READER_READ_FAIL_AFTER_OP = -3010;
        public const int ERR_ACR120_READER_NO_VALUE_BLOCK = -3020;
        public const int ERR_ACR120_READER_OP_FAILURE = -3030;
        public const int ERR_ACR120_READER_UNKNOWN = -3040;
        public const int ERR_ACR120_READER_LOGIN_INVALID_STORED_KEY_FORMAT = -4010;
        public const int ERR_ACR120_READER_WRITE_READ_AFTER_WRITE_ERROR = -4020;
        public const int ERR_ACR120_READER_DEC_FAILURE_EMPTY = -4030;
        public const int ERR_READER_VALUE_INC_OVERFLOW = -4031;
        public const int ERR_READER_VALUE_OP_FAILURE = -4032;
        public const int ERR_READER_VALUE_INVALID_BLOCK = -4033;
        public const int ERR_READER_VALUE_ACCESS_FAILURE = -4034;
        public const int ERR_READER_NO_RESPONSE = -5000;

        //======================= Reader Port for AC_Open ==========================

        public const int ACR120_USB1 = 0;
        public const int ACR120_USB2 = 1;
        public const int ACR120_USB3 = 2;
        public const int ACR120_USB4 = 3;
        public const int ACR120_USB5 = 4;
        public const int ACR120_USB6 = 5;
        public const int ACR120_USB7 = 6;
        public const int ACR120_USB8 = 7;

        //======================== Key Type for AC_Login ===========================

        public const int ACR120_LOGIN_KEYTYPE_A = (int)(0xAA);

        public const int ACR120_LOGIN_KEYTYPE_B = (int)(0xBB);
        public const int ACR120_LOGIN_KEYTYPE_DEFAULT_A = (int)(0xAD);
        public const int ACR120_LOGIN_KEYTYPE_DEFAULT_B = (int)(0xBD);
        public const int ACR120_LOGIN_KEYTYPE_DEFAULT_F = (int)(0xFD);
        public const int ACR120_LOGIN_KEYTYPE_STORED_A = (int)(0xAF);
        public const int ACR120_LOGIN_KEYTYPE_STORED_B = (int)(0xBF);

        //======================== Key Type for AC_Login ===========================
        [StructLayout(LayoutKind.Sequential)]
        public struct tReaderStatus
        {
            public byte MifareInterfaceType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] CardsSupported;
            public byte CardOpMode;
            public byte FWI;		// the current FWI value (time out value)
            public byte RFU;		// To be defined
            public int RFU2;		// To be defined
        };

        //======================================================================================
        // READER COMMANDS
        //======================================================================================


        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Open(int ReaderPort);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Close(int hReader);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Reset(int hReader);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Status(int hReader, ref byte pFirmwareVersion, ref tReaderStatus pReaderStatus);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_ReadRC531Reg(int hReader, byte RegNo, ref byte pRegData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_WriteRC531Reg(int hReader, byte RegNo, byte pRegData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_DirectSend(int hReader, byte DataLength, ref byte pData,
                                                   ref byte pResponseDataLength, ref byte pResponseData, int TimedOut);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_DirectReceive(int hReader, byte RespectedDataLength, ref byte pReceivedDataLength,
                                                      ref byte pReceivedData, int TimedOut);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_RequestDLLVersion(ref byte pVersionInfoLen, ref byte pVersionInfo);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_ReadEEPROM(int hReader, byte RegNo, ref byte pEEPROMData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_WriteEEPROM(int hReader, byte RegNo, byte eePROMData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_ReadUserPort(int hReader, ref byte pUserPortState);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_WriteUserPort(int hReader, byte userPortState);

        //======================================================================================
        // CARD COMMANDS
        //======================================================================================

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Select(int hReader, ref byte pResultTagType,
                                               ref byte pResultTagLength, ref byte pResultSN);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Login(int hReader, byte Sector, int keyType,
                                              byte storedNo, ref byte pKey);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Read(int hReader, byte Block, ref byte pBlockData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_ReadValue(int hReader, byte Block, ref byte pValueData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Write(int hReader, byte Block, ref byte pBlockData);

        [DllImport("ACR120U.DLL", CharSet = CharSet.Auto)]
        public static extern int ACR120_WriteValue(int hReader, byte Block, int ValueData);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_WriteMasterKey(int hReader, byte keyNo, ref byte pKey);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Inc(int hReader, byte Block,
                                            int value, ref int pNewValue);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Dec(int hReader, byte Block,
                                            int value, ref int pNewValue);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Copy(int hReader, byte srcBlock,
                                             byte desBlock, ref int pNewValue);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_Power(int hReader, byte State);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_ListTags(int hReader, ref byte pNumTagFound, ref byte pTagType,
                                                 ref byte pTagLength, ref byte pSN);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_MultiTagSelect(int hReader, byte TagLength, ref byte SN,
                                                       ref byte pResultTagType, ref byte pResultTagLength,
                                                       ref byte pResultSN);

        [DllImport("ACR120U.DLL")]
        public static extern int ACR120_TxDataTelegram(int hReader, byte SendDataLength, ref byte pSendData,
                                                       ref byte pReceivedDataLength, ref byte pReceivedData);

        public static string GetErrMsg(int code)
        {
            switch (code)
            {
                case -1000:
                    return ("Unexpected Internal Library Error : -1000");

                case -2000:
                    return ("Invalid Port : -2000");

                case -2010:
                    return ("Port Occupied by Another Application : -2010");

                case -2020:
                    return ("Invalid Handle : -2020");

                case -2030:
                    return ("Incorrect Parameter : -2030");

                case -3000:
                    return ("No TAG Selected or in Reachable Range : -3000");

                case -3010:
                    return ("Read Failed after Operation : -3010");

                case -3020:
                    return ("Block doesn't contain value : -3020");

                case -3030:
                    return ("Operation Failed : -3030");

                case -3040:
                    return ("Unknown Reader Error : -3040");

                case -4010:
                    return ("Invalid stored key format in login process : -4010");

                case -4020:
                    return ("Reader can't read after write operation : -4020");

                case -4030:
                    return ("Decrement Failure (Empty) : -4030");

            }
            return ("Error is not documented.");
        }

        public ACR120U()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
