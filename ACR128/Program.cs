/*
    ACR1281U-C8 Mifare Classic Serial Number Reader
    Modularsoft Sdn Bhd
    V1.0

    Date Created        :       03/07/2019
    Developer           :       Amir Asyraf | amirasyraf@outlook.com | 0193902459 | amirasyraf.dev
    Description         :       Continually reads Mifare Classic RFID card and sends the serial number as input (like copy/paste)

    TODOS               :       1) GUI
                                2) Error Handling
                                3) Clean Code
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace ACR128
{
    public class ProgramConfig
    {
        public int organizationChoice { get; set; }
        public string pollingRate { get; set; }
    }
    class Program
    {
        /* CONSTANTS */
        private const int PORT = 0; // Always 0
        private const int ASCC = 1;
        private const int UTEM = 2;
        private const int UPSI = 3;
        private const int UUM = 4;
        private const int DELAY_RATE = 1500; // Room for 'optimization' hehehehe. Used in Program_Delay()

        private static int hReader = 0;
        private static bool ISCONNECTED = false;
        private static int status = -1; // Status code returned by ACR128

        static ProgramConfig config = new ProgramConfig();

        static void Main(string[] args)
        {
            string serialNumber = "0000000000";
            string serialNumber2 = "0000000001";
            byte[] dataRead = new byte[8]; // Will contain card's Serial Number AKA UID
            byte[] tagType = new byte[50];
            byte ResultTag = 0;

            if (!Program_Initialize()) // Program_Initialize returns False. Exit program. Possible permission issue.
                Exit_Program();

            Reader_Initialize();

            Program_Delay();

            for (; ; )
            {
                /* Slow down reader read operation based on polling rate since there's no reason to have it run fast continually */
                Thread.Sleep(Int32.Parse(config.pollingRate));

                status = ACR120U.ACR120_Select(hReader, ref tagType[0], ref ResultTag, ref dataRead[0]); // Read operation

                if (status == 0) // 0 = No error/successful
                {
                    serialNumber = "0000000000";
                    serialNumber = ByteArrayToString(dataRead).Substring(0, 8);

                    if (config.organizationChoice == UTEM) // UTEM uses numbers only. Convert from Hex to Int
                        serialNumber = Convert.ToString(Int64.Parse(serialNumber, System.Globalization.NumberStyles.HexNumber));

                    Console.WriteLine("Serial Number: " + serialNumber);

                    /* Checks if the card being read is the same. If yes, no need to continually send input */
                    if (string.Equals(serialNumber2, serialNumber))
                        continue;
                    else
                    {
                        SendKeys.SendWait(serialNumber.Trim());
                        serialNumber2 = serialNumber;
                    }

                }

                /* No card is detected by the reader. Sets both serial number variables to 00000000 as a reset procedure */
                else if (status == 62536)
                {
                    serialNumber = "0000000000";
                    serialNumber2 = "0000000000";
                    Console.WriteLine("Serial Number: " + serialNumber);
                    continue;
                }
                /* TODO: Error handling? */
                else
                    continue;
            }
        }
        private static void Reader_Initialize()
        {
            Console.WriteLine("Connecting to reader...");
            Reader_Disconnect();
            Reader_Connect();

            if (ISCONNECTED)
                Console.WriteLine("Successful!");
        }
        private static void Reader_Connect()
        {
            while (hReader < 0)
            {
                hReader = ACR120U.ACR120_Open(PORT);

                //if success return 0 else return < 0
                if (hReader >= 0)
                {
                    if (!ISCONNECTED)
                    {
                        ISCONNECTED = true;
                        break;
                    }
                    else
                    {
                    }
                }
                else
                {
                    string err = ACR120U.GetErrMsg(hReader);
                    ISCONNECTED = false;
                }
            }
        }
        private static void Reader_Disconnect()
        {
            status = ACR120U.ACR120_Close(hReader);
            hReader = -1;
            ISCONNECTED = false;
        }
        private static string ByteArrayToString(byte[] data)
        {
            StringBuilder sDataOut;

            if (data != null)
            {
                sDataOut = new StringBuilder(data.Length * 2);
                for (int nI = 0; nI < data.Length; nI++)
                    sDataOut.AppendFormat("{0:X02}", data[nI]);
            }
            else
                sDataOut = new StringBuilder();

            return sDataOut.ToString();
        }
        /* TODO: Error Handling for read operation of config.json */
        private static Boolean Program_Initialize()
        {
            for (int fail = 0; fail < 3; fail++)
            {
                if (File.Exists("config.json"))
                {
                    Console.WriteLine("Reading Config File...");
                    Program_Delay();
                    config = JsonConvert.DeserializeObject<ProgramConfig>(File.ReadAllText(@"config.json"));
                    Console.WriteLine("Successful!");
                    break;
                }
                else // If file not found, assume as first initialization, create new config file
                {
                    Console.WriteLine("config.json not found. Initiating config initialization...");
                    Program_Delay();
                    Write_Config();
                    Console.WriteLine("Writing config.json...");
                    Program_Delay();
                    Console.WriteLine("Successful!");
                }

                if (fail == 2)
                    return false;
            }

            return true;
        }
        /* TODO: Error Handling for write operation of config.json and user input */
        private static void Write_Config()
        {
            int choice;
            string rate = String.Empty;
            Console.WriteLine("\n\n****************PROGRAM INITIALIZATION****************\n");
            Console.WriteLine("ASCC: 1");
            Console.WriteLine("UTEM: 2");
            Console.WriteLine("UPSI: 3");
            Console.WriteLine("UUM: 4");
            Console.WriteLine("Others: 5");
            Console.Write("Enter organization: ");
            choice = Convert.ToInt32(Console.ReadKey().Key.ToString().Substring(1));
            Console.WriteLine();

            Console.Write("Enter polling rate(ms)(recommended 250): ");
            rate = Console.ReadLine();
            Console.WriteLine();
            if (rate == "")
                rate = "250";

            ProgramConfig config = new ProgramConfig
            {
                organizationChoice = choice,
                pollingRate = rate
            };
            File.WriteAllText(@"config.json", JsonConvert.SerializeObject(config));
        }
        private static void Exit_Program()
        {
            Console.WriteLine("Unable to initialize program for first time use. Possible file permission issue.");
            Console.WriteLine("Exiting Program, Press Any Key to Exit");
            Console.ReadKey();
            System.Environment.Exit(1);
        }
        private static void Program_Delay()
        {
            System.Threading.Thread.Sleep(DELAY_RATE);
        }
    }
}
