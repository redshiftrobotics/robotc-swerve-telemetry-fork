//
// TelemetryMessage.cs
//
using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace TelemetryFTC
    {
    // Values used to tag telemetry data coming from NXT
    public enum  DATUM_TYPE : byte
        {
        NONE = 0,
        INT8,
        INT16,
        INT32,
        UINT8,
        UINT16,
        UINT32,
        FLOAT,
        STRING,
        EOF,
        };

    // A telemetry record as received from the NXT
    public class TelemetryMessage
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public const int    cbMessageMax = 58;                      // REVIEW: is this *exactly* right?
        public byte[]       rgbBuffer    = new byte[cbMessageMax];
        public int          cbBuffer     = 0;
        public List<object> data         = new List<object>();
        public bool         fEof         = false;

        //--------------------------------------------------------------------------
        // Reception
        //--------------------------------------------------------------------------

        delegate T PFNPARSE<T>(byte[] value);

        // Parse these bytes into the data that they represent
        void Parse<T>(ref int ib, int cb, PFNPARSE<T> converter, bool fReverse) where T : struct
            {
            // Read the data and put in the correct order
            byte[] rgb = new byte[cb];
            Buffer.BlockCopy(rgbBuffer, ib, rgb, 0, cb);
            if (fReverse) Array.Reverse(rgb);

            // Convert and remember it
            T value = converter(rgb);
            this.data.Add(value);

            // Skip the data
            ib += cb;
            }

        // Parse the message contents. The message is a sequence of tagged data, where the tags are
        // drawn from DATUM_TYPE. Each type of data has its own data format, which is pretty straightfoward.
        // Note that only the lower four bits of the tags is significant. The upper four bits of the first
        // tag indicates the (zero-based) Sheet number which is to be logged to; the upper four bits of
        // remaining tags are currently unused.
        public void Parse()
            {
            this.data = new List<object>();

            bool fDone = false;

            // Parse the data in the message
            for (int ib = 0; !fDone && ib < this.cbBuffer; )
                {
                byte bTag = this.rgbBuffer[ib++];
                switch (bTag & 0x0F)
                    {
                case (byte)DATUM_TYPE.INT8:
                    Parse(ref ib, sizeof(sbyte), rgb => (sbyte)(rgb[0]), false);
                    break;
                case (byte)DATUM_TYPE.INT16:
                    Parse(ref ib, sizeof(short), rgb => BitConverter.ToInt16(rgb, 0), false);
                    break;
                case (byte)DATUM_TYPE.INT32:
                    Parse(ref ib, sizeof(int), rgb => BitConverter.ToInt32(rgb, 0), false);
                    break;

                case (byte)DATUM_TYPE.UINT8:
                    Parse(ref ib, sizeof(byte), rgb => (byte)(rgb[0]), false);
                    break;
                case (byte)DATUM_TYPE.UINT16:
                    Parse(ref ib, sizeof(ushort), rgb => (ushort)BitConverter.ToInt16(rgb, 0), false);
                    break;
                case (byte)DATUM_TYPE.UINT32:
                    Parse(ref ib, sizeof(uint), rgb => (uint)BitConverter.ToInt32(rgb, 0), false);
                    break;

                case (byte)DATUM_TYPE.FLOAT:
                    Parse(ref ib, sizeof(float), rgb => BitConverter.ToSingle(rgb, 0), false);
                    break;

                case (byte)DATUM_TYPE.STRING:
                    int cch = rgbBuffer[ib++];
                    StringBuilder s = new StringBuilder();
                    for (int ich = 0; ich < cch; ich++)
                        {
                        s.Append((char)rgbBuffer[ib++]);
                        }
                    this.data.Add(s.ToString());
                    break;

                case (byte)DATUM_TYPE.EOF:
                    this.fEof = true;
                    fDone = true;
                    break;

                default:
                    fDone = true;
                    break;
                    }
                }
            }

        int sheetIndex { get 
        // return the zero-based sheet index in which we are to post this data
            {
            if (this.cbBuffer > 0)
                {
                return ((this.rgbBuffer[0] >> 4) & 0x0F);
                }
            else
	            {
                return 0;
	            }
            }}

        //--------------------------------------------------------------------------
        // Excel communication
        //--------------------------------------------------------------------------

        // Return the name of this cell in "A1" notation. Note that iCol & iRow here are zero-based
        public static string CellName(int iRow, int iCol)
            {
            char ch = (char)((iCol % 26) + 'A');
            StringBuilder result = new StringBuilder();
            result.Append(ch);
            //
            while (iCol > 26)
                {
                result.Append(ch);
                iCol -= 26;
                }                
            //
            result.Append((iRow+1).ToString());
            return result.ToString();
            }

        // Send the parsed data to the worksheet at the indicated location
        public void PostToSheet()
            {
            // Make sure we have the right sheet
            if (null != Program.TelemetryContext.Sheet)
                {
                int index = this.sheetIndex + 1;
                if (Program.TelemetryContext.Sheet.Index != index)
                    {
                    Excel.Workbook wb = Program.TelemetryContext.Workbook;
                    //
                    while (wb.Worksheets.Count < index)
                        {
                        wb.Worksheets.Add(After: wb.Worksheets[wb.Worksheets.Count]);
                        }
                    //
                    Program.TelemetryContext.Sheet = wb.Worksheets[index];
                    }
                }

            // Put the data on the sheet, and advance the cursor so that the 
            // next record won't overwrite it
            if (null != Program.TelemetryContext.Sheet)
                {
                TelemetryContext.Cursor cursor;
                if (!Program.TelemetryContext.Cursors.TryGetValue(Program.TelemetryContext.Sheet.Index, out cursor))
                    {
                    cursor = Program.TelemetryContext.InitCursor(0,0);
                    }

                Excel.Range range = Program.TelemetryContext.Sheet.get_Range(
                    CellName(cursor.iRow, cursor.iCol+0), 
                    CellName(cursor.iRow, cursor.iCol+data.Count-1)
                    );
                range.set_Value(value: data.ToArray());

                cursor.iRow++;
                }
            }
        }

    //------------------------------------------------------------------------------------------------
    // TelemetryFTCConnection
    //------------------------------------------------------------------------------------------------

    class TelemetryFTCConnection : SerialConnection
        {
        public static int iMailbox = 1; // NXT BT protocol mailbox used to receive telemetry messages (zero based)

        public TelemetryFTCConnection()
            {
            serialPort.DataReceived += this.OnDataReceived;
            }

        void OnDataReceived(object senser, System.IO.Ports.SerialDataReceivedEventArgs e)
            {
            // Note: the packet format here is described in the "Lego Mindstorms NXT Bluetooth Development Kit"
            // http://mindstorms.lego.com/en-us/support/files/default.aspx
            switch (e.EventType)
                {
            case System.IO.Ports.SerialData.Chars:
                {
                while (serialPort.BytesToRead > 0)      // NOTE: NOT this.cbAvailable
                    {
                    byte lsb = ReadByte();
                    byte msb = ReadByte();
                    int cbMessage = 256 * (int)msb + lsb;
                    int cbAvailable = this.CbAvailable;
                    if (cbAvailable >= cbMessage)
                        {
                        byte bCommandType = ReadByte();
                        switch (bCommandType)
                            {
                        case 0x80:              // 0x80 == 'direct' command (not system command), no response required
                            {
                            byte bCommand  = ReadByte();    // the 'direct' command
                            byte mailbox   = ReadByte();    // the mailbox/queue to which the data was sent, should be iMailbox
                            byte cbPayload = ReadByte();    // the size of the packet that came from the NXT

                            TelemetryMessage msg = new TelemetryMessage();
                            msg.cbBuffer = cbPayload;
                            ReadBytes(msg.rgbBuffer, 0, msg.cbBuffer);
                            SkipBytes(cbMessage - 4 - cbPayload);

                            switch (bCommand)
                                {
                            case 9:                             // 9=='message write' command
                                lock (this.messages)
                                    {
                                    this.messages.Add(msg);
                                    }
                                this.MessageAvailableEvent.Set();
                                break;
                            default:
                                throw new Exception(string.Format("unknown packet command received: {0}", bCommand));
                                }
                            }
                            break;

                        case 0x02:                              // reply packet
                            {
                            byte bCommand = ReadByte();
                            byte bStatus = ReadByte();
                            // Program.Trace("{0} {1}", bCommand, bStatus);
                            SkipBytes(cbMessage - 3);
                            }
                            break;

                        default:
                            // It's a command type we don't know how to process. Just skip the message and hope for the best.
                            SkipBytes(cbMessage - 1);
                            break;
                            }
                        }
                    else
                        {
                        // Not all of the bytes of the packet are yet available to read. So 
                        // put these two back until more data is around and we'll try again.
                        //
                        PutBack(msb);
                        PutBack(lsb);
                        }
                    }
                break;
                }
            // end switch
                }
            }
        }
    }
