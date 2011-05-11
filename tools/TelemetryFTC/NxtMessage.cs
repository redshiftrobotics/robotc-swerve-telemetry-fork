//
// NxtMessage.cs
//
using System.Text;

namespace TelemetryFTC
    {
    public class NxtMessage
    // A raw message to send to the NXT
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public byte[] rgbMessage;

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------
        
        public NxtMessage(byte commandType, byte command, int cbPayload)
            {
            int cbMessage   = 4 + cbPayload;  // two bytes for length, command type, command, payload
            int cbTotal     = cbMessage - 2;  // don't count the two length bytes
            this.rgbMessage = new byte[cbMessage];

            this.rgbMessage[0] = (byte)(cbTotal & 0xFF);
            this.rgbMessage[1] = (byte)((cbTotal>>8) & 0xFF);
            this.rgbMessage[2] = commandType;
            this.rgbMessage[3] = command;
            }
        }

    public class MailboxNxtMessage : NxtMessage
    // A 'MESSAGEWRITE' direct command
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public const int dibPayload = 6;                   // offset from StartFunction of message to the payload

        public byte this[int ib]
            {
            get { return this.rgbMessage[dibPayload + ib]; }
            set { this.rgbMessage[dibPayload + ib] = value ; }
            }

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public MailboxNxtMessage(byte mailbox, int cbPayload) : base(0x80, 0x09, 2 + cbPayload)
            {
            this.rgbMessage[4] = mailbox;
            this.rgbMessage[5] = (byte)cbPayload;
            }
        }
    
    public class GetDeviceInfoNxtMessage : NxtMessage
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public int      CbReply { get { return 33; }}
        public string   NxtName;

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public GetDeviceInfoNxtMessage() : base(0x01, 0x9B, 0) // system command type, command 'Get Device Info'
            {
            }

        //--------------------------------------------------------------------------
        // Access
        //--------------------------------------------------------------------------

        public bool ParseReply(byte[] rgb)
            {
            if (rgb.Length >= 33 && rgb[2+0]==0x02 && rgb[2+1]==0x09b)
                {
                StringBuilder s = new StringBuilder();
                for (int ich = 2+3; ich<= 2+17 && rgb[ich]!=0; ich++)
                    {
                    s.Append((char)rgb[ich]);
                    }
                this.NxtName = s.ToString();
                return true;
                }
            return false;
            }
        }

    }
