namespace TelemetryFTC
    {
    partial class TelemetryFTCUIOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TelemetryFTCUIOptions));
            this.comboBoxMailbox = new System.Windows.Forms.ComboBox();
            this.labelMailbox = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxMailbox
            // 
            this.comboBoxMailbox.DisplayMember = "ListBoxDisplayName";
            this.comboBoxMailbox.FormattingEnabled = true;
            this.comboBoxMailbox.Location = new System.Drawing.Point(15, 38);
            this.comboBoxMailbox.Name = "comboBoxMailbox";
            this.comboBoxMailbox.Size = new System.Drawing.Size(220, 21);
            this.comboBoxMailbox.TabIndex = 8;
            // 
            // labelMailbox
            // 
            this.labelMailbox.AutoSize = true;
            this.labelMailbox.Location = new System.Drawing.Point(12, 9);
            this.labelMailbox.Name = "labelMailbox";
            this.labelMailbox.Size = new System.Drawing.Size(161, 13);
            this.labelMailbox.TabIndex = 7;
            this.labelMailbox.Text = "NXT mailbox queue to use when";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(15, 79);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(160, 79);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "connected to NXT with Bluetooth:";
            // 
            // TelemetryFTCUIOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 114);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxMailbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelMailbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TelemetryFTCUIOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "TelemetryFTC Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelemetryFTCUIOptions_FormClosing);
            this.Shown += new System.EventHandler(this.TelemetryFTCUIOptions_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxMailbox;
        private System.Windows.Forms.Label labelMailbox;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        }
    }