// 
// Program.cs
//
using System;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace TelemetryFTC
    {
    //------------------------------------------------------------------------------------------------
    // Program
    //------------------------------------------------------------------------------------------------

    public class TelemetryContext
        {
        public class Cursor
            {
            public int iRow;
            public int iCol;
            }

        public Excel.Workbook               Workbook;
        public Excel.Worksheet              Sheet;
        public IDictionary<int, Cursor>     Cursors;    // keeps track of where the next record is to be written in each sheet

        public TelemetryContext()
            {
            Initialize();
            }

        public void Initialize()
            {
            this.Workbook = null;
            this.Sheet    = null;
            this.Cursors  = new Dictionary<int, Cursor>();
            }

        public Cursor InitCursor(int iRow, int iCol)
            {
            Cursor cursor = new Cursor();
            cursor.iRow = iRow;
            cursor.iCol = iCol;
            this.Cursors[this.Sheet.Index] = cursor;
            return cursor;
            }
        };

    class Program
        {
        //--------------------------------------------------------------------------
        // State
        //
        // REVIEW: Some of these could be better factored into other locations.
        //--------------------------------------------------------------------------

        public static string         Mailbox         = null;     // from command line arg
        public static string         NXTConnectionID = null;     // from command line arg
        public static bool           Joysticks       = true;     // from command line arg: enable joysticks if present

        public static TelemetryFTCUI        TheForm          = null;                    // our sole instance of the UI
        public static KnownNXT              CurrentNXT       = null;                    // currently running/connected NXT
        public static TelemetryContext      TelemetryContext = new TelemetryContext();  // where to put our received telemetry

        //--------------------------------------------------------------------------
        // Debugging
        //--------------------------------------------------------------------------

        public static void Trace(string sFormat, params object[] data)
            {
            System.Diagnostics.Debug.WriteLine(sFormat, data);
            }

        public static void ReportError(string sFormat, params object[] data)
            {
            string sMessage = String.Format(sFormat, data);
            string sTitle   = ProgramName;
            System.Windows.Forms.MessageBox.Show(sMessage, sTitle);
            }

        public static RET_T Fail<RET_T>()
            {
            System.Diagnostics.Debug.Fail("program exiting");
            return default(RET_T);
            }

        //--------------------------------------------------------------------------
        // Argument processing
        //--------------------------------------------------------------------------

        static string ProgramName = "TelemetryFTC";

        static void Usage()
            {
            Program.ReportError(
                "usage: {0} [-mailboxn] [-port COMn] [-nojoy]\n" +
                "    -mailboxn    the 'mailbox' on the NXT. default: -mailbox2\n" +
                "    -port COMn   the serial port on which NXT is found\n" +
                "    -nojoy       do not enable joysticks by default", ProgramName
                );
            AbortProgram();
            }

        static void ParseArgs(string[] args)
            {
            for (int iArg = 0; iArg < args.Length; iArg++)
                {
                if (args[iArg][0] == '-' || args[iArg][0] == '/')
                    {
                    if (args[iArg].Length > 1)
                        {
                        string param = args[iArg].Substring(1);
                        switch (param.ToLowerInvariant())
                            {
                        case "?":
                            Usage();
                            break;
                        case "nojoy":
                            Program.Joysticks = false;
                            continue;
                        case "port":
                            iArg++;
                            if (iArg < args.Length)
                                {
                                Program.NXTConnectionID = args[iArg];
                                continue;
                                }
                            break;
                        case "mailbox1":
                        case "mailbox2":
                        case "mailbox3":
                        case "mailbox4":
                        case "mailbox5":
                        case "mailbox6":
                        case "mailbox7":
                        case "mailbox8":
                        case "mailbox9":
                        case "mailbox10":
                            Program.Mailbox = param;
                            continue; 
                            }
                        }
                    }
                Usage();
                }
            }

        public static void AbortProgram()
            {
            System.Environment.Exit(-1);
            }

        //--------------------------------------------------------------------------
        // Test Logic
        //--------------------------------------------------------------------------

        static void DoTest()
            {
            BluetoothConnection.GetNXTBluetoothSerialPortNames();
            }

        //--------------------------------------------------------------------------
        // Main loop
        //--------------------------------------------------------------------------

        static void LaunchUserInterface()
            {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new TelemetryFTCUI());
            }

        [STAThread]
        static void Main(string[] args)
            {
            System.Threading.Thread.CurrentThread.Name = "Program.Main";

            ParseArgs(args);
            JoystickController.FindJoystickControllers();
            LaunchUserInterface();
            // DoTest();
            }
        }
    }
