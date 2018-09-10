namespace nshkj
{
	partial class PreferencesDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.screenshotLabel = new System.Windows.Forms.Label();
            this.screenshotHotKeyTextBox = new System.Windows.Forms.TextBox();
            this.shortcutWarningToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // screenshotLabel
            // 
            this.screenshotLabel.Location = new System.Drawing.Point(16, 37);
            this.screenshotLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.screenshotLabel.Name = "screenshotLabel";
            this.screenshotLabel.Size = new System.Drawing.Size(183, 52);
            this.screenshotLabel.TabIndex = 1;
            this.screenshotLabel.Text = "截屏快捷键：";
            this.screenshotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // screenshotHotKeyTextBox
            // 
            this.screenshotHotKeyTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.screenshotHotKeyTextBox.Location = new System.Drawing.Point(213, 47);
            this.screenshotHotKeyTextBox.Margin = new System.Windows.Forms.Padding(7);
            this.screenshotHotKeyTextBox.Name = "screenshotHotKeyTextBox";
            this.screenshotHotKeyTextBox.ReadOnly = true;
            this.screenshotHotKeyTextBox.Size = new System.Drawing.Size(385, 38);
            this.screenshotHotKeyTextBox.TabIndex = 15;
            this.screenshotHotKeyTextBox.Enter += new System.EventHandler(this.screenshotHotKeyTextBox_Enter);
            this.screenshotHotKeyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.screenshotHotKeyTextBox_KeyDown);
            this.screenshotHotKeyTextBox.Leave += new System.EventHandler(this.screenshotHotKeyTextBox_Leave);
            // 
            // shortcutWarningToolTip
            // 
            this.shortcutWarningToolTip.AutomaticDelay = 0;
            this.shortcutWarningToolTip.IsBalloon = true;
            this.shortcutWarningToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.shortcutWarningToolTip.UseAnimation = false;
            this.shortcutWarningToolTip.UseFading = false;
            // 
            // PreferencesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(216F, 216F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(642, 140);
            this.Controls.Add(this.screenshotHotKeyTextBox);
            this.Controls.Add(this.screenshotLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreferencesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " 科举答题 - 设置";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label screenshotLabel;
		private System.Windows.Forms.TextBox screenshotHotKeyTextBox;
		private System.Windows.Forms.ToolTip shortcutWarningToolTip;
	}
}