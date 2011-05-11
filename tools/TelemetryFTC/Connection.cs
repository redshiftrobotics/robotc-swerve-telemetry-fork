﻿//
// Connection.cs
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace TelemetryFTC
    {
    //------------------------------------------------------------------------------------------------
    // BluetoothSerialPort
    //
    // A wrapper around a serial port we know to have bluetooth underneath.
    //------------------------------------------------------------------------------------------------

    class ThreadContext
        {
        public ParameterizedThreadStart StartFunction  = null;
        public Thread                   Thread         = null;
        public bool                     StopRequest    = false;
        public EventWaitHandle          StopEvent      = new EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);

        public ThreadContext(ParameterizedThreadStart start)
            {
            this.StartFunction = start;
            }

        public void Start()
            {
            if (this.Thread == null)
                {
                this.Thread = new Thread((ctx) => ((ThreadContext)ctx).ThreadRoot());
                this.StopRequest = false;
                this.StopEvent.Reset();
                this.Thread.Start(this);
                }
            }
        public void Stop()
            {
            if (this.Thread != null)
                {
                this.StopRequest = true;
                this.StopEvent.Set();
                this.Thread.Join();
                this.Thread = null;
                }
            }

        private void ThreadRoot()
            {
            System.Threading.Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);
            this.StartFunction(this);
            }
        };

    // A serial port which is known to overlay a bluetooth port
    public class BluetoothSerialPort
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        public string           SerialPortDeviceID = "COM1";
        public bool             HasNXTConnected    = false;
        public string           NxtName            = "";
        public string PortName           { get { return SerialPortDeviceID; }}             // serial port name: 'COM4'
        public string ListBoxDisplayName { get { return SerialPortDeviceID + (HasNXTConnected ? " - " + NxtName : ""); }}   // for use in listbox UI

        public BluetoothSerialPort()
            {
            telemetryReceiveThread  = new ThreadContext((ctx) => ReceiveTelemetryMessages((ThreadContext)ctx));
            telemetrySendThread     = new ThreadContext((ctx) => SendNxtMessages((ThreadContext)ctx));
            }

        TelemetryFTCConnection  connection   = new TelemetryFTCConnection();
        ThreadContext           telemetryReceiveThread;
        ThreadContext           telemetrySendThread;
        List<NxtMessage>        messagesToSend = new List<NxtMessage>();

        //--------------------------------------------------------------------------
        // Communication
        //--------------------------------------------------------------------------

        public bool IsOpen { get { return this.connection.IsOpen; }}

        public void Open()
        // Idempotent
            {
            if (!this.IsOpen)
                {
                try {
                    this.connection.PortName = this.PortName;
                    this.connection.Open();
                    }
                catch (System.TimeoutException)
                    {
                    this.connection.Close();    // REVIEW: more of an error message?
                    }
                }
            }

        public void Close()
        // Idempotent
            {
            if (this.IsOpen)
                {
                this.connection.Close();
                }
            }

        public void Run(bool fUseJoystick)
        // Idempotent
            {
            if (fUseJoystick && JoystickController.HasControllers())
                {
                this.telemetrySendThread.Start();
                //
                Program.ActiveBTPort = this;
                Program.TheForm.timerJoystickTransmission.Enabled = true;
                }
            this.telemetryReceiveThread.Start();
            }

        public void Stop()
        // Idempotent
            {
            Program.TheForm.timerJoystickTransmission.Enabled = false;
            Program.ActiveBTPort = null;
            //
            this.telemetryReceiveThread.Stop();
            this.telemetrySendThread.Stop();
            }

        //-------------------------------

        public void SendJoystickMessage()
        // NOTE: must be called on the main thread
            {
            JoystickNxtMessage msg = new JoystickNxtMessage();
            //
            lock (this.messagesToSend)
                {
                this.messagesToSend.Add(msg);
                this.connection.MessageToSendSemaphore.Release();
                }
            }

        void SendNxtMessages(ThreadContext ctx)
            {
            WaitHandle[] waitHandles = new WaitHandle[2];
            waitHandles[0] = this.connection.MessageToSendSemaphore;
            waitHandles[1] = ctx.StopEvent;
            //
            while (!ctx.StopRequest)
                {
                int iWait = WaitHandle.WaitAny(waitHandles);
                //
                switch (iWait)
                    {
                case 0: // MessageToSendSemaphore
                    {
                    NxtMessage msg = null;
                    lock (this.messagesToSend)
                        {
                        msg = this.messagesToSend[0];
                        this.messagesToSend.RemoveAt(0);
                        }
                    this.connection.Send(msg);
                    break;
                    }
                case 1: // StopEvent
                    break;
                // end switch
                    }
                }
            }

        //-------------------------------

        void ReceiveTelemetryMessages(ThreadContext ctx)
            {
            Excel.Worksheet sheet = Program.TelemetryContext.sheet;
            //
            if (null != sheet)
                {
                bool fEof = false;
                //
                WaitHandle[] waitHandles = new WaitHandle[2];
                waitHandles[0] = this.connection.MessageAvailableEvent;
                waitHandles[1] = ctx.StopEvent;
                //
                for (int iRow = 0; !ctx.StopRequest && !fEof ; )
                    {
                    int iWait = WaitHandle.WaitAny(waitHandles);

                    switch (iWait)
                        {
                    case 0: // MessageAvailableEvent
                        {
                        // There's data there to read. 
                        for (;!fEof;)
                            {
                            TelemetryMessage message = null;
                            lock (connection.Records)
                                {
                                if (0 == connection.Records.Count)
                                    break;
                                message = connection.Records[0];
                                connection.Records.RemoveAt(0);
                                }
                            message.Parse();
                            //
                            if (message.fEof || message.data.Count == 0)
                                {
                                fEof = true;
                                }
                            else
                                {
                                message.PostToSheet(sheet, iRow++);
                                }
                            }
                        break;
                        }
                    case 1: // StopEvent
                        break;
                    // end switch
                        }
                    }
                }
            }

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        // Quickly see if there a NXT at the other end of this connection?
        public bool ProbeForNXT()
            {
            bool fResult = false;
            //
            SerialConnection cxn = new SerialConnection(this.PortName);
            if (cxn.Open(false))
                {
                Program.Trace("opened {0}", this.PortName);
                //
                try {
                    GetDeviceInfoNxtMessage msg = new GetDeviceInfoNxtMessage();
                    if (cxn.Send(msg))
                        {
                        Thread.Sleep(500);                              // Hack!
                        //
                        byte[] rgb = new byte[msg.CbReply];
                        cxn.ReadBytes(rgb, 0, msg.CbReply);
                        if (msg.ParseReply(rgb))
                            {
                            this.HasNXTConnected = true;
                            this.NxtName         = msg.NxtName;
                            Program.Trace("NXT attached");
                            }
                        }
                    }
                catch (System.TimeoutException)
                    {
                    }
                //
                cxn.Close();
                }
            else
                {
                Program.Trace("failed to open {0}", this.PortName);
                }
            //
            return fResult;
            }

        // Return the names serial ports on this computer which are overlaying a Bluetooth connection
        public static List<BluetoothSerialPort> Ports { get {
            //
            List<BluetoothSerialPort> result = new List<BluetoothSerialPort>();
            //
            TelemetryFTCUI.ShowWaitCursorWhile(() =>
                {
                IEnumerable<string> serialPortNames = GetBluetoothSerialPortNamesFromRegistry();
                //
                foreach (string serialPortName in serialPortNames)
                    {
                    BluetoothSerialPort port = new BluetoothSerialPort();
                    port.SerialPortDeviceID = serialPortName;
                    result.Add(port);
                    }
                });
            //
            return result;
            }}

        // Return the list of serial port names which have an underlying bluetooth connection
        static IEnumerable<string> GetBluetoothSerialPortNamesFromRegistry()
            {
            List<string> result = new List<string>();
            RegistryKey localMachine = null;
            RegistryKey key2 = null;
            new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\HARDWARE\DEVICEMAP\SERIALCOMM").Assert();
            try
                {
                localMachine = Registry.LocalMachine;
                key2 = localMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM", false);
                if (key2 != null)
                    {
                    foreach(string valueName in key2.GetValueNames())
                        {
                        if (valueName.StartsWith(@"\Device\BthModem", StringComparison.OrdinalIgnoreCase))
                            {
                            result.Add((string)key2.GetValue(valueName));
                            }
                        }
                    }
                }
            finally
                {
                if (localMachine != null)
                    {
                    localMachine.Close();
                    }
                if (key2 != null)
                    {
                    key2.Close();
                    }
                System.Security.CodeAccessPermission.RevertAssert();
                }

            result.Sort();
            return result.Distinct<string>();
            }
        }

    //------------------------------------------------------------------------------------------------
    // SerialConnection
    //------------------------------------------------------------------------------------------------

    // A connection on a serial port
    class SerialConnection
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        static public string DefaultPortName = "COM1";

        protected System.IO.Ports.SerialPort  serialPort = new System.IO.Ports.SerialPort();
        protected List<TelemetryMessage>      messages   = new List<TelemetryMessage>();

        public string                  PortName { set { serialPort.PortName = value; } }
        public int                     Queue    { set { } }
        public List<TelemetryMessage>  Records  { get { return this.messages; } }
        public EventWaitHandle         MessageAvailableEvent  = new EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
        public Semaphore               MessageToSendSemaphore = new Semaphore(0, Int32.MaxValue);

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public SerialConnection()
            {
            serialPort.DiscardNull  = false;
            serialPort.ReadTimeout  = 500;
            serialPort.WriteTimeout = 500;
            serialPort.BaudRate     = 115200;
            serialPort.PortName     = DefaultPortName;
            }

        public SerialConnection(string portName) : this()
            {
            serialPort.PortName = portName;
            }

        public bool Open(bool fTraceFailure=true)  
            {
            try 
                {
                serialPort.Open();
                return true;
                }
            catch (System.IO.IOException)
                {
                if (fTraceFailure)
                    Program.ReportError("warning: can't open serial port {0} (is the NXT connected?)", serialPort.PortName);
                return false;
                }
            }

        public bool IsOpen { get { return this.serialPort.IsOpen; }}

        public void Close()
            {
            if (serialPort.IsOpen)
                {
                serialPort.Close();
                }
            }


        //--------------------------------------------------------------------------
        // Data transmission
        //--------------------------------------------------------------------------

        public bool Send(NxtMessage msg)
            {
            bool fResult = false;
            if (this.serialPort.IsOpen)
                {
                try {
                    this.serialPort.Write(msg.rgbMessage, 0, msg.rgbMessage.Length);
                    fResult = true;
                    }
                catch (System.TimeoutException)
                    {
                    }
                }
            return fResult;
            }

        //--------------------------------------------------------------------------
        // Data reception
        //--------------------------------------------------------------------------

        public byte ReadByte()
            {
            if (this.putBacks.Count > 0)
                return this.putBacks.Pop();

            return (byte)this.serialPort.ReadByte();
            }

        public void ReadBytes(byte[] rgb, int ib, int cb)
            {
            while (this.putBacks.Count > 0 && cb > 0)
                {
                rgb[ib++] = this.putBacks.Pop();
                cb--;
                }
            this.serialPort.Read(rgb, ib, cb);
            }

        public void SkipBytes(int cb)
            {
            while (putBacks.Count > 0 && cb > 0)
                {
                this.putBacks.Pop();
                cb--;
                }
            while (cb-- > 0)
                {
                this.serialPort.ReadByte();
                }
            }
             
        public int CbAvailable { get { return this.putBacks.Count + this.serialPort.BytesToRead; } }

        Stack<byte> putBacks = new Stack<byte>();

        public void PutBack(byte b)
            {
            this.putBacks.Push(b);
            }
        }
    }
