//
// TelememtryFTCUI.cs
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using COMTypes = System.Runtime.InteropServices.ComTypes;
using Excel = Microsoft.Office.Interop.Excel;

namespace TelemetryFTC
    {
    public partial class TelemetryFTCUI : Form, IDisposable
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------

        // The serial ports that we know to be Bluetooth underneath
        List<BluetoothSerialPort> bluetoothPorts;

        // The port that is currently opened
        BluetoothSerialPort openedPort;

        // If we have a moniker to the logging destination, then this is it
        COMTypes.IMoniker _pmkLoggingDestination;

        COMTypes.IMoniker pmkLoggingDestination 
            { 
            get { return _pmkLoggingDestination; }
            set { _pmkLoggingDestination = value; Program.Trace("moniker now {0}", null==_pmkLoggingDestination ? "null" : "non-null"); }
            }

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public TelemetryFTCUI()
            {
            Program.TheForm = this;
            COM.OleInitialize(IntPtr.Zero);
            InitializeComponent();
            }

        void IDisposable.Dispose()
            {
            COM.OleUninitialize();
            }

        void InitializeComboBoxPortSelection()
            {
            // Find all the bluetooth serial ports
            //
            this.bluetoothPorts = BluetoothSerialPort.Ports;
            foreach (BluetoothSerialPort port in this.bluetoothPorts)
                {
                port.ProbeForNXT();
                this.comboBoxPortSelection.Items.Add(port);
                }
            
            // Select a reasonable default
            //
            this.comboBoxPortSelection.SelectedIndex = -1;
            //
            if (Program.PortName != null)
                {
                this.comboBoxPortSelection.SelectedIndex = this.comboBoxPortSelection.FindString(Program.PortName);
                }
            if (this.comboBoxPortSelection.SelectedIndex < 0)
                {
                foreach (BluetoothSerialPort port in this.bluetoothPorts)
                    {
                    if (port.HasNXTConnected)
                        {
                        this.comboBoxPortSelection.SelectedIndex = this.comboBoxPortSelection.FindString(port.ListBoxDisplayName);
                        break;
                        }
                    }
                }

            // If we have nothing to connect to, give the user some (miniscule) feedback
            if (null == this.SelectedPort)
                {
                this.comboBoxPortSelection.Items.Add("no NXTs detected");
                this.comboBoxPortSelection.SelectedIndex = this.comboBoxPortSelection.Items.Count -1;
                }

            UpdateEnabledState();
            }

        void InitializeComboBoxMailbox()
            {
            for (int i=0; i < 10; i++)
                {
                this.comboBoxMailbox.Items.Add(new NxtMailboxQueue(i));
                }
            this.comboBoxMailbox.SelectedIndex = this.comboBoxMailbox.FindString
                (
                Program.Mailbox != null 
                    ? Program.Mailbox 
                    : new NxtMailboxQueue(TelemetryFTCConnection.iMailbox).ListBoxDisplayName
                );
            }

        //--------------------------------------------------------------------------
        // Utility
        //--------------------------------------------------------------------------

        public static void ShowWaitCursorWhile(Action action)
            {
            bool fWasWait = false;
            try
                {
                fWasWait = Application.UseWaitCursor;
                Application.UseWaitCursor = true;
                action();
                }
            finally
                {
                Application.UseWaitCursor = fWasWait;
                }
            }

        //--------------------------------------------------------------------------
        // Display management
        //--------------------------------------------------------------------------

        private void NxtLogUI_Shown(object sender, EventArgs e)
            {
            string strOld = labelPortSelection.Text;
            labelPortSelection.Text = "(please wait)";
            //
            InitializeComboBoxPortSelection();
            InitializeComboBoxMailbox();
            //
            this.checkBoxJoystickControl.Checked = Program.Joysticks && JoystickController.HasControllers();
            //
            labelPortSelection.Text = strOld;
            }

        public BluetoothSerialPort SelectedPort { get { 
            if (-1 == this.comboBoxPortSelection.SelectedIndex)
                return null;
            else
                return this.comboBoxPortSelection.Items[this.comboBoxPortSelection.SelectedIndex] as BluetoothSerialPort;
            }}

        private void comboBoxPortSelection_SelectedIndexCommitted(object sender, EventArgs e)
            {
            if (this.openedPort != null)
                {
                if (this.openedPort != this.SelectedPort)
                    {
                    this.openedPort.Close();
                    this.openedPort = null;
                    }
                }
            UpdateEnabledState();
            }

        string IntToWords(int i)
            {
            switch (i)
                {
            case 0: return "zero";
            case 1: return "one";
            case 2: return "two";
            default: return i.ToString();
                }
            }

        void EnableControls(bool fConnection, bool fDisconnection)
            {
            this.buttonDisconnect.Enabled = fDisconnection;

            this.buttonConnect.Enabled             = fConnection;
            this.checkBoxJoystickControl.Enabled   = fConnection && JoystickController.HasControllers();
            this.comboBoxMailbox.Enabled           = fConnection;
            this.comboBoxPortSelection.Enabled     = fConnection;

            this.textBoxLoggingDestination.Enabled = false; // fConnection;   // deprecated

            this.labelJoystickCount.Text = String.Format("{0} joystick{1} detected", 
                IntToWords(JoystickController.Controllers.Count),
                JoystickController.Controllers.Count != 1 ? "s" : ""
                );
            }

        public void UpdateEnabledState()
            {
            BluetoothSerialPort port = SelectedPort;
            if (null == port)
                {
                EnableControls(false, false);
                }
            else if (port.IsOpen)
                {
                EnableControls(false, true);
                }
            else
                {
                EnableControls(true, false);
                }
            }

        private void buttonConnect_Click(object sender, EventArgs e)
            {
            Connect();
            }

        private void buttonDisconnect_Click(object sender, EventArgs e)
            {
            Disconnect();
            }

        private void NxtLogUI_FormClosing(object sender, FormClosingEventArgs e)
            {
            Disconnect();
            }

        //--------------------------------------------------------------------------
        // Connection management
        //--------------------------------------------------------------------------

        void Connect()
            {
            BluetoothSerialPort port = SelectedPort;
            if (port != null)
                {
                if (!port.IsOpen)
                    {
                    ShowWaitCursorWhile(() => { 
                        try {
                            // OpenLoggingDestination();
                            port.Open(); 
                            port.Run(this.checkBoxJoystickControl.Checked);
                            }
                        catch (Exception e)
                            {
                            Program.ReportError("Unable to connect: {0}", e.Message);
                            }
                        });
                    UpdateEnabledState();
                    }
                }
            }

        void Disconnect()
            {
            BluetoothSerialPort port = SelectedPort;
            if (port != null)
                {
                port.Stop();
                port.Close();
                CloseLoggingDestination();
                UpdateEnabledState();
                }
            }

        //--------------------------------------------------------------------------
        // Logging destination 
        //--------------------------------------------------------------------------

        public void OpenLoggingDestinationIfNecessary()
            {
            if (null == Program.TelemetryContext.Sheet)
                {
                OpenLoggingDestination();
                }
            }

        void CloseLoggingDestination()
            {
            Program.TelemetryContext.Initialize();
            }

        void OpenLoggingDestination()
            {
            if (null == this.pmkLoggingDestination && this.textBoxLoggingDestination.Text.Length > 0)
                {
                this.pmkLoggingDestination = COM.MkParseDisplayName(this.textBoxLoggingDestination.Text);
                }
            //
            if (null != this.pmkLoggingDestination)
                {
                // Bind the moniker!
                //
                COMTypes.IBindCtx pbc = COM.CreateBindContext();
                object punk;
                this.pmkLoggingDestination.BindToObject(pbc, null, ref COM.IID_IUnknown, out punk);
                //
                // Is this Excel?
                //
                Excel.Workbook workbook = punk as Excel.Workbook;
                if (null != workbook)
                    {
                    // Make the target visible and selected
                    COM.IOleClientSite clientSite = new COM.OleClientSite();
                    ((COM.IOleObject)punk).DoVerb(COM.OLEIVERB.SHOW, IntPtr.Zero, clientSite, 0, IntPtr.Zero, IntPtr.Zero);

                    // Interogate the selection to get the range to use (we note just the upper left hand corner)
                    dynamic sel = ((Excel.Workbook)punk).Application.Selection;
                    Excel.Range range = (Excel.Range)sel;               // REVIEW: might throw if not just one range?

                    // Remember the Sheet
                    Program.TelemetryContext.Sheet = range.Worksheet;

                    // Remember the location on the Sheet. But if the whole Sheet is selected,
                    // change the selection just as we do for a new blank Sheet. We hit this case
                    // when the moniker we bound is the whole file, e.g., c:\tmp\book2.xlsx
                    long cCellFullSheet = (long)16384 * 1048576;
                    long cCellSelected  = range.CountLarge;
                    if (cCellSelected >= cCellFullSheet)
                        {
                        Program.TelemetryContext.InitCursor(0,0);
                        Program.TelemetryContext.Sheet.get_Range(TelemetryMessage.CellName(0,0)).Select();
                        }
                    else
                        {
                        Program.TelemetryContext.InitCursor(-1, -1);
                        }
                    }
                else
                    {
                    throw new InvalidComObjectException("logging destination not an Excel sheet");
                    }
                }
            else
                {
                // Open a new Excel workbook
                Excel.Application app = GetExcelApp();
                app.Workbooks.Add();
                Program.TelemetryContext.Sheet = (Excel.Worksheet)(app.Worksheets[1]);
                Program.TelemetryContext.InitCursor(0,0);
                }

            if (null != Program.TelemetryContext.Sheet)
                {
                Program.TelemetryContext.Workbook = (Excel.Workbook)(Program.TelemetryContext.Sheet.Parent);
                }
            }

        public static Excel.Application GetExcelApp()
            {
            Excel.Application app = null;

            try {
                // Get reference to Excel.Application from the Running Object Table
	            app = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                if (app != null)
                    {
                    // Don't use any hidden excel instances that might be around
                    if (!app.Visible)
                        {
                        app = null;
                        }
                    }
                }
            catch (System.Runtime.InteropServices.COMException)
                {
                }

            if (null == app)
                app = new Excel.Application();

            app.Visible     = true;
            app.UserControl = true;

            return app;
            }

        const string cfLinkSource = "Link Source";

        private void textBoxLoggingDestination_DragEnter(object sender, DragEventArgs e)
            {
            if (CanAcceptDrop(e))
                {
                e.Effect = ComputeDropEffect(e);
                }
            }

        private void textBoxLoggingDestination_DragOver(object sender, DragEventArgs e)
            {
            if (CanAcceptDrop(e))
                {
                e.Effect = ComputeDropEffect(e);
                }
            }

        private void textBoxLoggingDestination_DragLeave(object sender, EventArgs e)
            {
            }

        private void textBoxLoggingDestination_DragDrop(object sender, DragEventArgs e)
            {
            if (CanAcceptDrop(e))
                {
                // Dig out and reify the serialized moniker which is living in the Link Source
                //
                COMTypes.IMoniker pmkLoggingDestination = MonikerFromData(e.Data);
                if (null != pmkLoggingDestination)
                    {
                    // Find out the moniker's display name
                    //
                    COMTypes.IBindCtx pbc = COM.CreateBindContext();
                    string displayName;
                    pmkLoggingDestination.GetDisplayName(pbc, null, out displayName);
                    //
                    // Put it on the screen and remember it
                    //
                    this.textBoxLoggingDestination.Text = displayName;
                    this.pmkLoggingDestination = pmkLoggingDestination;
                    }

                // Report 'no dice' so the source leaves what we were dragging alone:
                // we were only really interested in the moniker. But if we can tell
                // them we *did* make a link, then we do so, so he might better maintain
                // it for us (good moniker hygene, but not really important in Excel).
                if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
                    e.Effect = DragDropEffects.Link;
                else
                    e.Effect = DragDropEffects.None;
                }
            }

        COMTypes.IMoniker MonikerFromData(IDataObject oData)
            {
            byte[] rgb = COM.GetData(oData, cfLinkSource);
            if (null != rgb)
                {
                return COM.LoadMoniker(rgb);
                }
            return null;
            }

        bool CanAcceptDrop(DragEventArgs e)
            {
            // Is there a moniker?
            if (e.Data.GetDataPresent(cfLinkSource, false))
                {
                // Is the thing on the other side of the moniker Excel?
                COMTypes.IMoniker pmk = MonikerFromData(e.Data);
                if (null != pmk)
                    {
                    COMTypes.IBindCtx pbc = COM.CreateBindContext();
                    object punk;
                    pmk.BindToObject(pbc, null, ref COM.IID_IUnknown, out punk);
                    Excel.Workbook workbook = punk as Excel.Workbook;
                    //
                    return workbook != null;
                    }
                }
            return false;
            }

        DragDropEffects ComputeDropEffect(DragEventArgs e)
            {
            if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
                return DragDropEffects.Link;
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                return DragDropEffects.Move;
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                return DragDropEffects.Copy;
            else
                return DragDropEffects.None;
            }

        private void textBoxLoggingDestination_TextChanged(object sender, EventArgs e)
            {
            this.pmkLoggingDestination = null;
            }

        private void statusBarTeamWebSite_Click(object sender, EventArgs e)
            {
            System.Diagnostics.Process.Start("http://www.ftc417.org");
            }

        private void statusBarHelp_Click(object sender, EventArgs e)
            {
            System.Diagnostics.Process.Start("http://www.ftc417.org/TelemetryFTC");
            }

        private void timerJoystickTransmission_Tick(object sender, EventArgs e)
        // We use a timer to send joystick messages so as to keep the action on 
        // the thread/apartment in which all our DirectInput objects were created.
            {
            if (null != Program.ActiveBTPort)
                {
                Program.ActiveBTPort.SendJoystickMessage();
                }
            }

        }

    class NxtMailboxQueue   
    // Just for being in a list box
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------
    
        int iMailbox;

        public string ListBoxDisplayName { get { return "mailbox" + (this.iMailbox+1); }}

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public NxtMailboxQueue(int iMailbox)
            {
            this.iMailbox = iMailbox;
            }
        }
    }
