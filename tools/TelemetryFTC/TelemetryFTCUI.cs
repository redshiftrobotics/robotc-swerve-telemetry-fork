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

        // The NXTs that we know about
        List<KnownNXT> knownNXTs;

        // The the currently active NXT, if any
        KnownNXT currentNXT;

        // If we have a moniker to the telemetry destination, then this is it
        COMTypes.IMoniker _pmkTelemetryDestination;

        COMTypes.IMoniker pmkTelemetryDestination 
            { 
            get { return _pmkTelemetryDestination; }
            set { _pmkTelemetryDestination = value; Program.Trace("moniker now {0}", null==_pmkTelemetryDestination ? "null" : "non-null"); }
            }

        //--------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------

        public TelemetryFTCUI()
            {
            Program.TheForm = this;
            WIN32.OleInitialize(IntPtr.Zero);
            InitializeComponent();
            }

        void IDisposable.Dispose()
            {
            WIN32.OleUninitialize();
            }

        void InitializeComboBoxNXTSelection()
            {
            // Initialize to a clean slate
            this.comboBoxNXTSelection.Items.Clear();

            // Populate with all the known NXTs, probing each to get it's connected/disconnected status
            //
            this.knownNXTs = KnownNXT.KnownNXTs;
            foreach (KnownNXT knownNXT in this.knownNXTs)
                {
                knownNXT.ProbeForNXT();
                this.comboBoxNXTSelection.Items.Add(knownNXT);
                }
            
            // Select something in the list of items
            //
            this.comboBoxNXTSelection.SelectedIndex = -1;
            //
            // If the user said to connect to a particular item, then select that one.
            // REVIEW: this needs more testing.
            //
            if (Program.NXTConnectionID != null)
                {
                this.comboBoxNXTSelection.SelectedIndex = this.comboBoxNXTSelection.FindString(Program.NXTConnectionID);
                }
            //
            // If we've still not got anything selected, then select the first known NXT
            // that we've detected as having a live connected NXT.
            // 
            if (this.comboBoxNXTSelection.SelectedIndex < 0)
                {
                foreach (KnownNXT knownNXT in this.knownNXTs)
                    {
                    if (knownNXT.HasNXTConnected==true)
                        {
                        this.comboBoxNXTSelection.SelectedIndex = this.comboBoxNXTSelection.FindString(knownNXT.ListBoxDisplayName);
                        break;
                        }
                    }
                }
            //
            // If there's nothing in the list of NXTs, then add something
            //
            if (this.comboBoxNXTSelection.Items.Count == 0)
                {
                this.comboBoxNXTSelection.Items.Add("no NXTs detected");
                }
            // 
            // Make sure something is selected
            //
            if (null == this.SelectedNXT)
                {
                this.comboBoxNXTSelection.SelectedIndex = this.comboBoxNXTSelection.Items.Count -1;
                }

            UpdateEnabledState();
            }

        //--------------------------------------------------------------------------
        // Message processing
        //--------------------------------------------------------------------------

        protected override void WndProc(ref Message m)
            {
            switch (m.Msg)
                {
            case WIN32.WM_DEVICECHANGE:
                {
                switch ((int)m.WParam)
                    {
                case WIN32.DBT_DEVICEARRIVAL:
                    if (null != this.DeviceArrived)
                        this.DeviceArrived(this, m);
                    break;
                case WIN32.DBT_DEVICEREMOVECOMPLETE:
                    if (null != this.DeviceRemoveComplete)
                        this.DeviceRemoveComplete(this, m);
                    break;
                    }
                }
                break;
                }
            base.WndProc(ref m);
            }

        public delegate void MsgEventHandler(object sender, Message m);

        public event MsgEventHandler    DeviceArrived;
        public event MsgEventHandler    DeviceRemoveComplete;

        //--------------------------------------------------------------------------
        // Utility
        //--------------------------------------------------------------------------

        public static void ShowWaitCursorWhile(Action action)
            {
            Cursor cursorPrev = null;
            try
                {
                cursorPrev = System.Windows.Forms.Cursor.Current;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                action();
                }
            finally
                {
                System.Windows.Forms.Cursor.Current = cursorPrev;
                }
            }

        //--------------------------------------------------------------------------
        // Display management
        //--------------------------------------------------------------------------

        private void TelemetryFTCUI_Shown(object sender, EventArgs e)
            {
            string strOld = labelNXTSelection.Text;
            labelNXTSelection.Text = "(please wait)";
            //
            this.checkBoxJoystickControl.Checked = Program.Joysticks && JoystickController.HasControllers();
            //
            labelNXTSelection.Text = strOld;
            //
#if DEBUG
            this.buttonQuery.Visible = true;
            this.buttonPoll.Visible = true;
#else
            this.buttonQuery.Visible = false;
            this.buttonPoll.Visible = false;
#endif
            //
            InitializeComboBoxNXTSelection();
            }

        public KnownNXT SelectedNXT { get { 
            if (-1 == this.comboBoxNXTSelection.SelectedIndex)
                return null;
            else
                return this.comboBoxNXTSelection.Items[this.comboBoxNXTSelection.SelectedIndex] as KnownNXT;
            }}

        private void comboBoxNXTSelection_SelectedIndexCommitted(object sender, EventArgs e)
            {
            if (this.currentNXT != null)
                {
                if (this.currentNXT != this.SelectedNXT)
                    {
                    this.currentNXT.Stop();
                    this.currentNXT = null;
                    }
                }
            UpdateEnabledState();
            }

        string IntToWords(int i)
            {
            string[] rgNames = new string[] { "zero", "one", "two", "three", "four", "five", "six" };
            if (0 <= i && i < rgNames.Length)
                {
                return rgNames[i];
                }
            return i.ToString();
            }

        static string sJoystickLabelFormatString = "{0} joystick controller{1} connected to PC";

        void EnableControls(bool fConnection, bool fDisconnection)
            {
            this.buttonDisconnect.Enabled = fDisconnection;

            this.buttonConnect.Enabled             = fConnection;
            this.checkBoxJoystickControl.Enabled   = fConnection && JoystickController.HasControllers();
            this.comboBoxNXTSelection.Enabled      = fConnection;

            this.textBoxTelemetryDestination.Enabled = false; // fConnection;   // deprecated

            this.labelJoystickCount.Text = String.Format(sJoystickLabelFormatString, 
                IntToWords(JoystickController.Controllers.Count),
                JoystickController.Controllers.Count != 1 ? "s" : ""
                );
            }

        public void UpdateEnabledState()
            {
            KnownNXT nxt = SelectedNXT;
            if (null == nxt)
                {
                EnableControls(false, false);
                }
            else if (nxt.IsOpen)
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

        private void TelemetryFTCUI_FormClosing(object sender, FormClosingEventArgs e)
            {
            Disconnect();
            }

        //--------------------------------------------------------------------------
        // Connection management
        //--------------------------------------------------------------------------

        void Connect()
            {
            KnownNXT nxt = this.SelectedNXT;
            if (nxt != null)
                {
                if (!nxt.IsOpen)
                    {
                    ShowWaitCursorWhile(() => { 
                        try {
                            nxt.Run(fUseJoystick: this.checkBoxJoystickControl.Checked);
                            }
                        catch (Exception e)
                            {
                            Program.ReportError("Unable to connect to {0}: {1}", nxt.NxtName, e.Message);
                            }
                        });
                    UpdateEnabledState();
                    }
                }
            }

        public void Disconnect()
            {
            KnownNXT nxt = this.SelectedNXT;
            if (nxt != null)
                {
                nxt.Stop();
                DisconnectTelemetryDestination();
                UpdateEnabledState();
                }
            }


        //--------------------------------------------------------------------------
        // Telemetry destination 
        //--------------------------------------------------------------------------

        public void OpenTelemetryDestinationIfNecessary()
            {
            // Deal with the fact that the sheet may have been manually closed,
            // which would leave us with a dangling reference that will throw
            // an exception if we try to use it.
            if (null != Program.TelemetryContext.Sheet)
                {
                try {
                    int dummy = Program.TelemetryContext.Sheet.Index;
                    }
                catch
                    {
                    Program.TelemetryContext.Initialize();
                    }
                }
            //
            if (null == Program.TelemetryContext.Sheet)
                {
                OpenTelemetryDestination();
                }
            }

        public void DisconnectTelemetryDestination()
            {
            Program.TelemetryContext.Initialize();
            }

        void OpenTelemetryDestination()
            {
            if (null == this.pmkTelemetryDestination && this.textBoxTelemetryDestination.Text.Length > 0)
                {
                this.pmkTelemetryDestination = WIN32.MkParseDisplayName(this.textBoxTelemetryDestination.Text);
                }
            //
            if (null != this.pmkTelemetryDestination)
                {
                // Bind the moniker!
                //
                COMTypes.IBindCtx pbc = WIN32.CreateBindContext();
                object punk;
                this.pmkTelemetryDestination.BindToObject(pbc, null, ref WIN32.IID_IUnknown, out punk);
                //
                // Is this Excel?
                //
                Excel.Workbook workbook = punk as Excel.Workbook;
                if (null != workbook)
                    {
                    // Make the target visible and selected
                    WIN32.IOleClientSite clientSite = new WIN32.OleClientSite();
                    ((WIN32.IOleObject)punk).DoVerb(WIN32.OLEIVERB.SHOW, IntPtr.Zero, clientSite, 0, IntPtr.Zero, IntPtr.Zero);

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
                        Program.TelemetryContext.Sheet.get_Range(TelemetryRecord.CellName(0,0)).Select();
                        }
                    else
                        {
                        Program.TelemetryContext.InitCursor(-1, -1);
                        }
                    }
                else
                    {
                    throw new InvalidComObjectException("telemetry destination not an Excel sheet");
                    }
                }
            else
                {
                // Open a new Excel workbook
                //
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

        private void textBoxTelemetryDestination_DragEnter(object sender, DragEventArgs e)
            {
            if (this.CanAcceptDrop(e))
                {
                e.Effect = ComputeDropEffect(e);
                }
            }

        private void textBoxTelemetryDestination_DragOver(object sender, DragEventArgs e)
            {
            if (this.CanAcceptDrop(e))
                {
                e.Effect = ComputeDropEffect(e);
                }
            }

        private void textBoxTelemetryDestination_DragLeave(object sender, EventArgs e)
            {
            }

        private void textBoxTelemetryDestination_DragDrop(object sender, DragEventArgs e)
            {
            if (this.CanAcceptDrop(e))
                {
                // Dig out and reify the serialized moniker which is living in the Link Source
                //
                COMTypes.IMoniker pmkTelemetryDestination = MonikerFromData(e.Data);
                if (null != pmkTelemetryDestination)
                    {
                    // Find out the moniker's display name
                    //
                    COMTypes.IBindCtx pbc = WIN32.CreateBindContext();
                    string displayName;
                    pmkTelemetryDestination.GetDisplayName(pbc, null, out displayName);
                    //
                    // Put it on the screen and remember it
                    //
                    this.textBoxTelemetryDestination.Text = displayName;
                    this.pmkTelemetryDestination = pmkTelemetryDestination;
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
            byte[] rgb = WIN32.GetData(oData, cfLinkSource);
            if (null != rgb)
                {
                return WIN32.LoadMoniker(rgb);
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
                    COMTypes.IBindCtx pbc = WIN32.CreateBindContext();
                    object punk;
                    pmk.BindToObject(pbc, null, ref WIN32.IID_IUnknown, out punk);
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

        private void textBoxTelemetryDestination_TextChanged(object sender, EventArgs e)
            {
            this.pmkTelemetryDestination = null;
            }

        private void statusBarTeamWebSite_Click(object sender, EventArgs e)
            {
            System.Diagnostics.Process.Start("http://www.ftc417.org");
            }

        private void statusBarHelp_Click(object sender, EventArgs e)
            {
            System.Diagnostics.Process.Start("http://www.ftc417.org/TelemetryFTC");
            }

        //---------------------------------

        private void timerJoystickTransmission_Tick(object sender, EventArgs e)
        // We use a timer to send joystick telemetryMessages so as to keep the action on 
        // the thread/apartment in which all our DirectInput objects were created.
            {
            if (null != Program.CurrentNXT)
                {
                Program.CurrentNXT.SendJoystickMessage();
                }
            }

        //---------------------------------

        bool fNXTWantsTelemetryPolling        = true;
        bool fConnectionWantsTelemetryPolling = false;
        int  msTelemetryPollingInterval       = 30;     // 0==as fast as possible (we don't use the timer in that case)

        public bool NXTWantsTelemetryPolling        { get { return this.fNXTWantsTelemetryPolling;        } set { this.fNXTWantsTelemetryPolling        = value; this.UpdateTelemetryPollingTimerState(); }}
        public bool ConnectionWantsTelemetryPolling { get { return this.fConnectionWantsTelemetryPolling; } set { this.fConnectionWantsTelemetryPolling = value; this.UpdateTelemetryPollingTimerState(); }}
        public int  TelemetryPollingInterval        { get { return this.msTelemetryPollingInterval; } set { this.msTelemetryPollingInterval = value; this.UpdateTelemetryPollingTimerState(); }}

        void UpdateTelemetryPollingTimerState()
            {
            bool fPollingEnabled = (this.fNXTWantsTelemetryPolling && this.fConnectionWantsTelemetryPolling);
            if (fPollingEnabled)
                {
                // Manually send a first poll
                //
                if (!this.timerTelemetryPolling.Enabled)
                    {
                    SendTelemetryPollMessage();
                    }

                if (this.msTelemetryPollingInterval != 0)
                    {
                    this.timerTelemetryPolling.Interval = msTelemetryPollingInterval;
                    if (!this.timerTelemetryPolling.Enabled)
                        {
                        Program.Trace("enabling telemetry polling");
                        }
                    this.timerTelemetryPolling.Enabled = true;
                    }
                else
                    {
                    if (this.timerTelemetryPolling.Enabled)
                        {
                        Program.Trace("disabling telemetry polling");
                        }
                    this.timerTelemetryPolling.Enabled = false;
                    }
                }
            else
                {
                if (this.timerTelemetryPolling.Enabled)
                    {
                    Program.Trace("disabling telemetry polling");
                    }
                this.timerTelemetryPolling.Enabled = false;
                }
            }

        private void timerTelemetryPolling_Tick(object sender, EventArgs e)
            {
            SendTelemetryPollMessage();
            }

        void SendTelemetryPollMessage()
            {
            if (null != Program.CurrentNXT)
                {
                Program.CurrentNXT.SendTelemetryPollMessage();
                }
            }

        private void buttonAdvanced_Click(object sender, EventArgs e)
            {
            TelemetryFTCUIOptions options = new TelemetryFTCUIOptions();
            options.ShowDialog(Program.TheForm);
            }

        private void buttonRescan_Click(object sender, EventArgs e)
            {
            // Repopulate the list of available NXTs
            ShowWaitCursorWhile(() => 
                {
                this.Disconnect();
                this.InitializeComboBoxNXTSelection();
                });
            }

        private void buttonQuery_Click(object sender, EventArgs e)
            {
            if (null != Program.CurrentNXT)
                {
                Program.CurrentNXT.QueryAvailableNXTData();
                }
            }

        private void buttonPoll_Click(object sender, EventArgs e)
            {
            SendTelemetryPollMessage();
            }
        }

    }
