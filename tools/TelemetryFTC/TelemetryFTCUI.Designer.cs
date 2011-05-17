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
            this.checkBoxJoystickControl = new System.Windows.Forms.CheckBox();
            this.comboBoxPortSelection = new System.Windows.Forms.ComboBox();
            this.labelPortSelection = new System.Windows.Forms.Label();
            this.labelMailbox = new System.Windows.Forms.Label();
            this.comboBoxMailbox = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.textBoxLoggingDestination = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarBanter = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarTeamWebSite = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarSpacing = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarHelp = new System.Windows.Forms.ToolStripStatusLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.labelJoystickCount = new System.Windows.Forms.Label();
            this.timerJoystickTransmission = new System.Windows.Forms.Timer(this.components);
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxJoystickControl
            // 
            this.checkBoxJoystickControl.AutoSize = true;
            this.checkBoxJoystickControl.Location = new System.Drawing.Point(97, 80);
            this.checkBoxJoystickControl.Name = "checkBoxJoystickControl";
            this.checkBoxJoystickControl.Size = new System.Drawing.Size(134, 17);
            this.checkBoxJoystickControl.TabIndex = 2;
            this.checkBoxJoystickControl.Text = "Send Joysticks to NXT";
            this.checkBoxJoystickControl.UseVisualStyleBackColor = true;
            // 
            // comboBoxPortSelection
            // 
            this.comboBoxPortSelection.DisplayMember = "ListBoxDisplayName";
            this.comboBoxPortSelection.FormattingEnabled = true;
            this.comboBoxPortSelection.Location = new System.Drawing.Point(97, 6);
            this.comboBoxPortSelection.Name = "comboBoxPortSelection";
            this.comboBoxPortSelection.Size = new System.Drawing.Size(150, 21);
            this.comboBoxPortSelection.Sorted = true;
            this.comboBoxPortSelection.TabIndex = 0;
            this.comboBoxPortSelection.SelectionChangeCommitted += new System.EventHandler(this.comboBoxPortSelection_SelectedIndexCommitted);
            // 
            // labelPortSelection
            // 
            this.labelPortSelection.AutoSize = true;
            this.labelPortSelection.Location = new System.Drawing.Point(12, 9);
            this.labelPortSelection.Name = "labelPortSelection";
            this.labelPortSelection.Size = new System.Drawing.Size(77, 13);
            this.labelPortSelection.TabIndex = 6;
            this.labelPortSelection.Text = "Bluetooth Port:";
            // 
            // labelMailbox
            // 
            this.labelMailbox.AutoSize = true;
            this.labelMailbox.Location = new System.Drawing.Point(12, 36);
            this.labelMailbox.Name = "labelMailbox";
            this.labelMailbox.Size = new System.Drawing.Size(81, 13);
            this.labelMailbox.TabIndex = 7;
            this.labelMailbox.Text = "Mailbox Queue:";
            // 
            // comboBoxMailbox
            // 
            this.comboBoxMailbox.DisplayMember = "ListBoxDisplayName";
            this.comboBoxMailbox.FormattingEnabled = true;
            this.comboBoxMailbox.Location = new System.Drawing.Point(97, 33);
            this.comboBoxMailbox.Name = "comboBoxMailbox";
            this.comboBoxMailbox.Size = new System.Drawing.Size(150, 21);
            this.comboBoxMailbox.TabIndex = 1;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Enabled = false;
            this.buttonConnect.Location = new System.Drawing.Point(73, 202);
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
            this.buttonDisconnect.Location = new System.Drawing.Point(176, 202);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 5;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // textBoxLoggingDestination
            // 
            this.textBoxLoggingDestination.AllowDrop = true;
            this.textBoxLoggingDestination.Location = new System.Drawing.Point(12, 173);
            this.textBoxLoggingDestination.Name = "textBoxLoggingDestination";
            this.textBoxLoggingDestination.Size = new System.Drawing.Size(307, 20);
            this.textBoxLoggingDestination.TabIndex = 3;
            this.textBoxLoggingDestination.TextChanged += new System.EventHandler(this.textBoxLoggingDestination_TextChanged);
            this.textBoxLoggingDestination.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxLoggingDestination_DragDrop);
            this.textBoxLoggingDestination.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxLoggingDestination_DragEnter);
            this.textBoxLoggingDestination.DragOver += new System.Windows.Forms.DragEventHandler(this.textBoxLoggingDestination_DragOver);
            this.textBoxLoggingDestination.DragLeave += new System.EventHandler(this.textBoxLoggingDestination_DragLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data logging destination:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Leave empty to create a new Excel sheet";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Drag && drop from an Excel cell to indicate specific location";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Or type the name of an Excel file";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarBanter,
            this.statusBarTeamWebSite,
            this.statusBarSpacing,
            this.statusBarHelp});
            this.statusBar.Location = new System.Drawing.Point(0, 236);
            this.statusBar.Name = "statusBar";
            this.statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusBar.Size = new System.Drawing.Size(331, 22);
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
            this.statusBarSpacing.Size = new System.Drawing.Size(107, 17);
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
            this.label5.Location = new System.Drawing.Point(12, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Joysticks:";
            // 
            // labelJoystickCount
            // 
            this.labelJoystickCount.AutoSize = true;
            this.labelJoystickCount.Location = new System.Drawing.Point(97, 62);
            this.labelJoystickCount.Name = "labelJoystickCount";
            this.labelJoystickCount.Size = new System.Drawing.Size(115, 13);
            this.labelJoystickCount.TabIndex = 14;
            this.labelJoystickCount.Text = "zero joysticks detected";
            // 
            // timerJoystickTransmission
            // 
            this.timerJoystickTransmission.Interval = 65;
            this.timerJoystickTransmission.Tick += new System.EventHandler(this.timerJoystickTransmission_Tick);
            // 
            // TelemetryFTCUI
            // 
            this.AcceptButton = this.buttonConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 258);
            this.Controls.Add(this.labelJoystickCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLoggingDestination);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.comboBoxMailbox);
            this.Controls.Add(this.labelMailbox);
            this.Controls.Add(this.labelPortSelection);
            this.Controls.Add(this.comboBoxPortSelection);
            this.Controls.Add(this.checkBoxJoystickControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TelemetryFTCUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "TelemetryFTC v0.6";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NxtLogUI_FormClosing);
            this.Shown += new System.EventHandler(this.NxtLogUI_Shown);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxJoystickControl;
        private System.Windows.Forms.ComboBox comboBoxPortSelection;
        private System.Windows.Forms.Label labelPortSelection;
        private System.Windows.Forms.Label labelMailbox;
        private System.Windows.Forms.ComboBox comboBoxMailbox;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.TextBox textBoxLoggingDestination;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripStatusLabel statusBarBanter;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusBarTeamWebSite;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelJoystickCount;
        private System.Windows.Forms.ToolStripStatusLabel statusBarHelp;
        private System.Windows.Forms.ToolStripStatusLabel statusBarSpacing;
        public System.Windows.Forms.Timer timerJoystickTransmission;
        }
    }