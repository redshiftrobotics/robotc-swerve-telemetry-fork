//
// TelemetryFTCUIOptions.cs
//
// Advanced configuration for our little app
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TelemetryFTC
    {
    public partial class TelemetryFTCUIOptions : Form
        {
        public TelemetryFTCUIOptions()
            {
            InitializeComponent();
            }

        private void buttonOK_Click(object sender, EventArgs e)
            {
            NxtMailboxQueue mailbox = this.comboBoxMailbox.SelectedItem as NxtMailboxQueue;
            if (null != mailbox)
                {
                BluetoothConnection.iTelemetryMailbox = mailbox.iMailbox;
                }
            }

        private void TelemetryFTCUIOptions_Shown(object sender, EventArgs e)
            {
            InitializeComboBoxMailbox();
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
                    : new NxtMailboxQueue(BluetoothConnection.iTelemetryMailbox).ListBoxDisplayName
                );
            }

        private void TelemetryFTCUIOptions_FormClosing(object sender, FormClosingEventArgs e)
            {
            }

        }

    class NxtMailboxQueue   
    // Just for being in a list box
        {
        //--------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------
    
        public int iMailbox;

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
