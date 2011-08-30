//
// TelemetryFTCUI.Designer.cs
//
namespace TelemetryFTC
    {
    partial class TelemetryFTCUI
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TelemetryFTCUI));
            this.checkBoxJoystickControl = new System.Windows.Forms.CheckBox();
            this.comboBoxNXTSelection = new System.Windows.Forms.ComboBox();
            this.labelNXTSelection = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.textBoxTelemetryDestination = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarBanter = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarTeamWebSite = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarSpacing = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarHelp = new System.Windows.Forms.ToolStripStatusLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.labelJoystickCount = new System.Windows.Forms.Label();
            this.timerJoystickTransmission = new System.Windows.Forms.Timer(this.components);
            this.timerTelemetryPolling = new System.Windows.Forms.Timer(this.components);
            this.buttonAdvanced = new System.Windows.Forms.Button();
            this.buttonRescan = new System.Windows.Forms.Button();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.buttonPoll = new System.Windows.Forms.Button();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxJoystickControl
            // 
            this.checkBoxJoystickControl.AutoSize = true;
            this.checkBoxJoystickControl.Location = new System.Drawing.Point(74, 63);
            this.checkBoxJoystickControl.Name = "checkBoxJoystickControl";
            this.checkBoxJoystickControl.Size = new System.Drawing.Size(177, 17);
            this.checkBoxJoystickControl.TabIndex = 2;
            this.checkBoxJoystickControl.Text = "Send joystick controllers to NXT";
            this.checkBoxJoystickControl.UseVisualStyleBackColor = true;
            // 
            // comboBoxNXTSelection
            // 
            this.comboBoxNXTSelection.DisplayMember = "ListBoxDisplayName";
            this.comboBoxNXTSelection.FormattingEnabled = true;
            this.comboBoxNXTSelection.Location = new System.Drawing.Point(74, 12);
            this.comboBoxNXTSelection.Name = "comboBoxNXTSelection";
            this.comboBoxNXTSelection.Size = new System.Drawing.Size(195, 21);
            this.comboBoxNXTSelection.Sorted = true;
            this.comboBoxNXTSelection.TabIndex = 0;
            this.comboBoxNXTSelection.SelectionChangeCommitted += new System.EventHandler(this.comboBoxNXTSelection_SelectedIndexCommitted);
            // 
            // labelNXTSelection
            // 
            this.labelNXTSelection.AutoSize = true;
            this.labelNXTSelection.Location = new System.Drawing.Point(12, 16);
            this.labelNXTSelection.Name = "labelNXTSelection";
            this.labelNXTSelection.Size = new System.Drawing.Size(32, 13);
            this.labelNXTSelection.TabIndex = 6;
            this.labelNXTSelection.Text = "NXT:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Enabled = false;
            this.buttonConnect.Location = new System.Drawing.Point(15, 91);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(147, 91);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 5;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // textBoxTelemetryDestination
            // 
            this.textBoxTelemetryDestination.AllowDrop = true;
            this.textBoxTelemetryDestination.Location = new System.Drawing.Point(12, 173);
            this.textBoxTelemetryDestination.Name = "textBoxTelemetryDestination";
            this.textBoxTelemetryDestination.Size = new System.Drawing.Size(307, 20);
            this.textBoxTelemetryDestination.TabIndex = 3;
            this.textBoxTelemetryDestination.Visible = false;
            this.textBoxTelemetryDestination.TextChanged += new System.EventHandler(this.textBoxTelemetryDestination_TextChanged);
            this.textBoxTelemetryDestination.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxTelemetryDestination_DragDrop);
            this.textBoxTelemetryDestination.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxTelemetryDestination_DragEnter);
            this.textBoxTelemetryDestination.DragOver += new System.Windows.Forms.DragEventHandler(this.textBoxTelemetryDestination_DragOver);
            this.textBoxTelemetryDestination.DragLeave += new System.EventHandler(this.textBoxTelemetryDestination_DragLeave);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarBanter,
            this.statusBarTeamWebSite,
            this.statusBarSpacing,
            this.statusBarHelp});
            this.statusBar.Location = new System.Drawing.Point(0, 124);
            this.statusBar.Name = "statusBar";
            this.statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusBar.Size = new System.Drawing.Size(368, 22);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 12;
            // 
            // statusBarBanter
            // 
            this.statusBarBanter.AutoSize = false;
            this.statusBarBanter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBarBanter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBarBanter.Name = "statusBarBanter";
            this.statusBarBanter.Size = new System.Drawing.Size(97, 17);
            this.statusBarBanter.Text = "Brought to you by";
            this.statusBarBanter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusBarTeamWebSite
            // 
            this.statusBarTeamWebSite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBarTeamWebSite.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBarTeamWebSite.IsLink = true;
            this.statusBarTeamWebSite.Name = "statusBarTeamWebSite";
            this.statusBarTeamWebSite.Size = new System.Drawing.Size(80, 17);
            this.statusBarTeamWebSite.Text = "FTC Team 417";
            this.statusBarTeamWebSite.Click += new System.EventHandler(this.statusBarTeamWebSite_Click);
            // 
            // statusBarSpacing
            // 
            this.statusBarSpacing.Name = "statusBarSpacing";
            this.statusBarSpacing.Size = new System.Drawing.Size(144, 17);
            this.statusBarSpacing.Spring = true;
            // 
            // statusBarHelp
            // 
            this.statusBarHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBarHelp.IsLink = true;
            this.statusBarHelp.Name = "statusBarHelp";
            this.statusBarHelp.Size = new System.Drawing.Size(32, 17);
            this.statusBarHelp.Text = "Help";
            this.statusBarHelp.Click += new System.EventHandler(this.statusBarHelp_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Joysticks:";
            // 
            // labelJoystickCount
            // 
            this.labelJoystickCount.AutoSize = true;
            this.labelJoystickCount.Location = new System.Drawing.Point(71, 45);
            this.labelJoystickCount.Name = "labelJoystickCount";
            this.labelJoystickCount.Size = new System.Drawing.Size(161, 13);
            this.labelJoystickCount.TabIndex = 14;
            this.labelJoystickCount.Text = "zero joystick controllers detected";
            // 
            // timerJoystickTransmission
            // 
            this.timerJoystickTransmission.Interval = 65;
            this.timerJoystickTransmission.Tick += new System.EventHandler(this.timerJoystickTransmission_Tick);
            // 
            // timerTelemetryPolling
            // 
            this.timerTelemetryPolling.Interval = 30;
            this.timerTelemetryPolling.Tick += new System.EventHandler(this.timerTelemetryPolling_Tick);
            // 
            // buttonAdvanced
            // 
            this.buttonAdvanced.Location = new System.Drawing.Point(279, 90);
            this.buttonAdvanced.Name = "buttonAdvanced";
            this.buttonAdvanced.Size = new System.Drawing.Size(77, 24);
            this.buttonAdvanced.TabIndex = 15;
            this.buttonAdvanced.Text = "Advanced...";
            this.buttonAdvanced.UseVisualStyleBackColor = true;
            this.buttonAdvanced.Click += new System.EventHandler(this.buttonAdvanced_Click);
            // 
            // buttonRescan
            // 
            this.buttonRescan.Location = new System.Drawing.Point(279, 12);
            this.buttonRescan.Name = "buttonRescan";
            this.buttonRescan.Size = new System.Drawing.Size(77, 24);
            this.buttonRescan.TabIndex = 16;
            this.buttonRescan.Text = "Rescan";
            this.buttonRescan.UseVisualStyleBackColor = true;
            this.buttonRescan.Click += new System.EventHandler(this.buttonRescan_Click);
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(279, 48);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(37, 24);
            this.buttonQuery.TabIndex = 17;
            this.buttonQuery.Text = "Qry";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // buttonPoll
            // 
            this.buttonPoll.Location = new System.Drawing.Point(319, 48);
            this.buttonPoll.Name = "buttonPoll";
            this.buttonPoll.Size = new System.Drawing.Size(37, 24);
            this.buttonPoll.TabIndex = 18;
            this.buttonPoll.Text = "Poll";
            this.buttonPoll.UseVisualStyleBackColor = true;
            this.buttonPoll.Click += new System.EventHandler(this.buttonPoll_Click);
            // 
            // TelemetryFTCUI
            // 
            this.AcceptButton = this.buttonConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 146);
            this.Controls.Add(this.buttonPoll);
            this.Controls.Add(this.buttonQuery);
            this.Controls.Add(this.buttonRescan);
            this.Controls.Add(this.buttonAdvanced);
            this.Controls.Add(this.labelJoystickCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.textBoxTelemetryDestination);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.labelNXTSelection);
            this.Controls.Add(this.comboBoxNXTSelection);
            this.Controls.Add(this.checkBoxJoystickControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TelemetryFTCUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "TelemetryFTC v0.8";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelemetryFTCUI_FormClosing);
            this.Shown += new System.EventHandler(this.TelemetryFTCUI_Shown);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxJoystickControl;
        private System.Windows.Forms.ComboBox comboBoxNXTSelection;
        private System.Windows.Forms.Label labelNXTSelection;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.TextBox textBoxTelemetryDestination;
        private System.Windows.Forms.ToolStripStatusLabel statusBarBanter;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusBarTeamWebSite;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelJoystickCount;
        private System.Windows.Forms.ToolStripStatusLabel statusBarHelp;
        private System.Windows.Forms.ToolStripStatusLabel statusBarSpacing;
        public System.Windows.Forms.Timer timerJoystickTransmission;
        private System.Windows.Forms.Timer timerTelemetryPolling;
        private System.Windows.Forms.Button buttonAdvanced;
        private System.Windows.Forms.Button buttonRescan;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Button buttonPoll;
        }
    }