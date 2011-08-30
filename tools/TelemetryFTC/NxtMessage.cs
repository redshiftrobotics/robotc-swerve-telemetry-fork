//
// NxtMessage.cs
//
// Classes that implement and process the built-in direct and system commands that can be
// sent to the NXT. Consult the Lego-provided Bluetooth Developers Kit for details.
// 
using System.Text;
using System.Threading;

namespace TelemetryFTC
    {
    //=======================================================================================================

    public enum COMMAND_TYPE : byte
        {
        DIRECT_REPLY_REQUIRED       = 0x00,
        SYSTEM_REPLY_REQUIRED       = 0x01,
        REPLY                       = 0x02,
        DIRECT_NO_REPLY_REQUIRED    = 0x80, // nb: NXT sometimes sends replies anyway!
        SYSTEM_NO_REPLY_REQUIRED    = 0x81,
        }

    public enum SYSTEM_COMMAND : byte
        {
        OPEN_READ                   = 0x80,
        OPEN_WRITE                  = 0x81,
        READ                        = 0x82,
        WRITE                       = 0x83,
        CLOSE                       = 0x84,
        DELETE                      = 0x85,
        FIND_FIRST                  = 0x86,
        FIND_NEXT                   = 0x87,
        GET_FIRMWARE_VERSION        = 0x88,
        OPEN_WRITE_LINEAR           = 0x89,
        OPEN_READ_LINEAR            = 0x8A,
        OPEN_WRITE_DATA             = 0x8B,
        OPEN_APPEND_DATA            = 0x8C,

        BOOT                        = 0x97,
        SET_BRICK_NAME              = 0x98,
        GET_DEVICE_INFO             = 0x9B,

        DELETE_USER_FLASH           = 0xA0,
        POLL_LENGTH                 = 0xA1,
        POLL                        = 0xA2,
        BLUETOOTH_FACTORY_RESET     = 0xA4,
        }

    public enum DIRECT_COMMAND : byte
        {
        START_PROGRAM               = 0x00,
        STOP_PROGRAM                = 0x01,
        PLAY_SOUND_FILE             = 0x02,
        PLAY_TONE                   = 0x03,
        SET_OUTPUT_STATE            = 0x04,
        SET_INPUT_MODE              = 0x05,
        GET_OUTPUT_STATE            = 0x06,
        GET_INPUT_VALUES            = 0x07,
        RESET_INPUT_SCALED_VALUE    = 0x08,
        MESSAGE_WRITE               = 0x09,
        RESET_MOTOR_POSITION        = 0x0A,
        GET_BATTERY_LEVEL           = 0x0B,
        STOP_SOUND_PLAYBACK         = 0x0C,
        KEEP_ALIVE                  = 0x0D,
        LS_GET_STATUS               = 0x0E,
        LS_WRITE                    = 0x0F,
        LS_READ                     = 0x10,
        GET_CURRENT_PROGRAM_NAME    = 0x11,
        MESSAGE_READ                = 0x13, // nb: not 0x12
        }

    public enum ERROR_CODE : byte
        {
        // From Bluetooth develpers guide
        SUCCESS                                             = 0x00,
        STAT_MSG_BUFFERWRAP                                 = 0x10, // Datalog buffer not being read fast enough
        STAT_COMM_PENDING                                   = 0x20, // Pending setup operation in progress
        STAT_MSG_EMPTY_MAILBOX                              = 0X40, // Specified mailbox contains no new messages

        REQUEST_FAILED_SPECIFIED_FILE_NOT_FOUND             = 0XBD,
        UNKNOWN_COMMAND_OPCODE                              = 0XBE,
        INSANE_PACKET                                       = 0XBF,
        DATA_CONTAINS_OUT_OF_RANGE_VALUES                   = 0XC0,
        COMMUNICATION_BUS_ERROR                             = 0XDD,
        NO_FREE_MEMORY_IN_COMMUNICATION_BUFFER              = 0XDE,
        SPECIFIED_CHANNEL_CONNECTION_IS_NOT_VALID           = 0XDF,
        SPECIFIED_CHANNEL_CONNECTION_NOT_CONFIGURED_OR_BUSY = 0XE0,
        NO_ACTIVE_PROGRAM                                   = 0XEC,
        ILLEGAL_SIZE_SPECIFIED                              = 0XED,
        ILLEGAL_MAILBOX_QUEUE_ID_SPECIFIED                  = 0XEE,
        ATTEMPTED_TO_ACCESS_INVALID_FIELD_OF_A_STRUCTURE    = 0XEF,
        BAD_INPUT_OR_OUTPUT_SPECIFIED                       = 0XF0,
        INSUFFICIENT_MEMORY_AVAILABLE                       = 0XFB,
        BAD_ARGUMENTS                                       = 0XFF,

        // From firmware source code (c_cmd.iom)
        ILLEGAL_BYTECODE_INSTRUCTION                        = 0xFE,
        MALFORMED_FILE_CONTENTS                             = 0xFD,
        FIRMWARE_VERSION_MISMATCH                           = 0xFC,
        BAD_POINTER                                         = 0xFA,

        // Placeholder until we know for sure
        UNDEFINED_ERROR                                     = 138,
        }

    public enum TELEMETRY_META_COMMAND : byte
        {
        POLLING_INTERVAL        = 0,
        BACKCHANNEL_MAILBOX     = 1,
        ZERO_TELEMETRY_DATA     = 2,
        }

    //=======================================================================================================

    public class NxtMessage
    // A message to send to the NXT
        {
        //--------------------------------------------------------------------------
        // Types
        //--------------------------------------------------------------------------

        public class PayloadAccessor
            {
            NxtMessage msg;

            public PayloadAccessor(NxtMessage msg)
                {
                this.msg = msg;
                }

            public byte this[int ib]
                {
                get { return this.msg.RgbMessage[4+ib]; }
                set {        this.msg.RgbMessage[4+ib] = value; }
                }

            }

        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public byte[]           RgbMessage;
        public PayloadAccessor  Payload            = null;
        public System.DateTime  TimeOfTransmission = System.DateTime.MinValue;

        public bool             ReplyValid      = false;
        public EventWaitHandle  ReplyValidEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        // Doesn't count the two length bytes, but does count the command type, command, and payload
        public int    CbMessage   { 
            get { 
                return ((int)RgbMessage[0]) | (((int)RgbMessage[1]) << 8);
                } 
            set {             
                this.RgbMessage[0] = (byte)(value & 0xFF);
                this.RgbMessage[1] = (byte)((value>>8) & 0xFF); 
                }}

        public byte     CommandType { get { return RgbMessage[2]; } set { RgbMessage[2] = value; }}
        public byte     Command     { get { return RgbMessage[3]; } set { RgbMessage[3] = value; }}

        public byte[]   DataWithoutLengthCountPrefix { get {
            byte[] rgbToSend = new byte[this.RgbMessage.Length-2];
            System.Array.Copy(this.RgbMessage, 2, rgbToSend, 0, rgbToSend.Length);
            return rgbToSend;
            }}

        // When sending to USB, we don't include the overall length count
        // which is present at the start of the message in the Bluetooth case;
        // the USB infrastructure provides its own implicit framing
        public byte[] DataForUSBTransmission { get { return this.DataWithoutLengthCountPrefix; }}

        // The bluetooth payload includes additional framing bytes that indicate length
        public byte[] DataForBluetoothTransmission { get { return this.RgbMessage; }}

        // Surprisingly, the payload for the IP case mirrors that of the USB case, not the Bluetooth case!
        // This seems an odd design to me....
        public byte[] DataForIPTransmission        { get { return this.DataWithoutLengthCountPrefix; }}

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------
        
        public NxtMessage(COMMAND_TYPE commandType, byte command, int cbPayload)
            {
            int cbMessage   = 4 + cbPayload;  // two bytes for length, command type, command, payload
            int cbTotal     = cbMessage - 2;  // don't count the two length bytes
            this.RgbMessage = new byte[cbMessage];

            this.RgbMessage[0] = (byte)(cbTotal & 0xFF);
            this.RgbMessage[1] = (byte)((cbTotal>>8) & 0xFF);
            this.RgbMessage[2] = (byte)commandType;
            this.RgbMessage[3] = command;

            this.Payload = new PayloadAccessor(this);
            }

        public NxtMessage(COMMAND_TYPE commandType, SYSTEM_COMMAND command, int cbPayload) : this(commandType, (byte)command, cbPayload)
            {
            }
        public NxtMessage(COMMAND_TYPE commandType, DIRECT_COMMAND command, int cbPayload) : this(commandType, (byte)command, cbPayload)
            {
            }

        //--------------------------------------------------------------------------
        // Access
        //--------------------------------------------------------------------------

        public virtual void ProcessReply(byte[] rgbPayload, Connection connection)
            {
            // subclass responsibility
            }

        protected void NoteReplyValid()
            {
            this.ReplyValid = true;
            this.ReplyValidEvent.Set();
            }

        public bool AwaitReply(int msWait = 1000)
            {
            this.ReplyValidEvent.WaitOne(msWait);
            return this.ReplyValid;
            }

        public void NoteSent()
        // Note that this message has been sent
            {
            this.TimeOfTransmission = System.DateTime.Now;
            }
        }

    //=======================================================================================================

    public class MessageWriteNxtMessage : NxtMessage
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public const int dibPayload = 6;                   // offset from StartFunction of message to the payload

        public byte this[int ib]
            {
            get { return this.RgbMessage[dibPayload + ib]; }
            set { this.RgbMessage[dibPayload + ib] = value ; }
            }

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public MessageWriteNxtMessage(byte mailbox, int cbPayload) : base(COMMAND_TYPE.DIRECT_NO_REPLY_REQUIRED, DIRECT_COMMAND.MESSAGE_WRITE, 2 + cbPayload)
            {
            this.RgbMessage[4] = mailbox;
            this.RgbMessage[5] = (byte)cbPayload;
            }
        }

    //=======================================================================================================

    public class MessageReadNxtMessage : NxtMessage
    // c_cmd.c(823)
        {
        public MessageReadNxtMessage(byte mailbox) : base(0x00, DIRECT_COMMAND.MESSAGE_READ, 3)
            {
            this.Payload[0] = mailbox;          // remote inbox number
            this.Payload[1] = this.Payload[0];  // local inbox number
            this.Payload[2] = 0;                // fRemove
            }

        public override void ProcessReply(byte[] rgbPayload, Connection connection)
            {
            byte bReply      = rgbPayload[0];
            byte bCommand    = rgbPayload[1];
            byte bStatus     = rgbPayload[2];
            byte bLocalInbox = rgbPayload[3];   // just an echo of what we passed in our 'send': so we know where to put the msg
            byte cbMessage   = rgbPayload[4];

            Program.Trace("msg read: cmd={0} stat={1} inbox={2} cb={3}", bCommand, ((ERROR_CODE)bStatus).ToString(), bLocalInbox, cbMessage);
            }
        }
    //=======================================================================================================
    
    public class GetDeviceInfoNxtMessage : NxtMessage
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

               string nxtName;
        public string NxtName { get { return null==nxtName ? "" : nxtName; } set { nxtName = value; }}

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public GetDeviceInfoNxtMessage() : base(COMMAND_TYPE.SYSTEM_REPLY_REQUIRED, SYSTEM_COMMAND.GET_DEVICE_INFO, 0) // system command type, command 'Get Device Info'
            {
            }

        //--------------------------------------------------------------------------
        // Access
        //--------------------------------------------------------------------------

        public override void ProcessReply(byte[] rgbPayload, Connection connection)
            {
            byte bReply   = rgbPayload[0];
            byte bCommand = rgbPayload[1];
            byte bStatus  = rgbPayload[2];
            const int dib = 3;
            if (rgbPayload.Length >= 30+dib && bCommand==0x9b)
                {
                StringBuilder s = new StringBuilder();
                for (int ich = 0+dib; ich <= 14+dib && rgbPayload[ich]!=0; ich++)
                    {
                    s.Append((char)rgbPayload[ich]);
                    }
                this.NxtName = s.ToString();
                //
                Program.Trace("GetDeviceInfoNxtMessage: reply received: {0}", this.NxtName);
                this.NoteReplyValid();
                }
            }

        }


    //=======================================================================================================

    public enum POLLTYPE
        {
        PollBuffer      = 0,
        HighSpeedBuffer = 1,
        Dummy           = 39,
        };

    public class PollLengthNxtMessage : NxtMessage
        {
        public int CbAvailable = 0;

        public PollLengthNxtMessage(POLLTYPE type) : base(COMMAND_TYPE.SYSTEM_REPLY_REQUIRED, SYSTEM_COMMAND.POLL_LENGTH, cbPayload: 1)
            {
            this.Payload[0] = (byte)type;
            }

        public override void ProcessReply(byte[] rgbPayload, Connection connection)
            {
            if (rgbPayload.Length >= 4)
                {
                // NB: this is the right order; the Mindstorms doc is wrong (it swaps bStatus and bBuffer)
                byte bReply   = rgbPayload[0];  // 0x02
                byte bCommand = rgbPayload[1];
                byte bStatus  = rgbPayload[2];
                byte bBuffer  = rgbPayload[3];
                byte cb       = rgbPayload[4];
                //
                this.CbAvailable = cb;
                this.NoteReplyValid();
                //
                Program.Trace("poll len: cmd={0} buf={1} stat={2} cb={3} ", bCommand, bBuffer, ((ERROR_CODE)bStatus).ToString(), cb);
                }
            }
        }

    //=======================================================================================================

    public class PollNxtMessage : NxtMessage    
        {
        public PollNxtMessage(POLLTYPE type) : base(COMMAND_TYPE.SYSTEM_REPLY_REQUIRED, SYSTEM_COMMAND.POLL, cbPayload: 2)
            {
            this.Payload[0] = (byte)type;
            //
            // Set the maximum number of payload bytes to return
            //
            switch (type)
                {
            case POLLTYPE.PollBuffer:
                // If we're communicating over USB, the NXT has a 64 byte internal buffer (USBBUF). Five
                // of these bytes in the reply are overhead (see ProcessReply below), leaving 59 for data.
                //
                // Note that the NXT Bluetooth buffer is larger, so this should work for both USB and Bluetooth.
                //
                this.Payload[1] = 59;   // 59 works; 60 verified to give an error
                break;
            default:
                this.Payload[1] = 59;   // not yet tested
                break;
                }
            }

        public override void ProcessReply(byte[] rgbPayload, Connection connection)
        // The POLLs we send return telemetry data as a sequence of chunks
            {
            // Program.Trace("telemetry reply received");
            if (rgbPayload.Length >= 5)
                {
                byte bReply   = rgbPayload[0];  // 0x02
                byte bCommand = rgbPayload[1];  // 0xA2
                byte bStatus  = rgbPayload[2];  // 0==success
                byte bBuffer  = rgbPayload[3];  // 0x00==poll buffer, 0x01==high speed buffer
                byte cb       = rgbPayload[4];  // length of data

                if (cb != 0 || Program.TheForm.TelemetryPollingInterval == 0)
                    {
                    // We're supposed to keep issuing poll commands until no data is returned, then wait the polling interval
                    connection.SendTelemetryPollMessage();
                    }

                // Program.Trace("poll: cmd={0} buf={1} stat={2} cb={3} ", bCommand, bBuffer, ((ERROR_CODE)bStatus).ToString(), cb);

                if (0 == bStatus)
                    {
                    byte[] rgb = rgbPayload.Slice(5,cb);
                    if (rgb.Length > 0)
                        {
                        connection.AddIncomingPolledData(rgb);
                        }
                    }

                this.NoteReplyValid();
                }
            else
                {
                Program.Trace("poll: illegal return buffer size");
                }
            }
        }   
    }
